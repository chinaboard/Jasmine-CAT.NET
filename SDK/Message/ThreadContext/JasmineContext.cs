using CAT.Context;
using CAT.Message.Internals;
using CAT.Message.Spi.Internals;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;

namespace CAT.Message.ThreadContext
{
    internal class JasmineContext
    {
        private readonly Stack<ITransaction> _mStack;
        private readonly IMessageTree _mTree;

        public JasmineContext(String domain, String hostName, String ipAddress)
        {
            _mTree = new DefaultMessageTree();
            _mStack = new Stack<ITransaction>();

            Thread thread = Thread.CurrentThread;
            String groupName = Thread.GetDomain().FriendlyName;

            _mTree.ThreadGroupName = groupName;
            _mTree.ThreadId = thread.ManagedThreadId.ToString(CultureInfo.InvariantCulture);
            _mTree.ThreadName = thread.Name;

            _mTree.Domain = domain;
            _mTree.HostName = hostName;
            _mTree.IpAddress = ipAddress;
        }

        public IMessageTree Tree
        {
            get { return _mTree; }
        }

        /// <summary>
        ///   添加Event和Heartbeat
        /// </summary>
        /// <param name="manager"> </param>
        /// <param name="message"> </param>
        public void Add(DefaultMessageManager manager, IMessage message)
        {
            if ((_mStack.Count == 0))
            {
                IMessageTree tree = _mTree.Copy();
                tree.MessageId = manager.NextMessageId();
                tree.Message = message;
                manager.Flush(tree);
            }
            else
            {
                ITransaction entry = _mStack.Peek();
                entry.AddChild(message);
            }
        }

        ///<summary>
        ///  return true means the transaction has been flushed.
        ///</summary>
        ///<param name="manager"> </param>
        ///<param name="transaction"> </param>
        ///<returns> true if message is flushed, false otherwise </returns>
        public bool End(DefaultMessageManager manager, ITransaction transaction)
        {
            if (_mStack.Count != 0)
            {
                ITransaction current = _mStack.Pop();
                while (transaction != current && _mStack.Count != 0)
                {
                    current = _mStack.Pop();
                }
                if (transaction != current)
                    throw new Exception("没找到对应的Transaction.");

                if (_mStack.Count == 0)
                {
                    ValidateTransaction(current);

                    IMessageTree tree = _mTree.Copy();
                    _mTree.MessageId = null;
                    _mTree.Message = null;
                    manager.Flush(tree);
                    return true;
                }
                return false;
            }
            throw new Exception("Stack为空, 没找到对应的Transaction.");
        }

        /// <summary>
        ///   返回stack的顶部对象
        /// </summary>
        /// <returns> </returns>
        public ITransaction PeekTransaction()
        {
            return (_mStack.Count == 0) ? new NullTransaction() : _mStack.Peek();
        }

        /// <summary>
        ///   添加transaction
        /// </summary>
        /// <param name="manager"> </param>
        /// <param name="transaction"> </param>
        public void Start(DefaultMessageManager manager, ITransaction transaction)
        {
            if (_mStack.Count != 0)
            {
                transaction.Standalone = false;
                ITransaction entry = _mStack.Peek();
                entry.AddChild(transaction);
            }
            else
            {
                _mTree.MessageId = manager.NextMessageId();
                _mTree.Message = transaction;
            }

            _mStack.Push(transaction);
        }

        //验证Transaction
        internal void ValidateTransaction(ITransaction transaction)
        {
            IList<IMessage> children = transaction.Children;
            int len = children.Count;
            for (int i = 0; i < len; i++)
            {
                IMessage message = children[i];
                var transaction1 = message as ITransaction;
                if (transaction1 != null)
                {
                    ValidateTransaction(transaction1);
                }
            }

            if (!transaction.IsCompleted())
            {
                // missing transaction end, log a BadInstrument event so that
                // developer can fix the code
                IMessage notCompleteEvent = new DefaultEvent("CAT", "BadInstrument") { Status = "TransactionNotCompleted" };
                notCompleteEvent.Complete(CallContextManager.MessageTreeId);
                transaction.AddChild(notCompleteEvent);
                transaction.Complete(CallContextManager.MessageTreeId);
            }
        }
    }
}