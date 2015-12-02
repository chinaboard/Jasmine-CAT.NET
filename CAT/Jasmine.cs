using CAT.Configuration;
using CAT.Context;
using CAT.Log;
using CAT.Message;
using CAT.Message.Spi;
using CAT.Message.Spi.HeartBeat.Extend;
using CAT.Message.Spi.Internals;
using CAT.Util;
using System;
using System.Text;

namespace CAT
{
    public class Jasmine
    {
        private static readonly Jasmine Instance = new Jasmine();

        private bool m_initialized;

        private IMessageManager m_Manager;

        private IMessageProducer m_Producer;

        private JasmineSetting m_Setting;

        private Jasmine()
        {
        }

        public static IMessageManager GetManager()
        {
            return Instance.m_Manager;
        }

        public static IMessageProducer GetProducer()
        {
            return Instance.m_Producer;
        }

        public static JasmineSetting Setting
        {
            get
            {
                return Instance.m_Setting;
            }
        }

        public static bool IsInit()
        {
            return Instance.m_initialized;
        }

        #region Initialize

        [Obsolete]
        public static void Initialize(string configFilePath)
        {
            if (Instance.m_initialized)
            {
                LoggerManager.Warn("Cat can't initialize again with config file(%s), IGNORED!", configFilePath);
                return;
            }

            LoggerManager.Info("Initializing Cat .Net Client ...");

            DefaultMessageManager manager = new DefaultMessageManager();
            JasmineSetting clientConfig = XmlHelper.XmlDeserializeFromFile<JasmineSetting>(configFilePath, Encoding.UTF8);
            Instance.m_Setting = clientConfig;
            manager.InitializeClient(clientConfig);
            Instance.m_Producer = new DefaultMessageProducer(manager);
            Instance.m_Manager = manager;
            Instance.m_initialized = true;
            LoggerManager.Info("Cat .Net Client initialized.");
        }

        public static void Initialize(JasmineSetting config)
        {
            if (Instance.m_initialized)
            {
                LoggerManager.Warn("Cat can't initialize again , IGNORED!");
                return;
            }

            LoggerManager.Info("Initializing Cat .Net Client ...");
            Instance.m_Setting = config;

            DefaultMessageManager manager = new DefaultMessageManager();

            manager.InitializeClient(config);
            Instance.m_Producer = new DefaultMessageProducer(manager);
            Instance.m_Manager = manager;
            Instance.m_initialized = true;
            LoggerManager.Info("Cat .Net Client initialized.");
        }

        public static void EnableLocalLog()
        {
            LoggerManager.RegisterLogger(new DefaultLogger());
        }

        public static void RegisterHeartbeatExtention(HeartbeatExtention extension)
        {
            Instance.m_Manager.Register(extension);
        }

        internal static bool IsInitialized(string messageTreeId)
        {
            bool isInitialized = Instance.m_initialized;
            if (isInitialized && !Instance.m_Manager.HasContext(messageTreeId))
            {
                Instance.m_Manager.Setup(messageTreeId);
            }
            return isInitialized;
        }

        #endregion Initialize

        /// <summary>
        /// 注册一个日志接口 该接口会记录Cat客户端的一些日志，默认实现为写本地文件
        /// </summary>
        /// <param name="logger"></param>
        public static void RegisterLogger(ILog logger)
        {
            LoggerManager.RegisterLogger(logger);
        }

        /// <summary>
        /// 注册一个消息树ID生成的委托 该方法可以用更灵活的维度构建消息树
        /// 默认实现方式是每个线程一个消息树，该实现不支持异步
        /// Jasmine的插件为各个环境注册了不同的委托
        /// </summary>
        /// <param name="func"></param>
        public static void RegisterTreeIdGeneratorFunc(Func<string> func)
        {
            CallContextManager.RegisterTreeIdGeneratorFunc(func);
        }

        #region Log

        public static void Error(Exception ex)
        {
            Jasmine.GetProducer().LogError(ex, CallContextManager.MessageTreeId);
        }

        public static void Event(string type, string name, string status = "0", string nameValuePairs = null)
        {
            Jasmine.GetProducer().LogEvent(type, name, status, nameValuePairs, CallContextManager.MessageTreeId);
        }

        private static void Heartbeat(string type, string name, string status = "0", string nameValuePairs = null)
        {
            Jasmine.GetProducer().LogHeartbeat(type, name, status, nameValuePairs, CallContextManager.MessageTreeId);
        }

        public static void MetricForCount(string name, int quantity = 1)
        {
            LogMetricInternal(name, "C", quantity.ToString());
        }

        public static void MetricForDuration(string name, double value)
        {
            LogMetricInternal(name, "T", String.Format("{0:F}", value));
        }

        public static void MetricForSum(string name, double sum, int quantity)
        {
            LogMetricInternal(name, "S,C", String.Format("{0},{1:F}", quantity, sum));
        }

        private static void LogMetricInternal(string name, string status, string keyValuePairs = null)
        {
            Jasmine.GetProducer().LogMetric(name, status, keyValuePairs, CallContextManager.MessageTreeId);
        }

        #endregion Log

        #region New message

        public static IEvent NewEvent(string type, string name)
        {
            return Jasmine.GetProducer().NewEvent(type, name, CallContextManager.MessageTreeId);
        }

        public static ITransaction NewTransaction(string type, string name)
        {
            return Jasmine.GetProducer().NewTransaction(type, name, CallContextManager.MessageTreeId);
        }

        #endregion New message

        #region 消息追溯

        /// <summary>
        /// 消息树构建 由插件调用 应用不需要关心这个方法
        /// </summary>
        /// <returns></returns>
        public static RemotionCallContext logRemoteCallClient()
        {
            IMessageTree tree = Jasmine.GetManager().GetMessageTree(CallContextManager.MessageTreeId);
            String messageId = tree.MessageId;

            if (messageId == null)
            {
                messageId = Jasmine.GetProducer().CreateMessageId();
                tree.MessageId = messageId;
            }

            String childId = Jasmine.GetProducer().CreateMessageId();
            Jasmine.Event(JasmineConstants.TYPE_REMOTE_CALL, "", "0", childId);

            String root = tree.RootMessageId;

            if (root == null)
            {
                root = messageId;
            }

            return new RemotionCallContext()
            {
                ChildMessageId = childId,
                ParentMessageId = messageId,
                RootMessageId = root
            };

            //ctx.AddProperty(CatContext.ROOT, root);
            //ctx.AddProperty(CatContext.PARENT, messageId);
            //ctx.AddProperty(CatContext.CHILD, childId);
        }

        /// <summary>
        ///  消息树构建 由插件调用 应用不需要关心这个方法
        /// </summary>
        /// <param name="ctx"></param>
        public static void logRemoteCallServer(RemotionCallContext ctx)
        {
            IMessageTree tree = Jasmine.GetManager().GetMessageTree(CallContextManager.MessageTreeId);
            String childMessageId = ctx.ChildMessageId;
            String rootId = ctx.RootMessageId;
            String parentId = ctx.ParentMessageId;

            if (childMessageId != null)
            {
                tree.MessageId = childMessageId;
            }
            if (parentId != null)
            {
                tree.ParentMessageId = parentId;
            }
            if (rootId != null)
            {
                tree.RootMessageId = rootId;
            }

            String childId = Jasmine.GetProducer().CreateMessageId();
            Jasmine.Event(JasmineConstants.TYPE_REMOTE_CALL, "", "0", childId);
        }

        #endregion 消息追溯
    }
}