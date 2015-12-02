using CAT.Configuration;
using CAT.Log;
using CAT.Message.Internals;
using CAT.Message.Spi.HeartBeat.Extend;
using CAT.Message.Spi.IO;
using CAT.Message.ThreadContext;
using CAT.ThreadContext;
using System;
using System.Net;
using System.Threading;

namespace CAT.Message.Spi.Internals
{
    [Serializable]
    internal class DefaultMessageManager : IMessageManager
    {
        // we don't use static modifier since MessageManager is a singleton in
        // production actually
        private readonly CatThreadLocal m_Context = new CatThreadLocal();

        private JasmineSetting m_ClientConfig;

        private MessageIdFactory m_Factory;

        private bool m_FirstMessage = true;
        private String m_HostName;

        private IMessageSender m_Sender;

        private IMessageStatistics m_Statistics;

        private StatusUpdateTask _mStatusUpdateTask;

        #region IMessageManager Members

        public virtual JasmineSetting ClientConfig
        {
            get { return m_ClientConfig; }
        }

        public virtual ITransaction PeekTransaction(string messageTreeId)
        {
            JasmineContext ctx = GetContext(messageTreeId);

            return ctx != null ? ctx.PeekTransaction() : new NullTransaction();
        }

        public virtual IMessageTree GetMessageTree(string messageTreeId)
        {
            JasmineContext ctx = m_Context[messageTreeId];

            return ctx != null ? ctx.Tree : null;
        }

        public virtual void Reset(string messageTreeId)
        {
            // destroy current thread local data
            m_Context.Dispose(messageTreeId);
        }

        public virtual void InitializeClient(JasmineSetting clientConfig)
        {
            m_ClientConfig = clientConfig ?? new JasmineSetting();

            m_HostName = Dns.GetHostName();

            m_Statistics = new DefaultMessageStatistics();
            m_Sender = new TcpMessageSender(m_ClientConfig, m_Statistics);
            m_Sender.Initialize();
            m_Factory = new MessageIdFactory();

            // initialize domain and ip address
            m_Factory.Initialize(m_ClientConfig.Domain.Id, m_ClientConfig.Domain.Ip);

            // start status update task
            if (clientConfig.Domain.Enabled && clientConfig.Domain.HeartBeatEnabled)
            {
                _mStatusUpdateTask = new StatusUpdateTask(m_Statistics, clientConfig.Domain);
                ThreadPool.QueueUserWorkItem(_mStatusUpdateTask.Run);
            }

            LoggerManager.Info("Thread(StatusUpdateTask) started.");
        }

        public virtual bool HasContext(string messageTreeId)
        {
            return m_Context[messageTreeId] != null;
        }

        public virtual bool CatEnabled(string messageTreeId)
        {
            return m_ClientConfig.Domain.Enabled && m_Context[messageTreeId] != null;
        }

        public virtual void Add(IMessage message, string messageTreeId)
        {
            JasmineContext ctx = GetContext(messageTreeId);

            if (ctx != null)
            {
                ctx.Add(this, message);
            }
            else
                LoggerManager.Warn("Context没取到");
        }

        public virtual void Setup(string messageTreeId)
        {
            JasmineContext ctx = new JasmineContext(m_ClientConfig.Domain.Id, m_HostName,
                                      m_ClientConfig.Domain.Ip);

            m_Context[messageTreeId] = ctx;
        }

        public virtual void Start(ITransaction transaction, string messageTreeId)
        {
            JasmineContext ctx = GetContext(messageTreeId);

            if (ctx != null)
            {
                ctx.Start(this, transaction);
            }
            else if (m_FirstMessage)
            {
                m_FirstMessage = false;
                LoggerManager.Info("CAT client is not enabled because it's not initialized yet");
            }
            else
                LoggerManager.Warn("Context没取到");
        }

        public virtual void End(ITransaction transaction, string messageTreeId)
        {
            JasmineContext ctx = GetContext(messageTreeId);

            if (ctx != null)
            {
                //if (!transaction.Standalone) return;
                if (ctx.End(this, transaction))
                {
                    m_Context.Dispose(messageTreeId);
                }
            }
            else
                LoggerManager.Warn("Context没取到");
        }

        #endregion IMessageManager Members

        public MessageIdFactory GetMessageIdFactory()
        {
            return m_Factory;
        }

        internal void Flush(IMessageTree tree)
        {
            if (m_Sender != null)
            {
                m_Sender.Send(tree);

                if (m_Statistics != null)
                {
                    m_Statistics.OnSending(tree);
                }
            }
        }

        internal JasmineContext GetContext(string messageTreeId)
        {
            if (Jasmine.IsInitialized(messageTreeId))
            {
                JasmineContext ctx = m_Context[messageTreeId];

                if (ctx != null)
                {
                    return ctx;
                }
                if (m_ClientConfig.DevMode)
                {
                    throw new Exception(
                        "Cat has not been initialized successfully, please call Cal.setup(...) first for each thread.");
                }
            }

            return null;
        }

        public String NextMessageId()
        {
            return m_Factory.GetNextId();
        }

        //internal bool ShouldThrottle(IMessageTree tree)
        //{
        //    return false;
        //}

        public void Register(HeartbeatExtention extension)
        {
            this._mStatusUpdateTask.m_nodeInfo.HeartBeatExtensions.Add(extension);
        }
    }
}