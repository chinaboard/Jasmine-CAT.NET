using System.Xml.Serialization;

namespace CAT.Message.Spi.HeartBeat
{
    [XmlRoot("gc")]
    public class GCInfo
    {
        [XmlAttribute("name")]
        public string Name { get; set; }

        [XmlAttribute("count")]
        public int Count { get; set; }

        [XmlAttribute("time")]
        public int Time { get; set; }
    }
}