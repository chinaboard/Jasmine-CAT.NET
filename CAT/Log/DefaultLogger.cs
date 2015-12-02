using CAT.Util;
using System;
using System.IO;

namespace CAT.Log
{
    internal class DefaultLogger : ILog
    {
        private StreamWriter _mWriter;
        private string LogDirPath;
        private string LogFilePath;

        public DefaultLogger()
        {
            LogDirPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "CatClientLogDir");
            if (!Directory.Exists(LogDirPath))
                Directory.CreateDirectory(LogDirPath);
            LogFilePath = System.IO.Path.Combine(LogDirPath, "CatLog.txt");
            Initialize(LogFilePath);
        }

        public void Info(string pattern, params object[] args)
        {
            Log("INFO", pattern, args);
        }

        public void Warn(string pattern, params object[] args)
        {
            Log("WARN", pattern, args);
        }

        public void Error(string pattern, params object[] args)
        {
            Log("ERROR", pattern, args);
        }

        private void Log(string severity, string pattern, params object[] args)
        {
            string timestamp = new DateTime(MilliSecondTimer.CurrentTimeMicros() * 10L).ToString("yyyy-MM-dd HH:mm:ss.fff");
            string message = string.Format(pattern, args);
            string line = "[" + timestamp + "] [" + severity + "] " + message;

            if (_mWriter != null)
            {
                _mWriter.WriteLine(line);
                _mWriter.Flush();
            }
            else
            {
                Console.WriteLine(line);
            }
        }

        /// <summary>
        ///   初始化
        /// </summary>
        /// <param name="logFile"> </param>
        private void Initialize(string logFile)
        {
            try
            {
                _mWriter = new StreamWriter(logFile, true);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error when openning log file: " + e.Message + " " + e.StackTrace + ".");
            }
        }
    }
}