using CAT.Configuration;
using CAT.Message.Spi.HeartBeat.Extend;
using CAT.Message.Spi.Internals;

namespace CAT.Message.Spi
{
    ///<summary>
    ///  Message manager to help build CAT message. <p>Notes: This method is reserved for internal usage only. Application developer
    ///                                               should never call this method directly.</p>
    ///</summary>
    public interface IMessageManager
    {
        ///<summary>
        ///  Return configuration for CAT client.
        ///</summary>
        ///<value> CAT configuration </value>
        JasmineSetting ClientConfig { get; }

        //TransportManager TransportManager { get; }

        ///<summary>
        ///  Get peek transaction for current thread.
        ///</summary>
        ///<value> peek transaction for current thread, null if no transaction there. </value>
        ITransaction PeekTransaction(string messageTreeId);

        ///<summary>
        ///  Get thread local message information.
        ///</summary>
        ///<value> message tree, null means current thread is not setup correctly. </value>
        IMessageTree GetMessageTree(string messageTreeId);

        ///<summary>
        ///  Check if CAT logging is enabled or disabled.
        ///</summary>
        ///<value> true if CAT is enabled </value>
        bool CatEnabled(string messageTreeId);

        /// <summary>
        ///   用于添加Event或者Heartbeat到peek transaction或者到根 如果是添加到根，建议直接使用IMessageProducer中的LogError、LogEvent或LogHeartbeat方法
        /// </summary>
        /// <param name="message"> </param>
        void Add(IMessage message, string MessageTreeId);

        ///<summary>
        ///  Initialize CAT client with given CAT configuration.
        ///</summary>
        ///<param name="config"> CAT configuration </param>
        void InitializeClient(JasmineSetting config);

        ///<summary>
        ///  Do cleanup for current thread environment in order to release resources in thread local objects.
        ///</summary>
        void Reset(string MessageTreeId);

        ///<summary>
        ///  Check if the thread context is setup or not.
        ///</summary>
        ///<returns> true if the thread context is setup, false otherwise </returns>
        bool HasContext(string MessageTreeId);

        ///<summary>
        ///  Do setup for current thread environment in order to prepare thread local objects.
        ///</summary>
        void Setup(string messageTreeId);

        ///<summary>
        ///  Be triggered when a new transaction starts, whatever it's the root transaction or nested transaction.
        ///</summary>
        ///<param name="transaction"> </param>
        void Start(ITransaction transaction, string MessageTreeId);

        ///<summary>
        ///  Be triggered when a transaction ends, whatever it's the root transaction or nested transaction. However, if it's the root transaction then it will be flushed to back-end CAT server asynchronously.
        ///</summary>
        ///<param name="transaction"> </param>
        void End(ITransaction transaction, string MessageTreeId);

        MessageIdFactory GetMessageIdFactory();

        void Register(HeartbeatExtention extension);
    }
}