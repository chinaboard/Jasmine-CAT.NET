using System;

namespace CAT.Message.Internals
{
    public class DefaultMetric : AbstractMessage, IMetric
    {
        public DefaultMetric(String type, String name)
            : base(type, name)
        {
        }
    }
}