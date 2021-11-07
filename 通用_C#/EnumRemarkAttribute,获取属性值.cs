using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;


/// <summary>
/// http://blog.csdn.net/qq873113580/article/details/48464481
/// </summary>
namespace Zemp.Process.Util.EnumAttribute
{
    //首先自定义一个RemarkAttribute
    public class RemarkAttribute : Attribute
    {
        private string _remark;
        public RemarkAttribute(string _remark)
        {
            this._remark = _remark;
        }
        public string Remark
        {
            get { return _remark; }
            set { _remark = value; }
        }

        public static string GetEnumRemark(System.Enum _enum)
        {

            Type type = _enum.GetType();
            FieldInfo fd = type.GetField(_enum.ToString());
            if (fd == null) return string.Empty;
            object[] attrs = fd.GetCustomAttributes(typeof(RemarkAttribute), false);
            string name = string.Empty;
            foreach (RemarkAttribute attr in attrs)
            {
                name = attr.Remark;
            }
            return name;
        }


        //然后在Enum里面的使用
        public enum WorkflowTypeEnum
        {
            /// <summary>
            /// 草稿
            /// </summary>
            [Remark("草稿")]
            Draft = -1,
            /// <summary>
            /// 流转结束
            /// </summary>
            [Remark("流转结束")]
            Completed = 0,
            /// <summary>
            /// 用户取消
            /// </summary>
            [Remark("用户取消")]
            UserCancel = 1,
            /// <summary>
            /// 管理员取消
            /// </summary>
            [Remark("管理员取消")]
            AdminCancel = 2,
            /// <summary>
            /// 流转中 
            /// </summary>
            [Remark("流转中")]
            Running = 3,
            /// <summary>
            /// 流转中(曾被拒绝过)
            /// </summary>
            [Remark("流转中(曾被拒绝过)")]
            EverDeclined = 4,

            /// <summary>
            /// 未流转
            /// </summary>
            [Remark("未流转")]
            None = 5
        }

        //使用：得到Remark里面的值
        //RemarkAttribute.GetEnumRemark(WorkflowTypeEnum.None);
        //int 转 enum
        //（WorkflowTypeEnum）5;
        //enum转int
        //convert.into32(WorkflowTypeEnum.None);
    }
}