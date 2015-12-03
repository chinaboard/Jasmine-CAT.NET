using System;

namespace Cat.Context
{
    public abstract class ContextBase
    {
        public static readonly String ROOT = "_catRootMessageId";

        public static readonly String PARENT = "_catParentMessageId";

        public static readonly String CHILD = "_catChildMessageId";

        public abstract void AddProperty(String key, String value);

        public abstract String GetProperty(String key);
    }
}