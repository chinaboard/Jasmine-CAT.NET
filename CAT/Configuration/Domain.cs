using CAT.Util;
using System.Xml.Serialization;

namespace CAT.Configuration
{
    /// <summary>
    ///   描述当前系统的情况
    /// </summary>
    public class Domain
    {
        private string _id = "Unknown";
        private bool _mEnabled = false;

        /// <summary>
        ///   当前系统的标识
        /// </summary>
        [XmlAttribute("id")]
        public string Id
        {
            get { return _id; }
            set { _id = value; }
        }

        private string ip;

        /// <summary>
        ///   当前系统的IP
        /// </summary>
        [XmlAttribute("ip")]
        public string Ip
        {
            get
            {
                if (string.IsNullOrEmpty(ip))
                    ip = NetworkUtil.IP;
                return ip;
            }
            set
            {
                ip = value;
            }
        }

        /// <summary>
        ///   Cat日志是否开启，默认开启
        /// </summary>
        [XmlAttribute("enabled")]
        public bool Enabled//无论如何不关闭
        {
            get { return true; }
            set { _mEnabled = value; }
        }

        [XmlAttribute("heartbeat")]
        public bool HeartBeatEnabled { get; set; }

        [XmlAttribute("ticktime")]
        public int TickTime = 60 * 1000;
    }
}