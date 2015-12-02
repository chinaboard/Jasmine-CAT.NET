using System;

namespace CAT.Context
{
    public class CallContextManager
    {
        internal static Func<string> TreeIdGeneratorFunc;

        public static void RegisterTreeIdGeneratorFunc(Func<string> treeIdGeneratorFunc)
        {
            TreeIdGeneratorFunc = treeIdGeneratorFunc;
        }

        public static string MessageTreeId
        {
            get
            {
                if (TreeIdGeneratorFunc == null)
                    TreeIdGeneratorFunc = DefaultCallContext.GetMessageId;
                return TreeIdGeneratorFunc();
            }
        }
    }
}