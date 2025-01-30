using LaunchDarkly.EventSource;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using WoT.Core.Definitions;
using WoT.Core.Definitions.TD;

namespace WoT.Binding.Http.SubscriptionProtocols
{
    /// <summary>
    /// A Server-Sent Events subscription protocol
    /// </summary>
    public class SSESubscription : IProtocolSubscription, IDisposable
    {
        private readonly Form _form;
        private EventSource _eventSource;
        private bool _closed;
        private bool _disposed;
        public bool Closed { get => _closed; }
        public bool Disposed { get => _disposed; } 
        public SSESubscription(Form form) {
            _form = form;
            _closed = true;
            _disposed = false;
        }

        public void Close()
        {
            _eventSource?.Close();
            Dispose();
        }

        public Task Open(Action<Content> next, Action<Exception> onerror = null, Action complete = null)
        {
            _eventSource = new EventSource(_form.Href);

            TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();
            _eventSource.Opened += (sender, eventArgs) =>
            {
                tcs.SetResult(true);
                _closed = false;
            };

            _eventSource.MessageReceived += (sender, eventArgs) => {
                var data = eventArgs.Message.Data;
                var mem = new MemoryStream(Encoding.UTF8.GetBytes(data));
                Content output = new Content(_form.ContentType ?? ContentSerdes.DEFAULT, mem);

                next(output);
            };

            _eventSource.Error += (sender, eventArgs) => {
                onerror(eventArgs.Exception);
                complete();
                tcs.SetException(eventArgs.Exception);
                _closed = true;
            };

            _eventSource.StartAsync();

            return tcs.Task;
            
        }

        public void Dispose()
        {
            _eventSource.Dispose();
            _closed = true;
            _disposed = true;
        }
    }
}
