using System.Net;

namespace CAT.Util
{
    public class NetworkUtil
    {
        private static string Ip = null;

        public static string IP
        {
            get
            {
                if (NetworkUtil.Ip == null)
                {
                    var ipList = Dns.GetHostAddresses(Dns.GetHostName());
                    foreach (var item in ipList)
                    {
                        if (item.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                            NetworkUtil.Ip = item.ToString();
                    }
                }
                return NetworkUtil.Ip;
            }
        }
    }
}