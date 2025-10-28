using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WoT.Core.Definitions;
using WoT.Core.Definitions.TD;

namespace WoT.Binding.CoAP
{
    /// <summary>
    /// A basic CoAP client implementation of <see cref="IProtocolClient"/> for the WoT Consumer
    /// </summary>
    /// <remarks>
    /// This is a minimal CoAP implementation supporting basic GET, PUT, and POST operations.
    /// For production use with advanced features (blockwise transfer, observe, DTLS),
    /// consider integrating a full-featured CoAP library.
    /// </remarks>
    public class WotCoapClient : IProtocolClient, IDisposable
    {
        private readonly CoapClientConfig _config;
        private readonly UdpClient _udpClient;
        private ushort _messageId = 0;
        private bool _disposed = false;

        /// <summary>
        /// The protocol scheme of this client
        /// </summary>
        public string Scheme { get; internal set; } = "coap";

        /// <summary>
        /// Create a new <see cref="WotCoapClient"/>
        /// </summary>
        /// <param name="config">CoAP client configuration</param>
        public WotCoapClient(CoapClientConfig config)
        {
            _config = config ?? new CoapClientConfig();
            _udpClient = new UdpClient();
            // Don't set ReceiveTimeout on socket - timeout is handled in SendCoapRequest
        }

        #region ReadResource

        public async Task<Content> ReadResource(Form form)
        {
            return await ReadResource(form, CancellationToken.None);
        }

        public async Task<Content> ReadResource(Form form, CancellationToken cancellationToken)
        {
            var response = await SendCoapRequest(form.Href.ToString(), CoapMethod.GET, null, null, cancellationToken);
            return CreateContentFromResponse(response);
        }

        #endregion

        #region WriteResource

        public async Task WriteResource(Form form, Content content)
        {
            await WriteResource(form, content, CancellationToken.None);
        }

        public async Task WriteResource(Form form, Content content, CancellationToken cancellationToken)
        {
            byte[] payload = content != null ? ReadStreamToBytes(content.body) : null;
            string contentType = content?.type;
            await SendCoapRequest(form.Href.ToString(), CoapMethod.PUT, payload, contentType, cancellationToken);
        }

        #endregion

        #region InvokeResource

        public async Task<Content> InvokeResource(Form form)
        {
            return await InvokeResource(form, null, CancellationToken.None);
        }

        public async Task<Content> InvokeResource(Form form, CancellationToken cancellationToken)
        {
            return await InvokeResource(form, null, cancellationToken);
        }

        public async Task<Content> InvokeResource(Form form, Content content)
        {
            return await InvokeResource(form, content, CancellationToken.None);
        }

        public async Task<Content> InvokeResource(Form form, Content content, CancellationToken cancellationToken)
        {
            byte[] payload = content != null ? ReadStreamToBytes(content.body) : null;
            string contentType = content?.type;
            var response = await SendCoapRequest(form.Href.ToString(), CoapMethod.POST, payload, contentType, cancellationToken);
            return CreateContentFromResponse(response);
        }

        #endregion

        #region SubscribeResource

        public async Task<IProtocolSubscription> SubscribeResource(Form form, Action<Content> nextHandler, Action<Exception> errorHandler = null, Action complete = null)
        {
            await Task.CompletedTask;
            // CoAP Observe would be implemented here
            throw new NotImplementedException("CoAP Observe is not yet implemented in this basic CoAP client.");
        }

        public Task UnlinkResource(Form form)
        {
            return Task.CompletedTask;
        }

        #endregion

        public Task Start()
        {
            return Task.CompletedTask;
        }

        public Task Stop()
        {
            Dispose();
            return Task.CompletedTask;
        }

        /// <summary>
        /// Dispose of resources
        /// </summary>
        public void Dispose()
        {
            if (!_disposed)
            {
                _udpClient?.Dispose();
                _disposed = true;
            }
        }

        public bool SetSecurity(SecurityScheme[] metadata, Dictionary<CredentialScheme, object> credentials)
        {
            // DTLS security would be implemented here
            return false;
        }

        public async Task<Content> RequestThingDescription(string url)
        {
            return await RequestThingDescription(new Uri(url));
        }

        public async Task<Content> RequestThingDescription(Uri tdUrl)
        {
            var response = await SendCoapRequest(tdUrl.ToString(), CoapMethod.GET, null, null, CancellationToken.None);
            return CreateContentFromResponse(response, "application/td+json");
        }

