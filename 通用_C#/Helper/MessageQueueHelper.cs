using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Messaging;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Common
{
    /// <summary>
    /// Message Queue helper class
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class MessageQueueHelper<T> : MarshalByRefObject, IDisposable where T : class, new()
    {
        public event ReceiveEventHandler ReceiveEvent;
        public delegate void ReceiveEventHandler(object sender, ReceiveEventArgs<T> arg);
        private bool enableReceive = false;

        /// <summary>
        /// 消息队列
        /// </summary>
        private MessageQueue _MQ;

        /// <summary>
        /// 是否允许抛出异常
        /// </summary>
        public bool AllowException
        {
            get;
            private set;
        }

        private bool MssageQueueIsReady
        {
            get
            {
                if (_MQ == null)
                    if (AllowException)
                        throw new Exception("The message queue is not ready.");
                    else
                        return false;
                else
                    return true;
            }
        }

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="path"></param>
        public MessageQueueHelper(string path)
        {
            AllowException = true;
            try
            {
                _MQ = new MessageQueue(path);
                //m_Msq.SetPermissions("Everyone", System.Messaging.MessageQueueAccessRights.FullControl);
                _MQ.Formatter = new ZipBinaryFormatter();
                _MQ.Label = typeof(T).Name;
                _MQ.ReceiveCompleted += new ReceiveCompletedEventHandler(Msq_ReceiveCompleted);
            }
            catch (Exception ex)
            {
                //TODO 日志
                //CommonLoger.WriteLog(SourceType.BusinessService, LogType.ExceptionInfo, "MessageQueueHelper instance exception:" + ex.Message);
            }
        }

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="path"></param>
        public MessageQueueHelper(string path, bool isSimple)
        {
            AllowException = true;
            try
            {
                _MQ = new MessageQueue(path);
                _MQ.Formatter = new ZipBinaryFormatter();

            }
            catch (Exception ex)
            {
                //TODO 日志
                //CommonLoger.WriteLog(SourceType.BusinessService, LogType.ExceptionInfo, "MessageQueueHelper instance exception:" + ex.Message);
            }
        }
        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="path"></param>
        /// <param name="client"></param>
        public MessageQueueHelper(string path, int client)
        {
            AllowException = true;
            try
            {
                _MQ = new MessageQueue(path);
                _MQ.Formatter = new ZipBinaryFormatter();
            }
            catch (Exception ex)
            {
                //TODO 日志
                //CommonLoger.WriteLog(SourceType.BusinessService, LogType.ExceptionInfo, "MessageQueueHelper instance exception:" + ex.Message);
            }
        }


        /// <summary>
        /// 发送message到队列
        /// </summary>
        /// <param name="data"></param>
        public bool Send(T data)
        {
            bool result = false;

            if (!MssageQueueIsReady) return result;

            try
            {
                Message message = new Message(data);
                message.Formatter = new ZipBinaryFormatter();
                message.Recoverable = true;
                _MQ.Send(message);

                result = true;
            }
            catch (Exception ex)
            {
                //TODO 日志
                //WriteLog(obj, Common.Action.Add, "orderid:" + intt);
                throw new Exception("写队列异常", ex);
            }

            return result;
        }

        /// <summary>
        /// 发送message到队列
        /// </summary>
        /// <param name="data"></param>
        /// <param name="label"></param>
        public void Send(T data, string label)
        {
            if (!MssageQueueIsReady) return;
            try
            {
                Message message = new Message(data);
                message.Formatter = new ZipBinaryFormatter();
                message.Label = label;
                message.Recoverable = true;
                _MQ.Send(data, label);
            }
            catch (Exception ex)
            {
                //TODO 日志
                //CommonLoger.WriteLog(SourceType.BusinessService, LogType.ExceptionInfo, "写队列异常:" + ex.Message);
                throw new Exception("写队列异常", ex);
            }
        }

        /// <summary>
        /// 收message
        /// </summary>
        /// <returns></returns>
        public T Receive()
        {
            if (!MssageQueueIsReady) return default(T);
            Message m = _MQ.Receive(MessageQueueTransactionType.Single);
            //Message m = m_Msq.Receive(new TimeSpan(0, 0, int.Parse(ConfigurationManager.AppSettings[PublicConst.MSMQ_TIMEOUT])), MessageQueueTransactionType.Single);
            return m.Body as T;
        }
        /// <summary>
        /// 取得队列里所有message
        /// </summary>
        /// <returns></returns>
        public IList<T> ReceiveAll()
        {
            if (!MssageQueueIsReady) return null;

            Message[] ms = _MQ.GetAllMessages();
            _MQ.Purge();

            IList<T> list = new List<T>();
            foreach (Message m in ms)
            {
                m.Formatter = new ZipBinaryFormatter();// BinaryMessageFormatter();
                list.Add(m.Body as T);
            }
            return list;
        }

        /// <summary>
        /// 查看第一条消息
        /// </summary>
        /// <returns></returns>
        public T Peek()
        {
            if (!MssageQueueIsReady) return default(T);
            //m_Msq.Formatter = new XmlMessageFormatter(new Type[] { typeof(T) });
            Message m = _MQ.Peek();
            m.Formatter = new ZipBinaryFormatter();
            return m.Body as T;
        }

        /// <summary>
        /// 异步接收消息
        /// </summary>
        public void BeginReceive()
        {
            if (!MssageQueueIsReady) return;
            enableReceive = true;
            _MQ.BeginReceive();
        }

        /// <summary>
        /// 结束接收消息
        /// </summary>
        public void EndReceive()
        {
            enableReceive = false;
            if (!MssageQueueIsReady) return;
        }

        private void Msq_ReceiveCompleted(object sender, ReceiveCompletedEventArgs e)
        {
            MessageQueue mq = (MessageQueue)sender;
            Message m = mq.EndReceive(e.AsyncResult);
            m.Formatter = new ZipBinaryFormatter();
            if (ReceiveEvent != null)
                ReceiveEvent(this, new ReceiveEventArgs<T>(m.Body as T));
            if (enableReceive)
                mq.BeginReceive();
        }


        public void Dispose()
        {
            if (_MQ != null)
                _MQ.Close();
        }
    }
    /// <summary>
    /// 消息接受事件参数
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ReceiveEventArgs<T> : EventArgs where T : class
    {
        public ReceiveEventArgs(T result)
        {
            Result = result;
        }

        /// <summary>
        /// 结果
        /// </summary>
        public T Result
        {
            get;
            private set;
        }
    }

    public class ZipBinaryFormatter : IMessageFormatter
    {
        public ZipBinaryFormatter()
        {
        }

        public void Write(Message msg, object obj)
        {
            string ret = string.Empty;
            BinaryFormatter bf = new BinaryFormatter();

            MemoryStream ms = new MemoryStream();
            bf.Serialize(ms, obj);

            MemoryStream msZip = new MemoryStream();
            ICSharpCode.SharpZipLib.GZip.GZipOutputStream gZip = new ICSharpCode.SharpZipLib.GZip.GZipOutputStream(msZip);
            //System.IO.Compression.DeflateStream gZip = new System.IO.Compression.DeflateStream(msZip, System.IO.Compression.CompressionMode.Compress);
            gZip.SetLevel(9);
            byte[] aryByte = ms.ToArray();
            gZip.Write(aryByte, 0, aryByte.Length);

            gZip.Flush();
            gZip.Close();
            ms.Close();
            ms = new MemoryStream();
            aryByte = msZip.ToArray();
            ms.Write(aryByte, 0, aryByte.Length);

            msg.BodyStream = ms;

            msZip.Close();
            aryByte = null;
        }

        public object Read(Message msg)
        {
            object obj = null;
            BinaryFormatter bf = new BinaryFormatter();
            MemoryStream ms = new MemoryStream();

            msg.BodyStream.Seek(0, SeekOrigin.Begin);
            ICSharpCode.SharpZipLib.GZip.GZipInputStream gZip = new ICSharpCode.SharpZipLib.GZip.GZipInputStream(msg.BodyStream);

            byte[] bytData = new byte[4096];
            int size = 0;
            while ((size = gZip.Read(bytData, 0, bytData.Length)) > 0)
            {
                ms.Write(bytData, 0, size);
            }

            ms.Seek(0, SeekOrigin.Begin);

            obj = bf.Deserialize(ms);

            gZip.Close();
            ms.Close();
            bytData = null;

            return obj;
        }

        public bool CanRead(Message msg)
        {
            return true;
        }

        public object Clone()
        {
            return new ZipBinaryFormatter();
        }
    }
}
