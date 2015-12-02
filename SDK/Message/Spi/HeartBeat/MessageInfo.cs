using System.Xml.Serialization;

namespace CAT.Message.Spi.HeartBeat
{
    [XmlRoot("message")]
    public class MessageInfo : IRefresh
    {
        private IMessageStatistics statistics;

        public MessageInfo()
        {
        }

        public MessageInfo(IMessageStatistics statistics)
        {
            this.statistics = statistics;
        }

        [XmlAttribute("produced")]
        public long Produced { get; set; }

        [XmlAttribute("overflowed")]
        public long Overflowed { get; set; }

        [XmlAttribute("bytes")]
        public long Bytes { get; set; }

        public void Refresh()
        {
            this.Produced = statistics.Produced;
            this.Overflowed = statistics.Overflowed;
            this.Bytes = statistics.Bytes;
            statistics.Reset();
        }
    }
}