using System.Collections.Generic;
using System.Xml.Serialization;

namespace CAT.Message.Spi.HeartBeat.Extend
{
    public abstract class HeartbeatExtention : IRefresh
    {
        public abstract Dictionary<string, double> Dict { get; }

        [XmlAttribute("id")]
        public abstract string Id { get; }

        public abstract void Refresh();
    }
}