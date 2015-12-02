using Microsoft.VisualBasic.Devices;
using System;
using System.Diagnostics;
using System.Xml.Serialization;

namespace CAT.Message.Spi.HeartBeat
{
    [XmlRoot("os")]
    public class OSInfo : IRefresh
    {
        private ComputerInfo m_computer = null;

        public OSInfo()
        {
            m_computer = new ComputerInfo();
        }

        [XmlAttribute("name")]
        public string Name { get; set; }

        [XmlAttribute("arch")]
        public string Arch { get; set; }

        [XmlAttribute("version")]
        public string Version { get; set; }

        [XmlAttribute("available-processors")]
        public int ProcessCount { get; set; }

        [XmlAttribute("system-load-average")]
        public float AvgLoad { get; set; }

        [XmlAttribute("process-time")]
        public long ProcessTime { get; set; }

        [XmlAttribute("total-physical-memory")]
        public ulong TotalMemory { get; set; }

        [XmlAttribute("free-physical-memory")]
        public ulong FreeMemory { get; set; }

        [XmlAttribute("committed-virtual-memory")]
        public ulong CommitedMemory { get; set; }

        [XmlAttribute("total-swap-space")]
        public ulong TotalSwapSpace { get; set; }

        [XmlAttribute("free-swap-space")]
        public ulong FreeSwapSpace { get; set; }

        public void Refresh()
        {
            this.Name = m_computer.OSFullName;
            this.Arch = m_computer.OSPlatform;
            this.Version = m_computer.OSVersion;
            this.ProcessCount = Environment.ProcessorCount;
            this.TotalMemory = m_computer.TotalPhysicalMemory;
            this.FreeMemory = m_computer.TotalPhysicalMemory - m_computer.AvailablePhysicalMemory;
            this.ProcessTime = Process.GetCurrentProcess().TotalProcessorTime.Ticks / TimeSpan.TicksPerSecond;
        }
    }
}