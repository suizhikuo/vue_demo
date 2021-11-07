using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    /// <summary>
    /// 系统快照
    /// </summary>
    public class SnapshotHelper
    {
        /// <summary>
        /// 发送快照
        /// </summary>
        /// <param name="data">data为SnapshotMessage实体</param>
        public static void SendSnapshot(object data)
        {
            MessageQueueHelper<object> helper = new MessageQueueHelper<object>(AppSettings.SnapShot, 1);
            helper.Send(data);
        }
    }
}
