using System;

namespace CAT.Message.Internals
{
    public class DefaultHeartbeat : AbstractMessage, IHeartbeat
    {
        public DefaultHeartbeat(String type, String name)
            : base(type, name)
        {
        }
    }
}