using System;
using System.Threading.Tasks;
using WoT.Definitions;
using System.Threading;
namespace WoT.Implementation
{
    /// <summary>
    /// An implementation of <see cref="ISubscription"/>
    /// </summary>
    public class Subscription : ISubscription
    {
        private readonly SubscriptionType _type;
        private readonly string _name;
        private readonly InteractionAffordance _interaction;
        private readonly Form _form;
        private readonly ConsumedThing _thing;
        private bool _active;
        private CancellationToken _cancellationToken;
        public CancellationTokenSource tokenSource;

        public event EventHandler StopEvent;
        public event EventHandler StopObservation;

        /// <summary>
        /// Subscription Types
        /// </summary>
        public enum SubscriptionType
        {
            Event,
            Observation
        }

        public Subscription(SubscriptionType type, string name, InteractionAffordance interaction, Form form, ConsumedThing thing)
        {
            _type = type;
            _name = name;
            _interaction = interaction;
            _thing = thing;
            _active = true;
            tokenSource = new CancellationTokenSource();
            _cancellationToken = tokenSource.Token;
            switch (_type)
            {
                case SubscriptionType.Event:
                    _thing.AddSubscription(_name, this);
                    break;
                case SubscriptionType.Observation:
                    _thing.AddObservation(_name, this);
                    break;
            }

        }

        public bool Active => _active;
        public CancellationToken CancellationToken => _cancellationToken;

        public async Task Stop(InteractionOptions? options = null)
        {
            _active = false;
            if (_type == SubscriptionType.Event)
            {
                _thing.RemoveSubscription(_name);
                this.StopEvent?.Invoke(this, EventArgs.Empty);
            }
            if (_type == SubscriptionType.Observation)
            {
                _thing.RemoveObservation(_name);
                this.StopObservation?.Invoke(this, EventArgs.Empty);
            }
        }
    }

}