using System.Xml.Serialization;

namespace CAT.Configuration
{
    /// <summary>
    ///   描述记录当前系统日志的目标Cat服务器
    /// </summary>
    public class Server
    {
        private string _mIp;

        private int _mPort;

        public Server()
        {
        }

        public Server(string ip, int port)
        {
            _mIp = ip;
            _mPort = port;
            Enabled = true;
        }

        /// <summary>
        ///   Cat服务器IP
        /// </summary>
        [XmlAttribute("ip")]
        public string Ip
        {
            get { return _mIp; }
            set { _mIp = value; }
        }

        /// <summary>
        ///   Cat服务器端口
        /// </summary>
        [XmlAttribute("port")]
        public int Port
        {
            get { return _mPort; }
            set { _mPort = value; }
        }

        /// <summary>
        ///   Cat服务器是否有效，默认有效
        /// </summary>
        [XmlAttribute("enabled")]
        public bool Enabled { get; set; }
    }
}