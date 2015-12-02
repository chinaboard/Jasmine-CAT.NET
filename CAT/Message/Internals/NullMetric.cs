namespace CAT.Message.Internals
{
    public class NullMetric : AbstractMessage, IMetric
    {
        public NullMetric()
            : base(null, null)
        {
        }
    }
}