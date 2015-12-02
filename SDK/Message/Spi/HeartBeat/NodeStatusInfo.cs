using CAT.Message.Spi.HeartBeat.Extend;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace CAT.Message.Spi.HeartBeat
{
    [XmlRoot("status")]
    public class NodeStatusInfo : IRefresh, IXmlSerializable
    {
        internal bool HaveAcessRight = false;

        private XmlSerializerNamespaces ns = new XmlSerializerNamespaces();

        public NodeStatusInfo()
        {
        }

        public NodeStatusInfo(IMessageStatistics statistics)
        {
            RuntimeInfo = new RuntimeInfo();
            OSInfo = new OSInfo();
            DiskInfoList = new List<DiskInfo>();
            MemoryInfo = new MemoryInfo();
            ThreadInfo = new ThreadInfo();
            MessageInfo = new MessageInfo(statistics);
            HeartBeatExtensions = new List<HeartbeatExtention>();
            Timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
            ns.Add("", "");
        }

        [XmlAttribute("timestamp")]
        public string Timestamp { get; set; }

        public RuntimeInfo RuntimeInfo { get; set; }
        public OSInfo OSInfo { get; set; }

        public List<DiskInfo> DiskInfoList { get; set; }

        public MemoryInfo MemoryInfo { get; set; }

        public ThreadInfo ThreadInfo { get; set; }

        public MessageInfo MessageInfo { get; set; }

        public List<HeartbeatExtention> HeartBeatExtensions { get; set; }

        public void Refresh()
        {
            this.Timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
            this.RuntimeInfo.Refresh();
            this.OSInfo.Refresh();

            this.DiskInfoList.Clear();
            foreach (DriveInfo d in DriveInfo.GetDrives())
            {
                if (d.IsReady)
                {
                    this.DiskInfoList.Add(new DiskInfo()
                    {
                        Id = d.Name,
                        Free = d.AvailableFreeSpace,
                        Total = d.TotalSize,
                        Use = d.TotalSize - d.AvailableFreeSpace
                    });
                }
            }

            this.MemoryInfo.Refresh();
            this.ThreadInfo.Refresh();
            this.MessageInfo.Refresh();
            foreach (var item in HeartBeatExtensions)
            {
                item.Refresh();
            }
        }

        #region IXmlSerializable

        public System.Xml.Schema.XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(System.Xml.XmlReader reader)
        {
        }

        public void WriteXml(System.Xml.XmlWriter writer)
        {
            XmlSerializer serializer = null;

            writer.WriteAttributeString("timestamp", this.Timestamp);

            serializer = new XmlSerializer(typeof(RuntimeInfo));
            serializer.Serialize(writer, this.RuntimeInfo, ns);

            serializer = new XmlSerializer(typeof(OSInfo));
            serializer.Serialize(writer, this.OSInfo, ns);

            writer.WriteStartElement("disk");
            foreach (var item in DiskInfoList)
            {
                serializer = new XmlSerializer(typeof(DiskInfo));
                serializer.Serialize(writer, item, ns);
            }
            writer.WriteEndElement();

            serializer = new XmlSerializer(typeof(MemoryInfo));
            serializer.Serialize(writer, this.MemoryInfo, ns);

            serializer = new XmlSerializer(typeof(ThreadInfo));
            serializer.Serialize(writer, this.ThreadInfo, ns);

            serializer = new XmlSerializer(typeof(MessageInfo));
            serializer.Serialize(writer, this.MessageInfo, ns);

            foreach (var item in HeartBeatExtensions)
            {
                writer.WriteStartElement("extension");
                writer.WriteAttributeString("id", item.Id);
                foreach (var item2 in item.Dict)
                {
                    writer.WriteStartElement("extensionDetail");
                    writer.WriteAttributeString("id", item2.Key.ToString());
                    writer.WriteAttributeString("value", item2.Value.ToString());
                    writer.WriteEndElement();
                }
                writer.WriteEndElement();
            }
        }

        #endregion IXmlSerializable
    }
}