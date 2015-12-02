using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Xml.Serialization;

namespace CAT.Message.Spi.HeartBeat
{
    [XmlRoot("memory")]
    public class MemoryInfo : IRefresh, IXmlSerializable
    {
        private int[] gcCounts = new int[GC.MaxGeneration + 1];

        public MemoryInfo()
        {
            GCInfoList = new List<GCInfo>();
            GCInfoList.Add(new GCInfo());
            for (int i = 0; i <= GC.MaxGeneration; i++)
            {
                gcCounts[i] = GC.CollectionCount(i);
            }
        }

        [XmlAttribute("max")]
        public ulong Max { get; set; }

        [XmlAttribute("total")]
        public long Total { get; set; }

        [XmlAttribute("free")]
        public long Free { get; set; }

        [XmlAttribute("heap-usage")]
        public long HeapUse { get; set; }

        [XmlAttribute("non-heap-usage")]
        public long HeapUnUse { get; set; }

        [XmlElement("gc")]
        public List<GCInfo> GCInfoList { get; set; }

        public void Refresh()
        {
            var p = Process.GetCurrentProcess();
            this.Total = p.PrivateMemorySize64;
            this.HeapUse = GC.GetTotalMemory(false);
            this.HeapUnUse = this.Total - this.HeapUse;

            this.GCInfoList.Clear();
            for (int i = 0; i <= GC.MaxGeneration; i++)
            {
                var gcInfo = new GCInfo();
                gcInfo.Name = "Gen_" + i.ToString();
                var time = GC.CollectionCount(i);
                gcInfo.Count = time - gcCounts[i];
                gcInfo.Time = time;
                gcCounts[i] = time;
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
            writer.WriteAttributeString("max", Max.ToString());
            writer.WriteAttributeString("total", Total.ToString());
            writer.WriteAttributeString("free", Free.ToString());
            writer.WriteAttributeString("heap-usage", HeapUse.ToString());
            writer.WriteAttributeString("non-heap-usage", HeapUnUse.ToString());
            foreach (var item in GCInfoList)
            {
                writer.WriteStartElement("gc");
                if (!string.IsNullOrEmpty(item.Name))
                    writer.WriteAttributeString("name", item.Name.ToString());
                writer.WriteAttributeString("count", item.Count.ToString());
                writer.WriteAttributeString("time", item.Time.ToString());
                writer.WriteEndElement();
            }
        }

        #endregion IXmlSerializable
    }
}