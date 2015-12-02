using CAT.Message.ThreadContext;
using System.Collections;

namespace CAT.ThreadContext
{
    internal class CatThreadLocal
    {
        private readonly Hashtable m_Values = new Hashtable();

        public JasmineContext this[string messageTreeId]
        {
            get
            {
                return m_Values[messageTreeId] as JasmineContext;
            }
            set
            {
                lock (m_Values.SyncRoot)
                {
                    m_Values[messageTreeId] = value;
                }
            }
        }

        public void Dispose(string messageTreeId)
        {
            m_Values.Remove(messageTreeId);
        }
    }
}