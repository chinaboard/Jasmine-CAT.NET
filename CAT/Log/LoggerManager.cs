namespace CAT.Log
{
    internal class LoggerManager
    {
        private static ILog Logger;

        public static void RegisterLogger(ILog logger)
        {
            Logger = logger;
        }

        public static void Info(string pattern, params object[] args)
        {
            if (Logger != null)
                Logger.Info(pattern, args);
        }

        public static void Warn(string pattern, params object[] args)
        {
            if (Logger != null)
                Logger.Warn(pattern, args);
        }

        public static void Error(string pattern, params object[] args)
        {
            if (Logger != null)
                Logger.Error(pattern, args);
        }
    }
}