using CAT.Configuration;
using CAT.Message.Spi.HeartBeat;
using CAT.Message.Spi.HeartBeat.Extend;
using CAT.Util;
using System.Text;
using System.Threading;

namespace CAT.Message.Spi.Internals
{
    public class StatusUpdateTask
    {
        private readonly Domain domainInfo = null;
        internal readonly NodeStatusInfo m_nodeInfo = null;

        public StatusUpdateTask(IMessageStatistics mStatistics, Domain domainInfo)
        {
            this.domainInfo = domainInfo;

            try
            {
                m_nodeInfo = new NodeStatusInfo(mStatistics);
                m_nodeInfo.HeartBeatExtensions.Add(new CpuInfo());
                m_nodeInfo.HeartBeatExtensions.Add(new NetworkIO());
                m_nodeInfo.HeartBeatExtensions.Add(new DiskIO());
                m_nodeInfo.Refresh();
                m_nodeInfo.HaveAcessRight = true;
            }
            catch
            {
            }
        }

        public void Run(object o)
        {
            while (true)
            {
                if (!m_nodeInfo.HaveAcessRight)
                    break;
                m_nodeInfo.Refresh();
                var messageTreeId = Jasmine.GetManager().GetMessageIdFactory().GetNextId();
                ITransaction t = Jasmine.GetProducer().NewTransaction("System", "Status", messageTreeId);
                IHeartbeat h = Jasmine.GetProducer().NewHeartbeat("Heartbeat", domainInfo.Ip, messageTreeId);
                var xml = XmlHelper.XmlSerialize(m_nodeInfo, Encoding.UTF8);
                h.AddData(xml);
                h.Status = "0";
                h.Complete(messageTreeId);
                t.Status = "0";
                t.Complete(messageTreeId);

                Thread.Sleep(this.domainInfo.TickTime);
            }
        }
    }
}