using System;
using System.Web;

namespace CAT.Util
{
    public static class AppEnv
    {
        // public static string IP { get { return getLocalIP(); } }

        //private static string getLocalIP()
        //{
        //    try
        //    {
        //        return NetworkInterfaceManager.GetLocalHostAddress();
        //    }
        //    catch
        //    {
        //        return string.Empty;
        //    }
        //}

        public static string GetClientIp(HttpRequest request)
        {
            if (request == null)
            {
                return string.Empty;
            }
            var serverVariables = request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            if (!string.IsNullOrEmpty(serverVariables))
            {
                var items = serverVariables.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                if (items.Length > 0)
                {
                    return items[0];
                }
            }
            return request.ServerVariables["REMOTE_ADDR"];
        }

        public static string GetRemoteIp(HttpRequest request)
        {
            if (request == null)
            {
                return string.Empty;
            }
            return request.ServerVariables["REMOTE_ADDR"];
        }
    }
}