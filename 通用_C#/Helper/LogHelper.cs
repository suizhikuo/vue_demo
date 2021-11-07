/* LogHelper.cs
*
* 功 能： LogHelper
* 类 名： LogHelper
*
* Ver    变更日期         负责人  变更内容
* ───────────────────────────────────
* V0.01  2013.12.1       金鑫    --
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Web;
using System.Text;
using System.Reflection;
using System.ComponentModel;
using System.Text.RegularExpressions;

using Microsoft.BSF.Common.Entity;
using Microsoft.BSF.Common;

using Common.Helper;

using Newtonsoft.Json;

namespace Common
{
    /// <summary>
    ///LogHelper 的摘要说明
    /// </summary>
    public class LogHelper
    {
        private static Regex regex = new Regex(@",?""\w+""\s*:\s*""0001-01-01\s00:00:00"" | """, RegexOptions.IgnoreCase | RegexOptions.Singleline | RegexOptions.Multiline | RegexOptions.IgnorePatternWhitespace);
        public LogHelper()
        {
            //
            //TODO: 在此处添加构造函数逻辑
            //
        }

        /// <summary>
        /// 获取需要记录日志的字段
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static Dictionary<string, LogField> GetLogField<T>(T obj) where T : class
        {
            Dictionary<string, LogField> dicLogFields = new Dictionary<string, LogField>();

            Type t = obj.GetType();

            PropertyInfo[] aryProperty = t.GetProperties();

            foreach (PropertyInfo pi in aryProperty)
            {
                //如果是值类型且是枚举类型直接取值
                if (pi.PropertyType.IsValueType || pi.PropertyType.IsEnum || pi.PropertyType == typeof(string))
                {
                    Object[] aryObj = pi.GetCustomAttributes(typeof(Common.DescriptionAttribute), true);

                    if (aryObj.Length > 0)
                    {
                        LogField lf = new LogField();
                        lf.FieldCaption = (aryObj.GetValue(0) as Common.DescriptionAttribute).Description;
                        lf.FieldName = pi.Name;
                        Object pobj = pi.GetValue(obj, null);
                        lf.FieldValue = pobj == null ? "" : pobj.ToString();
                        dicLogFields.Add(lf.FieldName, lf);
                    }
                }
            }
            return dicLogFields;
        }

        /// <summary>
        /// 针对于GlobCommon中的实体取值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static Dictionary<string, LogField> GetLogFieldEx<T>(T obj) where T : class
        {
            Dictionary<string, LogField> dicLogFields = new Dictionary<string, LogField>();

            Type t = obj.GetType();

            PropertyInfo[] aryProperty = t.GetProperties();

            foreach (PropertyInfo pi in aryProperty)
            {
                //如果是值类型且是枚举类型直接取值
                if (pi.PropertyType.IsValueType || pi.PropertyType.IsEnum || pi.PropertyType == typeof(string))
                {
                    Object[] aryObj = pi.GetCustomAttributes(typeof(Common.DescriptionAttribute), true);

                    if (aryObj.Length > 0)
                    {
                        LogField lf = new LogField();
                        lf.FieldCaption = (aryObj.GetValue(0) as Common.DescriptionAttribute).Description;
                        lf.FieldName = pi.Name;
                        Object pobj = pi.GetValue(obj, null);
                        lf.FieldValue = pobj == null ? "" : pobj.ToString();
                        dicLogFields.Add(lf.FieldName, lf);
                    }
                }
            }
            return dicLogFields;
        }
        /// <summary>
        /// 获取变化的值
        /// </summary>
        /// <param name="dic1"></param>
        /// <param name="dic2"></param>
        /// <returns></returns>
        public static string GetChangeValue(Dictionary<string, LogField> dic1, Dictionary<string, LogField> dic2)
        {
            StringBuilder sb = new StringBuilder();
            if (dic1.Count != dic2.Count)
            {
                throw new Exception("比较对象数量不相等！");
            }

            foreach (string t in dic1.Keys)
            {
                if (dic2.ContainsKey(t))
                {
                    if (dic1[t].FieldValue != dic2[t].FieldValue)
                    {
                        sb.Append(dic1[t].FieldCaption + "的值由 【" + dic1[t].FieldValue.ToString() + "】修改为【" + dic2[t].FieldValue.ToString() + "】");
                    }
                }
            }

            return sb.ToString();
        }

        public static string GetChangeValue<T>(T originObj, Dictionary<string, LogField> dic1, Dictionary<string, LogField> dic2) where T : class
        {
            StringBuilder sb = new StringBuilder();
            if (dic1.Count != dic2.Count)
            {
                throw new Exception("比较对象数量不相等！");
            }
            if (originObj == default(T))
            {
                throw new Exception("传入对象为空");
            }
            //获取对象的主描述字段
            Type type = originObj.GetType();
            //遍历属性中描述的主描述信息，可以为多个，用逗号分割
            StringBuilder masterDesc = new StringBuilder();
            foreach (PropertyInfo pi in type.GetProperties())
            {
                object[] objs = pi.GetCustomAttributes(typeof(Common.MasterDescriptionAttribute), false);
                if (objs != null && objs.Length > 0)
                {
                    foreach (object obj in objs)
                    {
                        Common.MasterDescriptionAttribute desc = obj as Common.MasterDescriptionAttribute;
                        if (desc != null)
                        {
                            if (!String.IsNullOrEmpty(desc.MasterDesciption))
                            {
                                masterDesc.AppendFormat("{0}为{1},修改内容:", desc.MasterDesciption, pi.GetValue(originObj, null));
                            }
                        }
                    }
                }
            }


            foreach (string t in dic1.Keys)
            {
                if (dic2.ContainsKey(t))
                {
                    if (dic1[t].FieldValue != dic2[t].FieldValue)
                    {
                        sb.Append(dic1[t].FieldCaption + "的值由 【" + dic1[t].FieldValue.ToString() + "】修改为【" + dic2[t].FieldValue.ToString() + "】");
                    }
                }
            }

            return masterDesc.ToString() + sb.ToString();
        }

        /// <summary>
        /// 发送错误日志
        /// </summary>
        /// <param name="log"></param>
        /// <param name="data"></param>
        public static void SendErrorLog(BSFErrorEnt log, object data)
        {
            if (null != data)
                log.ErrorDesc = GetJson(data);
            MessageQueueHelper<BSFErrorEnt> helper = new MessageQueueHelper<BSFErrorEnt>(AppSettings.ERRORLOG, 1);
            helper.Send(log);
        }
        /// <summary>
        /// 发送功能日志
        /// </summary>
        /// <param name="log"></param>
        /// <param name="data"></param>
        public static void SendFunctionLog(BSFFuncEnt log, object data)
        {
            if (null != data)
                log.Desc = GetJson(data);
            if (log.ZipFlag == global::Microsoft.BSF.Common.Enum.ZipStatusEnum.Zip)
            {
                if (ZipWrapper.NeedCompress(log.Desc))
                {
                    log.Desc = ZipWrapper.Compress(log.Desc);
                }
                else
                {
                    log.ZipFlag = global::Microsoft.BSF.Common.Enum.ZipStatusEnum.UnZip;
                }
            }
            MessageQueueHelper<BSFFuncEnt> helper = new MessageQueueHelper<BSFFuncEnt>(AppSettings.FUNCTIONLOG, 1);
            helper.Send(log);

        }

        public static string GetJson(object data)
        {
            JsonSerializerSettings jss = new JsonSerializerSettings();
            jss.NullValueHandling = NullValueHandling.Ignore;
            //jss.Formatting = Formatting.Indented;
            jss.DateFormatString = "yyyy-MM-dd HH:mm:ss";
            jss.Converters.Add(new Common.DataTableConverter());
            string str = JsonConvert.SerializeObject(data, jss);
            str = regex.Replace(str, "").Replace(":,", ":\"\",");
            return str;
        }

    }

    [Serializable]
    public class LogField
    {
        /// <summary>
        /// 字段名称
        /// </summary>
        public string FieldName { get; set; }

        /// <summary>
        /// 字段显示名称
        /// </summary>
        public string FieldCaption { get; set; }

        /// <summary>
        /// 字段值
        /// </summary>
        public string FieldValue { get; set; }

    }

}

