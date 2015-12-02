using CAT.Context;
using CAT.Message.Internals;
using System;

namespace CAT.Message.Spi.Internals
{
    internal class DefaultMessageProducer : IMessageProducer
    {
        private readonly IMessageManager _mManager;

        public DefaultMessageProducer(IMessageManager manager)
        {
            _mManager = manager;
        }

        public String CreateMessageId()
        {
            return _mManager.GetMessageIdFactory().GetNextId();
        }

        #region IMessageProducer Members

        public virtual void LogError(Exception ex, string messageTreeId)
        {
            var type = ex.GetType().FullName;
            var target = ex.TargetSite;
            string name = null;
            if (target != null)
                name = target.DeclaringType.ToString();
            name = name ?? type;
            LogEvent("Exception", name, type, ex.ToString(), messageTreeId);
        }

        public virtual void LogEvent(String type, String name, String status, String nameValuePairs, string messageTreeId)
        {
            IEvent evt0 = NewEvent(type, name, messageTreeId);

            if (!string.IsNullOrEmpty(nameValuePairs))
            {
                evt0.AddData(nameValuePairs);
            }

            evt0.Status = status;
            evt0.Complete(CallContextManager.MessageTreeId);
        }

        public virtual void LogHeartbeat(String type, String name, String status, String nameValuePairs, string messageTreeId)
        {
            IHeartbeat heartbeat = NewHeartbeat(type, name, messageTreeId);

            if (!string.IsNullOrEmpty(nameValuePairs))
            {
                heartbeat.AddData(nameValuePairs);
            }
            heartbeat.Status = status;
            heartbeat.Complete(CallContextManager.MessageTreeId);
        }

        public virtual void LogMetric(String name, String status, String nameValuePairs, string messageTreeId)
        {
            String type = string.Empty;
            IMetric metric = NewMetric(type, name, messageTreeId);

            if (!string.IsNullOrWhiteSpace(nameValuePairs))
            {
                metric.AddData(nameValuePairs);
            }

            metric.Status = status;
            metric.Complete(CallContextManager.MessageTreeId);
        }

        public virtual IEvent NewEvent(String type, String name, string messageTreeId)
        {
            if (!_mManager.HasContext(messageTreeId))
            {
                _mManager.Setup(messageTreeId);
            }

            if (_mManager.CatEnabled(messageTreeId))
            {
                IEvent evt0 = new DefaultEvent(type, name);

                _mManager.Add(evt0, messageTreeId);
                return evt0;
            }
            return new NullEvent();
        }

        public virtual IHeartbeat NewHeartbeat(String type, String name, string messageTreeId)
        {
            if (!_mManager.HasContext(messageTreeId))
            {
                _mManager.Setup(messageTreeId);
            }

            if (_mManager.CatEnabled(messageTreeId))
            {
                IHeartbeat heartbeat = new DefaultHeartbeat(type, name);

                _mManager.Add(heartbeat, messageTreeId);
                return heartbeat;
            }
            return new NullHeartbeat();
        }

        public virtual ITransaction NewTransaction(String type, String name, string messageTreeId)
        {
            // this enable CAT client logging cat message without explicit setup
            if (!_mManager.HasContext(messageTreeId))
            {
                _mManager.Setup(messageTreeId);
            }

            if (_mManager.CatEnabled(messageTreeId))
            {
                ITransaction transaction = new DefaultTransaction(type, name, _mManager.End);

                _mManager.Start(transaction, messageTreeId);
                return transaction;
            }
            return new NullTransaction();
        }

        /// <summary>
        /// new metric
        /// </summary>
        /// <param name="type">group</param>
        /// <param name="name">key</param>
        public virtual IMetric NewMetric(String type, String name, string messageTreeId)
        {
            // this enable CAT client logging cat message without explicit setup
            if (!_mManager.HasContext(messageTreeId))
            {
                _mManager.Setup(messageTreeId);
            }

            if (_mManager.CatEnabled(messageTreeId))
            {
                IMetric metric = new DefaultMetric(string.IsNullOrWhiteSpace(type) ? string.Empty : type, name);

                _mManager.Add(metric, messageTreeId);
                return metric;
            }
            return new NullMetric();
        }

        #endregion IMessageProducer Members
    }
}