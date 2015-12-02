using System;
using System.Xml.Serialization;

namespace CAT.Message.Spi.HeartBeat
{
    [XmlRoot("runtime")]
    public class RuntimeInfo : IRefresh
    {
        public RuntimeInfo()
        {
            StartTime = DateTime.Now.Ticks;
        }

        [XmlAttribute("start-time")]
        public long StartTime { get; set; }

        [XmlAttribute("up-time")]
        public long UpTime { get; set; }

        [XmlAttribute("java-version")]
        public string NetVersion { get; set; }

        [XmlAttribute("user-name")]
        public string UserName { get; set; }

        [XmlElement("user-dir")]
        public string AppPath { get; set; }

        [XmlElement("java-classpath")]
        public string ClasPath { get; set; }

        public void Refresh()
        {
            this.UserName = Environment.UserName;
            this.AppPath = AppDomain.CurrentDomain.BaseDirectory;
            this.NetVersion = Environment.Version.ToString();
        }
    }
}