        #region CoAP Protocol Implementation

        private async Task<CoapResponse> SendCoapRequest(string uri, CoapMethod method, byte[] payload, string contentType, CancellationToken cancellationToken)
        {
            var parsedUri = new Uri(uri);
            var host = parsedUri.Host;
            var port = parsedUri.Port > 0 ? parsedUri.Port : 5683; // Default CoAP port
            var path = parsedUri.AbsolutePath.TrimStart('/');

            // Build CoAP message
            var message = BuildCoapMessage(method, path, payload, contentType);

            // Send request
            await _udpClient.SendAsync(message, message.Length, host, port);

            // Receive response with timeout and cancellation support
            // Note: UdpClient.ReceiveAsync() doesn't support CancellationToken in .NET Standard 2.0
            // Using Task.WhenAny as a workaround
            using (var timeoutCts = new CancellationTokenSource(_config.Timeout))
            using (var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, timeoutCts.Token))
            {
                var receiveTask = _udpClient.ReceiveAsync();
                var cancelTask = Task.Delay(-1, linkedCts.Token);
                
                var completedTask = await Task.WhenAny(receiveTask, cancelTask);
                
                if (completedTask == receiveTask)
                {
                    var result = await receiveTask;
                    return ParseCoapResponse(result.Buffer);
                }
                else if (timeoutCts.IsCancellationRequested)
                {
                    throw new TimeoutException($"CoAP request to {uri} timed out after {_config.Timeout}ms");
                }
                else
                {
                    throw new OperationCanceledException("CoAP request was cancelled", cancellationToken);
                }
            }
        }

        private byte[] BuildCoapMessage(CoapMethod method, string path, byte[] payload, string contentType)
        {
            using (var ms = new MemoryStream())
            {
                // CoAP header (4 bytes): Ver(2)|T(2)|TKL(4), Code(8), Message ID(16)
                byte ver = 1; // CoAP version 1
                byte type = 0; // CON (Confirmable)
                byte tkl = 0; // Token length (0 for simplicity)
                
                ms.WriteByte((byte)((ver << 6) | (type << 4) | tkl));
                ms.WriteByte(GetMethodCode(method));
                
                // Message ID
                ushort msgId = ++_messageId;
                ms.WriteByte((byte)(msgId >> 8));
                ms.WriteByte((byte)(msgId & 0xFF));

                // Options - must be in order by option number
                int lastOptionNumber = 0;

                // Uri-Path options (option 11)
                if (!string.IsNullOrEmpty(path))
                {
                    var pathSegments = path.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (var segment in pathSegments)
                    {
                        int optionNumber = 11;
                        lastOptionNumber = WriteOption(ms, optionNumber, Encoding.UTF8.GetBytes(segment), lastOptionNumber);
                    }
                }

                // Content-Format option (option 12) - must come after Uri-Path
                if (payload != null && payload.Length > 0 && !string.IsNullOrEmpty(contentType))
                {
                    int contentFormat = GetContentFormatCode(contentType);
                    int optionNumber = 12;
                    byte[] formatBytes;
                    if (contentFormat <= 255)
                    {
                        formatBytes = new byte[] { (byte)contentFormat };
                    }
                    else
                    {
                        formatBytes = new byte[] { (byte)(contentFormat >> 8), (byte)(contentFormat & 0xFF) };
                    }
                    lastOptionNumber = WriteOption(ms, optionNumber, formatBytes, lastOptionNumber);
                }

                // Payload marker and payload
                if (payload != null && payload.Length > 0)
                {
                    ms.WriteByte(0xFF);
                    ms.Write(payload, 0, payload.Length);
                }

                return ms.ToArray();
            }
        }

