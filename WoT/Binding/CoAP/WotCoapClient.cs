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
            var message = BuildCoapMessage(method, path, payload, contentType, null);

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
                    var response = ParseCoapResponse(result.Buffer);
                    
                    // Handle Block2 (blockwise transfer for responses)
                    if (response.Block2 != null && response.Block2.HasMore)
                    {
                        // Accumulate the complete payload across all blocks
                        var completePayload = new List<byte>();
                        if (response.Payload != null)
                        {
                            completePayload.AddRange(response.Payload);
                        }

                        var blockNum = response.Block2.BlockNumber;
                        var blockSize = response.Block2.BlockSize;

                        // Fetch remaining blocks
                        while (response.Block2.HasMore)
                        {
                            blockNum++;
                            
                            // Build request with Block2 option for next block
                            var block2Value = new Block2Option
                            {
                                BlockNumber = blockNum,
                                HasMore = false,
                                BlockSize = blockSize
                            };
                            
                            var blockMessage = BuildCoapMessage(method, path, null, null, block2Value);
                            await _udpClient.SendAsync(blockMessage, blockMessage.Length, host, port);

                            // Receive next block
                            var blockReceiveTask = _udpClient.ReceiveAsync();
                            var blockCancelTask = Task.Delay(-1, linkedCts.Token);
                            var blockCompletedTask = await Task.WhenAny(blockReceiveTask, blockCancelTask);

                            if (blockCompletedTask == blockReceiveTask)
                            {
                                var blockResult = await blockReceiveTask;
                                response = ParseCoapResponse(blockResult.Buffer);
                                
                                if (response.Payload != null)
                                {
                                    completePayload.AddRange(response.Payload);
                                }
                            }
                            else if (timeoutCts.IsCancellationRequested)
                            {
                                throw new TimeoutException($"CoAP block request to {uri} timed out after {_config.Timeout}ms");
                            }
                            else
                            {
                                throw new OperationCanceledException("CoAP block request was cancelled", cancellationToken);
                            }
                        }

                        // Return response with complete payload
                        response.Payload = completePayload.ToArray();
                    }
                    
                    return response;
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

        private byte[] BuildCoapMessage(CoapMethod method, string path, byte[] payload, string contentType, Block2Option block2Option)
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

                // Block2 option (option 23) for blockwise transfer
                if (block2Option != null)
                {
                    int optionNumber = 23;
                    byte[] block2Bytes = EncodeBlock2Option(block2Option);
                    lastOptionNumber = WriteOption(ms, optionNumber, block2Bytes, lastOptionNumber);
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

            // Parse options and find payload
            byte[] payload = null;
            Block2Option block2 = null;
            int pos = 4; // Start after header
            
            // Parse options until we hit payload marker (0xFF) or end of data
            int previousOptionNumber = 0;
            while (pos < data.Length)
            {
                byte b = data[pos];
                
                // Check for payload marker
                if (b == 0xFF)
                {
                    pos++; // Skip payload marker
                    break;
                }
                
                // Parse option header
                int delta = (b >> 4) & 0x0F;
                int length = b & 0x0F;
                pos++;
                
                // Handle extended delta
                if (delta == 13)
                {
                    if (pos >= data.Length) break;
                    delta = data[pos] + 13;
                    pos++;
                }
                else if (delta == 14)
                {
                    if (pos + 1 >= data.Length) break;
                    delta = ((data[pos] << 8) | data[pos + 1]) + 269;
                    pos += 2;
                }
                
                // Handle extended length
                if (length == 13)
                {
                    if (pos >= data.Length) break;
                    length = data[pos] + 13;
                    pos++;
                }
                else if (length == 14)
                {
                    if (pos + 1 >= data.Length) break;
                    length = ((data[pos] << 8) | data[pos + 1]) + 269;
                    pos += 2;
                }
                
                int optionNumber = previousOptionNumber + delta;
                previousOptionNumber = optionNumber;
                
                // Extract option value
                byte[] optionValue = new byte[length];
                if (length > 0 && pos + length <= data.Length)
                {
                    Array.Copy(data, pos, optionValue, 0, length);
                    pos += length;
                }
                
                // Parse Block2 option (option 23)
                if (optionNumber == 23 && optionValue.Length > 0)
                {
                    block2 = DecodeBlock2Option(optionValue);
                }
            }

            // Extract payload if present
            if (pos < data.Length)
            {
                payload = new byte[data.Length - pos];
                Array.Copy(data, pos, payload, 0, payload.Length);
            }

            return new CoapResponse
            {
                Code = code,
                CodeClass = codeClass,
                CodeDetail = codeDetail,
                Payload = payload,
                IsSuccess = codeClass == 2, // 2.xx codes are success
                Block2 = block2
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

        private byte[] EncodeBlock2Option(Block2Option block2)
        {
            // Block2 encoding: NUM(variable)|M(1)|SZX(3)
            // NUM = block number, M = more flag, SZX = size exponent (0-6)
            int szx = GetSizeExponent(block2.BlockSize);
            int value = (block2.BlockNumber << 4) | ((block2.HasMore ? 1 : 0) << 3) | szx;
            
            // Encode as variable-length integer (1-3 bytes)
            if (value < 256)
            {
                return new byte[] { (byte)value };
            }
            else if (value < 65536)
            {
                return new byte[] { (byte)(value >> 8), (byte)(value & 0xFF) };
            }
            else
            {
                return new byte[] { (byte)(value >> 16), (byte)((value >> 8) & 0xFF), (byte)(value & 0xFF) };
            }
        }

        private Block2Option DecodeBlock2Option(byte[] data)
        {
            if (data == null || data.Length == 0)
                return null;

            // Decode variable-length integer
            int value = 0;
            for (int i = 0; i < data.Length; i++)
            {
                value = (value << 8) | data[i];
            }

            // Extract fields: NUM(variable)|M(1)|SZX(3)
            int szx = value & 0x07;
            bool hasMore = ((value >> 3) & 0x01) == 1;
            int blockNumber = value >> 4;
            int blockSize = 1 << (szx + 4); // 2^(SZX + 4)

            return new Block2Option
            {
                BlockNumber = blockNumber,
                HasMore = hasMore,
                BlockSize = blockSize
            };
        }

        private int GetSizeExponent(int blockSize)
        {
            // Convert block size to SZX (size exponent)
            // Block sizes: 16(0), 32(1), 64(2), 128(3), 256(4), 512(5), 1024(6)
            switch (blockSize)
            {
                case 16: return 0;
                case 32: return 1;
                case 64: return 2;
                case 128: return 3;
                case 256: return 4;
                case 512: return 5;
                case 1024: return 6;
                default: return 6; // Default to 1024
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

        private class Block2Option
        {
            public int BlockNumber { get; set; }
            public bool HasMore { get; set; }
            public int BlockSize { get; set; }
        }

        private class CoapResponse
        {
            public byte Code { get; set; }
            public int CodeClass { get; set; }
            public int CodeDetail { get; set; }
            public byte[] Payload { get; set; }
            public bool IsSuccess { get; set; }
            public Block2Option Block2 { get; set; }
        }

        #endregion
    }
}
