using System.Runtime.Remoting.Messaging;
using System.Web;

namespace CAT.Context
{
    public class HttpCallContext
    {
        public static string GetMessageId()
        {
            var context = HttpContext.Current;
            if (context != null)
                return context.Items[JasmineConstants.ContextKey].ToString();
            return CallContext.LogicalGetData(JasmineConstants.ContextKey).ToString();
        }
    }
}