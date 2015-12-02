namespace CAT.Log
{
    public interface ILog
    {
        void Info(string pattern, params object[] args);

        void Warn(string pattern, params object[] args);

        void Error(string pattern, params object[] args);
    }
}