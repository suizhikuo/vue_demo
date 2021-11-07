/* JsonHelper.cs
*
* 功 能： JsonHelper
* 类 名： JsonHelper
*
* Ver    变更日期         负责人  变更内容
* ───────────────────────────────────
* V0.01  2013.12.1       金鑫    --
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Web.Script.Serialization;
using System.Text.RegularExpressions;
using System.Data;
using System.Text;
using System.Reflection;
using System.Collections;
using System.Data.Common;

using Lenovo.Business.CustomerManagement;

using Newtonsoft.Json;

namespace Common
{
    /// <summary>
    /// Json返回的数据的处理
    /// </summary>
    public class JsonResultData
    {
        /// <summary>
        /// JsonResultData 构造函数
        /// </summary>
        public JsonResultData()
        {
            Messages = new List<string>();
        }

        /// <summary>
        /// JsonResultData 构造函数
        /// </summary>
        /// <param name="isSuccess">是否成功</param>
        /// <param name="messages">成功与否信息列表</param>
        /// <param name="data">要返回给客户端的数据</param>
        public JsonResultData(bool isSuccess, object data, List<string> messages)
        {
            this.IsSuccess = isSuccess;
            this.Messages = messages;
            this.Data = data;
        }

        /// <summary>
        /// JsonResultData 构造函数
        /// </summary>
        /// <param name="isSuccess">是否成功</param>
        /// <param name="messages">成功与否信息</param>
        public JsonResultData(bool isSuccess, string message = "")
        {
            this.IsSuccess = isSuccess;
            this.Message = message;

        }

         

        /// <summary>
        /// JsonResultData 构造函数
        /// </summary>
        /// <param name="isSuccess">是否成功</param>
        /// <param name="messages">成功与否信息</param>
        /// <param name="data">要返回给客户端的数据</param>
        public JsonResultData(bool isSuccess, object data, string message = "")
        {
            this.IsSuccess = isSuccess;
            this.Message = message;
            this.Data = data;
        }

        /// <summary>
        /// 返回类型，用于js返回类型的判断
        /// </summary>
        public string Type { get { return this.GetType().Name.ToUpper(); } }

        /// <summary>
        /// 请求处理是否成功
        /// </summary>
        public bool IsSuccess { get; set; }

        /// <summary>
        /// 功与否信息提示
        /// </summary>
        public string Message { get; set; }

         

        /// <summary>
        /// 成功与否信息提示列表
        /// </summary>
        public List<string> Messages { get; set; }

        /// <summary>
        /// 要返回给客户端的数据
        /// </summary>
        public object Data { get; set; }

        /// <summary>
        /// 获取返回的JSON字符串
        /// </summary>
        /// <remarks>Data 如返回DataSet、DataTable </remarks>
        public string GetJosnString()
        {
            return this.ToJson();
        }
    }

    /// <summary>
    ///JsonHelper 的摘要说明
    /// </summary>
    public static class JsonHelper
    {
        /// <summary>
        /// Json数组字符串反序列为dynamic对象集合
        /// </summary>
        /// <param name="json">json数组字符串，例如："[{}]"或"[{},{}]"</param>
        /// <returns>dynamic对象集合（List<!--<dynamic>-->）</returns>
        public static List<dynamic> ToListDynamic(this string json)
        {
            List<dynamic> listJsonFormat = (List<dynamic>)JsonConvert.DeserializeObject(json, typeof(List<dynamic>));
            return listJsonFormat;
        }

        /// <summary>
        /// Json数组字符串反序列为T对象集合
        /// </summary>
        /// <param name="json">json数组字符串，例如："[{}]"或"[{},{}]"</param>
        /// <returns>dynamic对象集合（List<!--<T>-->）</returns>
        public static IList<T> GetEntities<T>(string json) where T : new()
        {
            List<T> listJsonFormat = (List<T>)JsonConvert.DeserializeObject(json, typeof(List<T>));
            return listJsonFormat;
        }

        /// <summary>
        /// 返回json字符串结果
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static string ToJson(this object model)
        {
            JavaScriptSerializer jss = new JavaScriptSerializer();
            //临时解决 X86产品组 数据大，超出MaxJsonLength指定大小。
            jss.MaxJsonLength = 999999999;
            return jss.Serialize(model);
        }

        /// <summary>
        /// 替换json字符串，值Value中的特殊字符
        /// </summary>
        /// <param name="jsonValue">json值</param>
        /// <returns></returns>
        public static string JsonValueRepalce(this string jsonValue)
        {
            if (!string.IsNullOrEmpty(jsonValue))
            {
                jsonValue = jsonValue.Replace("\\", "\\\\").Replace("\'", "\\\'").Replace("\t", " ").Replace("\r", " ").Replace("\n", "<br/>").Replace("\"", "'");
            }

            return jsonValue;
        }

        /// <summary>
        /// Json字符串反序列化成对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="json"></param>
        /// <returns></returns>
        public static T ToObject<T>(this string json)
        {
            JavaScriptSerializer js = new JavaScriptSerializer();
            return js.Deserialize<T>(json);
        }

        /// <summary>
        /// DataRowView转成Json 
        /// </summary>
        /// <param name="drv"></param>
        /// <returns></returns>
        public static string ToJson(this DataRowView drv)
        {
            StringBuilder Json = new StringBuilder();
            Json.Append("{");
            for (int i = 0; i < drv.Row.ItemArray.Length; i++)
            {
                Type type = drv.Row.ItemArray[i].GetType();
                string columnName = drv.Row.Table.Columns[i].ColumnName;
                Json.Append("" + columnName + ":" + StringFormat(drv.Row.ItemArray[i].ToString(), type));
                if (i < drv.Row.ItemArray.Length - 1) Json.Append(",");
            }
            Json.Append("}");
            return Json.ToString();
        }

        /// <summary>   
        /// DataTable转成Json    
        /// </summary>   
        /// <param name="jsonName">json中集合对象名称</param>
        /// <param name="dt"></param>   
        /// <returns></returns>   
        public static string ToJson(this DataTable dt, string jsonName)
        {
            StringBuilder Json = new StringBuilder();
            if (string.IsNullOrEmpty(jsonName)) jsonName = dt.TableName;
            Json.Append("{\"" + jsonName + "\":[");
            if (dt.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    Json.Append("{");
                    for (int j = 0; j < dt.Columns.Count; j++)
                    {
                        Type type = dt.Rows[i][j].GetType();
                        Json.Append("\"" + dt.Columns[j].ColumnName.ToString() + "\":" + StringFormat(dt.Rows[i][j].ToString(), type));
                        if (j < dt.Columns.Count - 1)
                            Json.Append(",");
                    }
                    Json.Append("}");
                    if (i < dt.Rows.Count - 1) Json.Append(",");
                }
            }
            Json.Append("]}");
            return Json.ToString();
        }

        /// <summary>    
        /// DataSet转换为Json    
        /// </summary>    
        /// <param name="dataSet">DataSet对象</param>    
        /// <returns>Json字符串</returns>    
        public static string ToJson(this DataSet dataSet)
        {
            string jsonString = "{";
            foreach (DataTable table in dataSet.Tables)
            {
                if (table.Rows.Count == 0) continue;
                jsonString += "\"" + table.TableName + "\":" + ToJson(table) + ",";
            }
            jsonString = jsonString.TrimEnd(',');
            return jsonString + "}";
        }

        /// <summary>    
        /// Datatable转换为Json    
        /// </summary>    
        /// <param name="dt">Datatable对象</param>    
        /// <returns>Json字符串</returns>    
        public static string ToJson(this DataTable dt)
        {
            if (dt == null || dt.Rows.Count == 0)//调整条件顺序，判空放在前面2015-1-13
            {
                return "{}";
            }
            StringBuilder jsonString = new StringBuilder();
            jsonString.Append("[");
            DataRowCollection drc = dt.Rows;
            for (int i = 0; i < drc.Count; i++)
            {
                jsonString.Append("{");
                for (int j = 0; j < dt.Columns.Count; j++)
                {
                    string strKey = dt.Columns[j].ColumnName;
                    string strValue = drc[i][j].ToString();
                    Type type = dt.Columns[j].DataType;
                    jsonString.Append("\"" + strKey + "\":");
                    strValue = StringFormat(strValue, type);
                    if (j < dt.Columns.Count - 1)
                        jsonString.Append(strValue + ",");
                    else
                        jsonString.Append(strValue);
                }
                jsonString.Append("},");
            }
            jsonString.Remove(jsonString.Length - 1, 1);
            jsonString.Append("]");
            return jsonString.ToString();
        }

        /// <summary>    
        /// DataReader转换为Json    
        /// </summary>    
        /// <param name="dataReader">DataReader对象</param>    
        /// <returns>Json字符串</returns>    
        public static string ToJson(this DbDataReader dataReader)
        {
            StringBuilder jsonString = new StringBuilder();
            jsonString.Append("[");
            while (dataReader.Read())
            {
                jsonString.Append("{");
                for (int i = 0; i < dataReader.FieldCount; i++)
                {
                    Type type = dataReader.GetFieldType(i);
                    string strKey = dataReader.GetName(i);
                    string strValue = dataReader[i].ToString();
                    jsonString.Append("\"" + strKey + "\":");
                    strValue = StringFormat(strValue, type);
                    if (i < dataReader.FieldCount - 1)
                        jsonString.Append(strValue + ",");
                    else
                        jsonString.Append(strValue);
                }
                jsonString.Append("},");
            }
            dataReader.Close();
            jsonString.Remove(jsonString.Length - 1, 1);
            jsonString.Append("]");
            return jsonString.ToString();
        }

        /// <summary>
        /// 将JSON解析成DataSet只限标准的JSON数据
        /// 例如：Json＝{t1:[{name:'数据name',type:'数据type'}]} 或 Json＝{t1:[{name:'数据name',type:'数据type'}],t2:[{id:'数据id',gx:'数据gx',val:'数据val'}]}
        /// </summary>
        /// <param name="Json">Json字符串</param>
        /// <returns>DataSet</returns>
        public static DataSet JsonToDataSet(string Json)
        {
            try
            {
                DataSet ds = new DataSet();
                JavaScriptSerializer JSS = new JavaScriptSerializer();
                object obj = JSS.DeserializeObject(Json);
                Dictionary<string, object> datajson = (Dictionary<string, object>)obj;
                foreach (var item in datajson)
                {
                    DataTable dt = new DataTable(item.Key);
                    object[] rows = (object[])item.Value;
                    foreach (var row in rows)
                    {
                        Dictionary<string, object> val = (Dictionary<string, object>)row;
                        DataRow dr = dt.NewRow();
                        foreach (KeyValuePair<string, object> sss in val)
                        {
                            if (!dt.Columns.Contains(sss.Key))
                            {
                                dt.Columns.Add(sss.Key.ToString());
                                dr[sss.Key] = sss.Value;
                            }
                            else
                                dr[sss.Key] = sss.Value;
                        }
                        dt.Rows.Add(dr);
                    }
                    ds.Tables.Add(dt);
                }
                return ds;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static void JsonDeserialize1(string strJson)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            Dictionary<string, string> aryJson = serializer.Deserialize<Dictionary<string, string>>(strJson);
        }
        public static void JsonDeserialize(string strJson, Control ctlSearch)
        {
            if (!string.IsNullOrEmpty(strJson))
            {
                strJson = JSBase64Decode(strJson.Replace(" ", "+"));

                MatchCollection mc = Regex.Matches(strJson, @"""(?<key>[^""]+)"":""(?<value>[^,}]+)""");
                Dictionary<string, string> dict = new Dictionary<string, string>();
                foreach (Match m in mc)
                {
                    if (dict.ContainsKey(m.Groups["key"].Value)) continue;//不能重复啊
                    dict.Add(m.Groups["key"].Value, m.Groups["value"].Value);
                }

                foreach (KeyValuePair<string, string> kvp in dict)
                {
                    Control ctl = ctlSearch.FindControl(kvp.Key);

                    if (ctl is HtmlInputText)
                    {
                        (ctl as HtmlInputText).Value = kvp.Value;
                    }
                    if (ctl is TextBox)
                    {
                        (ctl as TextBox).Text = kvp.Value;
                    }

                    if (ctl is HtmlSelect)
                    {
                        (ctl as HtmlSelect).Value = kvp.Value;
                    }
                    if (ctl is DropDownList)
                    {
                        (ctl as DropDownList).SelectedValue = kvp.Value;
                    }
                }
            }
        }

        /// <summary>
        /// Common.js JSEncodeBase64(str)加密对应服务器端解密方法
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string JSBase64Decode(string source)
        {
            string result;

            System.Text.UTF8Encoding encoder = new System.Text.UTF8Encoding();
            System.Text.Decoder utf8Decode = encoder.GetDecoder();
            byte[] todecode_byte = Convert.FromBase64String(source);

            int charCount = utf8Decode.GetCharCount(todecode_byte, 0, todecode_byte.Length);
            char[] decoded_char = new char[charCount];
            utf8Decode.GetChars(todecode_byte, 0, todecode_byte.Length, decoded_char, 0);
            result = new String(decoded_char);

            return result;
        }

        /// <summary>   
        /// 过滤特殊字符   
        /// </summary>   
        /// <param name="s"></param>   
        /// <returns></returns>   
        public static string String2Json(String s)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < s.Length; i++)
            {
                char c = s.ToCharArray()[i];
                switch (c)
                {
                    case '\"':
                        sb.Append("\\\""); break;
                    case '\\':
                        sb.Append("\\\\"); break;
                    case '/':
                        sb.Append("\\/"); break;
                    case '\b':
                        sb.Append("\\b"); break;
                    case '\f':
                        sb.Append("\\f"); break;
                    case '\n':
                        sb.Append("\\n"); break;
                    case '\r':
                        sb.Append("\\r"); break;
                    case '\t':
                        sb.Append("\\t"); break;
                    case '\0':
                        sb.Append(""); break;
                    default:
                        sb.Append(c); break;
                }
            }
            return sb.ToString().Replace('"', '\"').Replace("'", "\'");
        }

        /// <summary>   
        /// 过滤特殊字符   
        /// </summary>   
        /// <param name="s"></param>   
        /// <returns></returns>   
        public static string String3Json(String s)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < s.Length; i++)
            {
                char c = s.ToCharArray()[i];
                switch (c)
                {
                    case '\"':
                        sb.Append("&quot;"); break;
                    case '\'':
                        sb.Append("&#39;"); break;
                    case '\\':
                        sb.Append("\\\\"); break;
                    case '/':
                        sb.Append("\\/"); break;
                    case '\b':
                        sb.Append("\\b"); break;
                    case '\f':
                        sb.Append("\\f"); break;
                    case '\n':
                        sb.Append("\\n"); break;
                    case '\r':
                        sb.Append("\\r"); break;
                    case '\t':
                        sb.Append("\\t"); break;
                    case '\0':
                        sb.Append(""); break;
                    default:
                        sb.Append(c); break;
                }
            }
            return sb.ToString().Replace('"', '\"').Replace("'", "\'");
        }

        /// <summary>   
        /// 格式化字符型、日期型、布尔型   
        /// </summary>   
        /// <param name="str"></param>   
        /// <param name="type"></param>   
        /// <returns></returns>   
        private static string StringFormat(string str, Type type)
        {
            if (type == typeof(string))
            {
                str = String2Json(str);
                str = "\"" + str + "\"";
            }
            else if (type == typeof(DateTime))
            {
                str = "\"" + str + "\"";
            }
            else if (type == typeof(bool))
            {
                if (string.IsNullOrEmpty(str))
                    str = "false";
                else str = str.ToLower();
            }
            //else if (type == typeof(int))
            //{
            //    if (string.IsNullOrEmpty(str))
            //        str = "\"" + str + "\"";
            //}
            //else if (type == typeof(decimal))
            //{
            //    if (string.IsNullOrEmpty(str))
            //        str = "\"" + str + "\"";
            //}
            //else if (type == typeof(byte))
            //{
            //    if (string.IsNullOrEmpty(str))
            //        str = "\"" + str + "\"";
            //}
            if (string.IsNullOrEmpty(str))
                str = "\"" + str + "\"";
            return str;
        }
    }
}