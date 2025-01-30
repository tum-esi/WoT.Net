using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using WoT.Core.Definitions;
using WoT.Core.Definitions.TD;

namespace WoT.Binding.Http.SubscriptionProtocols
{
    /// <summary>
    /// A long polling subscription protocol
    /// </summary>
    public class LongPollingSubscription : IProtocolSubscription, IDisposable
    {
        private bool _closed = true;
        private bool _disposed = false;
        private readonly Form _form;
        private readonly HttpClient _client;
        private CancellationTokenSource _cancellationTokenSource;

        public bool Disposed { get => _disposed; }

        public bool Closed  { get => _closed; }

        public LongPollingSubscription(Form form, HttpClient client)
        {
            _form = form;
            _closed = true;
            _disposed = false;
            _client = client;
        }

        public void Close()
        {
            _cancellationTokenSource?.Cancel();
            Dispose();
        }

        public Task Open(Action<Content> next, Action<Exception> onerror = null, Action complete = null)
        {
            TaskCompletionSource<bool> tsc = new TaskCompletionSource<bool>();
            LongPolling(tsc, true, next, onerror, complete);
            return tsc.Task;

        }

        private async Task LongPolling(TaskCompletionSource<bool> tsc, bool handshake, Action<Content> next, Action<Exception> onerror = null, Action complete = null)
        {
            try
            {
                if (handshake)
                {
                    HttpRequestMessage headRequest = _client.GenerateHttpRequest(_form, HttpMethod.Head);
                    CancellationTokenSource HeadRequestcancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(1));
                    await _client.SendRequest(headRequest, HeadRequestcancellationTokenSource.Token);
                    _closed = false;
                    tsc.SetResult(true);
                }

                HttpRequestMessage request = _client.GenerateHttpRequest(_form, HttpMethod.Get);
                _cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromHours(1));

                var result = await _client.SendRequest(request, _cancellationTokenSource.Token);

                if(!_closed)
                {
                    next(result);
                    #pragma warning disable CS4014 
                    LongPolling(tsc, false, next, onerror, complete);
                    #pragma warning restore CS4014
                }

                complete();
            }
            catch (Exception ex)
            {
                onerror(ex);
                complete();
                tsc.SetException(ex);
            }
        }

        public void Dispose()
        {
            _cancellationTokenSource?.Dispose();
            _closed = true;
            _disposed = true;
        }
    }
}
