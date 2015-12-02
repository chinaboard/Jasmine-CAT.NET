using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace CAT.Configuration
{
    /// <summary>
    ///   Cat客户端配置
    /// </summary>
    [XmlRoot("jasmineSetting")]
    public class JasmineSetting : IConfigurationSectionHandler
    {
        private List<Server> _mServers;
        private Domain _mDomain;

        public JasmineSetting()
        {
            _mServers = new List<Server>();
        }

        /// <summary>
        ///   是否是开发模式
        /// </summary>
        [XmlAttribute("devmode")]
        public bool DevMode { get; set; }

        public Domain Domain
        {
            get { return _mDomain ?? (_mDomain = new Domain()); }

            set { _mDomain = value; }
        }

        /// <summary>
        ///   Cat日志服务器，可以有多个
        /// </summary>
        public List<Server> Servers
        {
            get { return _mServers; }

            set { _mServers = value; }
        }

        public object Create(object parent, object configContext, System.Xml.XmlNode section)
        {
            var serialize = new XmlSerializer(typeof(JasmineSetting));
            var xDocument = XDocument.Parse(section.OuterXml, LoadOptions.SetBaseUri | LoadOptions.SetLineInfo);
            JasmineSetting t;
            using (var memoryStream = new MemoryStream())
            {
                xDocument.Save(memoryStream);
                memoryStream.Seek(0, SeekOrigin.Begin);
                t = (JasmineSetting)serialize.Deserialize(memoryStream);
            }

            return t;
        }
    }
}