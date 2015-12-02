using CAT.Configuration;
using CAT.Log;
using CAT.Message.Spi.Codec;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Threading;

namespace CAT.Message.Spi.IO
{
    public class TcpMessageSender : IMessageSender
    {
        private readonly JasmineSetting _mClientConfig;
        private readonly IMessageCodec _mCodec;
        private readonly Queue<IMessageTree> _mQueue;
        private readonly IMessageStatistics _mStatistics;
        private bool _mActive;
        private TcpClient _mActiveConnection;
        private int _mErrors;
        private readonly int MaxQueueCount = (int)Math.Pow(10, 5);//默认缓存10万消息
        private int m_LastActiveIndex = -1;
        private int m_ActiveIndex = -1;

        public TcpMessageSender(JasmineSetting clientConfig, IMessageStatistics statistics)
        {
            _mClientConfig = clientConfig;
            _mStatistics = statistics;
            _mActive = true;
            _mQueue = new Queue<IMessageTree>();
            _mCodec = new PlainTextMessageCodec();
        }

        #region IMessageSender Members

        public virtual bool HasSendingMessage
        {
            get { return _mQueue.Count > 0; }
        }

        public void Initialize()
        {
            TcpClient connection = RandomConnect(ref m_ActiveIndex);

            if (connection != null)
            {
                _mActiveConnection = connection;
            }

            ThreadPool.QueueUserWorkItem(ConnectionDeamonTask);
            ThreadPool.QueueUserWorkItem(AsynchronousSendTask);

            LoggerManager.Info("Thread(TcpMessageSender-ConnectionDeamonTask) started.");
            LoggerManager.Info("Thread(TcpMessageSender-AsynchronousSendTask) started.");
        }

        private TcpClient RandomConnect(ref int activeServerIndex)
        {
            var list = Enumerable.Range(0, _mClientConfig.Servers.Count).ToList();
            if (m_LastActiveIndex != -1)
                list.Remove(m_LastActiveIndex);
            Random rd = new Random();
            for (int i = 0; i < list.Count; )
            {
                var index = rd.Next(100) % list.Count;
                var tcpClient = CreateConnection(index);
                if (tcpClient != null)
                {
                    activeServerIndex = index;
                    return tcpClient;
                }
                list.RemoveAt(index);
            }
            return null;
        }

        public void Send(IMessageTree tree)
        {
            lock (_mQueue)
            {
                if (_mQueue.Count < MaxQueueCount)
                {
                    _mQueue.Enqueue(tree);
                }
                else
                {
                    // throw it away since the queue is full
                    _mErrors++;

                    if (_mStatistics != null)
                    {
                        _mStatistics.OnOverflowed(tree);
                    }

                    if (_mErrors % 100 == 0)
                    {
                        LoggerManager.Warn("Can't send message to cat-server due to queue's full! Count: " + _mErrors);
                    }
                }
            }
        }

        public void Shutdown()
        {
            _mActive = false;

            try
            {
                if (_mActiveConnection != null && _mActiveConnection.Connected)
                {
                    _mActiveConnection.Close();
                }
            }
            catch
            {
                // ignore it
            }
        }

        #endregion IMessageSender Members

        public void ConnectionDeamonTask(object o)
        {
            while (true)
            {
                if (_mActive)
                {
                    if (_mActiveConnection == null || !_mActiveConnection.Connected)
                    {
                        if (_mActiveConnection != null)
                            LoggerManager.Warn("ChannelManagementTask中，Socket关闭");

                        m_LastActiveIndex = m_ActiveIndex;
                        TcpClient connnection = RandomConnect(ref m_ActiveIndex);

                        if (connnection != null)
                        {
                            var _mLastConnection = _mActiveConnection;
                            _mActiveConnection = connnection;
                            if (_mLastConnection != null)
                            {
                                try
                                {
                                    LoggerManager.Warn("SendInternal中，_mLastChannel关闭");
                                    _mLastConnection.Close();
                                }
                                catch
                                {
                                    // ignore it
                                }

                                _mLastConnection = null;
                            }
                        }
                    }
                }

                Thread.Sleep(2 * 1000); // every 2 seconds
            }
        }

        public void AsynchronousSendTask(object o)
        {
            IMessageTree tree = null;
            while (_mActive)
            {
                while (_mQueue.Count == 0 || _mActiveConnection == null || !_mActiveConnection.Connected)
                {
                    if (_mActiveConnection != null && !_mActiveConnection.Connected)
                        LoggerManager.Warn("AsynchronousSendTask中，Socket关闭");
                    Thread.Sleep(5);
                }
                lock (_mQueue)
                {
                    tree = _mQueue.Dequeue();
                }
                try
                {
                    SendInternal(tree);
                }
                catch (Exception t)
                {
                    LoggerManager.Error("Error when sending message over TCP socket! Error: {0}", t);
                }
            }
        }

        private void SendInternal(IMessageTree tree)
        {
            System.IO.MemoryStream ms = new System.IO.MemoryStream();

            if (_mActiveConnection != null && _mActiveConnection.Connected)
            {
                ChannelBuffer buf = new ChannelBuffer(8192);

                _mCodec.Encode(tree, buf);

                byte[] data = buf.ToArray();

                _mActiveConnection.Client.Send(data);

                if (_mStatistics != null)
                {
                    _mStatistics.OnBytes(data.Length);
                }
            }
            else
            {
                LoggerManager.Warn("SendInternal中，Socket关闭");
            }
        }

        private TcpClient CreateConnection(int index)
        {
            Server server = _mClientConfig.Servers[index];

            if (!server.Enabled)
            {
                return null;
            }

            TcpClient tcpClient = new TcpClient();
            tcpClient.NoDelay = true;
            tcpClient.ReceiveTimeout = 5 * 1000; // 2 seconds

            string ip = server.Ip;
            int port = server.Port;

            LoggerManager.Info("Connecting to server({0}:{1}) ...", ip, port);

            try
            {
                tcpClient.Connect(ip, port);

                if (tcpClient.Connected)
                {
                    LoggerManager.Info("Connected to server({0}:{1}).", ip, port);

                    return tcpClient;
                }
                LoggerManager.Error("Failed to connect to server({0}:{1}).", ip, port);
            }
            catch (Exception e)
            {
                LoggerManager.Error(
                    "Failed to connect to server({0}:{1}). Error: {2}.",
                    ip,
                    port,
                    e.Message
                    );
            }

            return null;
        }
    }
}