        private int WriteOption(MemoryStream ms, int optionNumber, byte[] value, int previousOptionNumber)
        {
            int delta = optionNumber - previousOptionNumber;
            int length = value.Length;

            // Encode delta and length according to RFC 7252
            // Note: This implementation supports delta/length values up to 268 (single extended byte)
            // Values 269-65804 would require 2 extended bytes, which is not implemented for simplicity
            int deltaEncoded = delta;
            int lengthEncoded = length;
            byte deltaExtra = 0;
            byte lengthExtra = 0;

            // Handle extended delta (13-268: use 1 extra byte)
            if (delta >= 13 && delta < 269)
            {
                deltaEncoded = 13;
                deltaExtra = (byte)(delta - 13);
            }
            else if (delta >= 269)
            {
                // 2-byte extended delta not implemented - this would require special handling
                throw new NotImplementedException($"CoAP option delta {delta} requires 2-byte extended encoding which is not implemented");
            }

            // Handle extended length (13-268: use 1 extra byte)
            if (length >= 13 && length < 269)
            {
                lengthEncoded = 13;
                lengthExtra = (byte)(length - 13);
            }
            else if (length >= 269)
            {
                // 2-byte extended length not implemented
                throw new NotImplementedException($"CoAP option length {length} requires 2-byte extended encoding which is not implemented");
            }

            // Write option header
            byte optionHeader = (byte)((deltaEncoded << 4) | lengthEncoded);
            ms.WriteByte(optionHeader);

            // Write extended delta if needed
            if (deltaEncoded == 13)
            {
                ms.WriteByte(deltaExtra);
            }

            // Write extended length if needed
            if (lengthEncoded == 13)
            {
                ms.WriteByte(lengthExtra);
            }

            // Write option value
            if (value.Length > 0)
            {
                ms.Write(value, 0, value.Length);
            }

            return optionNumber;
        }

        private CoapResponse ParseCoapResponse(byte[] data)
        {
            if (data.Length < 4)
                throw new Exception("Invalid CoAP response: too short");

            byte header = data[0];
            byte code = data[1];
            
            // Extract response code
            int codeClass = (code >> 5) & 0x07;
            int codeDetail = code & 0x1F;

            // Find payload (after 0xFF marker)
            byte[] payload = null;
            int payloadStart = -1;
            for (int i = 4; i < data.Length; i++)
            {
                if (data[i] == 0xFF)
                {
                    payloadStart = i + 1;
                    break;
                }
            }

            if (payloadStart > 0 && payloadStart < data.Length)
            {
                payload = new byte[data.Length - payloadStart];
                Array.Copy(data, payloadStart, payload, 0, payload.Length);
            }

            return new CoapResponse
            {
                Code = code,
                CodeClass = codeClass,
                CodeDetail = codeDetail,
                Payload = payload,
                IsSuccess = codeClass == 2 // 2.xx codes are success
            };
        }

        private byte GetMethodCode(CoapMethod method)
        {
            switch (method)
            {
                case CoapMethod.GET:
                    return 0x01; // 0.01
                case CoapMethod.POST:
                    return 0x02; // 0.02
                case CoapMethod.PUT:
                    return 0x03; // 0.03
                case CoapMethod.DELETE:
                    return 0x04; // 0.04
                default:
                    return 0x01;
            }
        }

        private int GetContentFormatCode(string contentType)
        {
            if (string.IsNullOrEmpty(contentType))
                return 50; // application/json

            contentType = contentType.ToLowerInvariant().Split(';')[0].Trim();

            switch (contentType)
            {
                case "text/plain":
                    return 0;
                case "application/link-format":
                    return 40;
                case "application/xml":
                    return 41;
                case "application/octet-stream":
                    return 42;
                case "application/exi":
                    return 47;
                case "application/json":
                case "application/td+json":
                    return 50;
                case "application/cbor":
                    return 60;
                default:
                    return 50; // default to JSON
            }
        }

        private Content CreateContentFromResponse(CoapResponse response, string defaultContentType = null)
        {
            if (!response.IsSuccess)
            {
                throw new Exception($"CoAP request failed with code {response.CodeClass}.{response.CodeDetail:D2}");
            }

            string contentType = defaultContentType ?? "application/json";
            byte[] payload = response.Payload ?? new byte[0];
            MemoryStream memStream = new MemoryStream(payload);

            return new Content(contentType, memStream);
        }

        private byte[] ReadStreamToBytes(Stream stream)
        {
            if (stream == null)
                return new byte[0];

            stream.Position = 0;
            using (MemoryStream ms = new MemoryStream())
            {
                stream.CopyTo(ms);
                return ms.ToArray();
            }
        }

        #endregion

        #region Helper Classes

        private enum CoapMethod
        {
            GET,
            POST,
            PUT,
            DELETE
        }

        private class CoapResponse
        {
            public byte Code { get; set; }
            public int CodeClass { get; set; }
            public int CodeDetail { get; set; }
            public byte[] Payload { get; set; }
            public bool IsSuccess { get; set; }
        }

        #endregion
    }
}
