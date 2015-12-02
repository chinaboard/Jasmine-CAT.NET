using System.Threading;

namespace CAT.Context
{
    public class DefaultCallContext
    {
        public static string GetMessageId()
        {
            return Thread.CurrentThread.ManagedThreadId.ToString();
        }
    }
}