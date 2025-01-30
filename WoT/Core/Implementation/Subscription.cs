using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WoT.Core.Definitions;
using WoT.Core.Definitions.TD;

namespace WoT.Core.Implementation
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
        private readonly IProtocolSubscription _protocolSubscription;

        /// <summary>
        /// Subscription Types
        /// </summary>
        public enum SubscriptionType
        {
            Event,
            Observation
        }

        /// <summary>
        /// Create a new Subscription
        /// </summary>
        /// <param name="type"></param>
        /// <param name="name"></param>
        /// <param name="interaction"></param>
        /// <param name="form"></param>
        /// <param name="thing"></param>
        /// <param name="internalSub"></param>
        public Subscription(SubscriptionType type, 
            string name, 
            InteractionAffordance interaction, 
            Form form,
            IProtocolSubscription internalSub)
        {
            _type = type;
            _name = name;
            _form = form;
            _interaction = interaction;
            _protocolSubscription = internalSub;
        }

        public bool Active => !_protocolSubscription.Closed;

        public Task Stop(InteractionOptions? options = null)
        {
            var tsc = new TaskCompletionSource<bool>();
            try
            {
                _protocolSubscription.Close();
                tsc.SetResult(true);
            }
            catch (Exception ex)
            {
                tsc.SetException(ex);
            }
            return tsc.Task;
        }

        public int FindBestMatchingUnlinkFormIndex()
        {
            string operation = _type == SubscriptionType.Event ? "unsubscribeevent" : "unobserveproperty";
            int maxScore = 0;
            int maxScoreIndex = -1;

            for (int i = 0; i < _interaction.Forms.Length; i++)
            {
                int score = 0;
                Form currentForm = _interaction.Forms[i];
                List<string> currentOps = new List<string>(currentForm.Op);
                if (currentOps.Contains(operation)) score += 1;
                if (currentForm.Href.Authority == _form.Href.Authority) score += 1;
                if (currentForm.ContentType == _form.ContentType) score += 1;

                if (score > maxScore)
                {
                    maxScore = score;
                    maxScoreIndex = i;
                }
            }

            if (maxScoreIndex < 0)
            {
                string unlinkText;
                string interactionText;
                if (_type == SubscriptionType.Event)
                {
                    interactionText = "event";
                    unlinkText = "unsubscribing";
                }
                else
                {
                    interactionText = "property";
                    unlinkText = "unobserving";
                }
                throw new Exception($"Could not find matching form for {unlinkText} {interactionText} {_name}");
            }
            return maxScoreIndex;
        }

        public Form FindBestMatchingUnlinkForm()
        {
            int maxScoreIndex = FindBestMatchingUnlinkFormIndex();
            return _interaction.Forms[maxScoreIndex];
        }
    }
}