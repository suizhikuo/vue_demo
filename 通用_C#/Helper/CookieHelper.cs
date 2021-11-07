/* CookieHelper.cs
*
* 功 能： CookieHelper
* 类 名： CookieHelper
*
* Ver    变更日期         负责人  变更内容
* ───────────────────────────────────
* V0.01  2013.12.1       金鑫    --
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Reflection;

namespace Common
{
    /// <summary>
    /// Cookie Helper
    /// </summary>
    public class CookieHelper
    {
        /// <summary>
        /// 将String写入cookie
        /// </summary>
        /// <param name="cookieName">名称</param>
        /// <param name="strValue">值</param>
        public static void WriteCookieByString(string cookieName, string strValue)
        {
            WriteCookieByString(cookieName, strValue, -1);
        }

        /// <summary>
        /// 将String写入cookie
        /// </summary>
        /// <param name="cookieName">名称</param>
        /// <param name="strValue">值</param>
        /// <param name="strValue">过期时间(分钟)</param>
        public static void WriteCookieByString(string cookieName, string strValue, int expires)
        {
            HttpCookie cookie = HttpContext.Current.Request.Cookies[cookieName];
            if (cookie == null)
                cookie = new HttpCookie(cookieName);
            cookie.Value = strValue;
            if (expires > -1) cookie.Expires = DateTime.Now.AddMinutes(expires);
            HttpContext.Current.Response.AppendCookie(cookie);
        }

        /// <summary>
        /// 将String写入cookie
        /// </summary>
        /// <param name="cookieName">名称</param>
        /// <param name="strValue">值</param>
        public static void WriteCookieByString(string cookieName, string key, string strValue)
        {
            HttpCookie cookie = HttpContext.Current.Request.Cookies[cookieName];
            if (cookie == null)
                cookie = new HttpCookie(cookieName);
            cookie[key] = strValue;
            HttpContext.Current.Response.AppendCookie(cookie);
        }

        /// <summary>
        /// 从cookie中获取String值
        /// </summary>
        /// <param name="cookieName">名称</param>
        /// <returns>cookie值</returns>
        public static string ReadCookieToString(string cookieName)
        {
            if (HttpContext.Current.Request.Cookies != null && HttpContext.Current.Request.Cookies[cookieName] != null)
                return HttpContext.Current.Request.Cookies[cookieName].Value.ToString();

            return string.Empty;
        }

        /// <summary>
        /// 从cookie中获取String值
        /// </summary>
        /// <param name="cookieName">名称</param>
        /// <returns>cookie值</returns>
        public static string ReadCookieToString(string cookieName, string key)
        {
            if (HttpContext.Current.Request.Cookies != null && HttpContext.Current.Request.Cookies[cookieName] != null && HttpContext.Current.Request.Cookies[cookieName][key] != null)
                return HttpContext.Current.Request.Cookies[cookieName][key].ToString();

            return string.Empty;
        }

        /// <summary>
        /// 将实体写入cookie
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="cookieName"></param>
        /// <param name="t"></param>
        public static void WriteCookieByModel<T>(string cookieName, T t)
        {
            WriteCookieByModel(cookieName, t, -1);
        }

        /// <summary>
        /// 将实体写入cookie
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="cookieName"></param>
        /// <param name="t"></param>
        public static void WriteCookieByModel<T>(string cookieName, T t, int expires)
        {
            bool isUpdate = true;
            HttpCookie cookie = HttpContext.Current.Request.Cookies[cookieName];
            if (cookie == null)
            {
                isUpdate = false;
                cookie = new HttpCookie(cookieName);
            }
            PropertyInfo[] p = t.GetType().GetProperties(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public);
            foreach (System.Reflection.PropertyInfo item in p)
            {
                if (item.GetValue(t, null) != null)
                    cookie.Values.Set(item.Name, HttpUtility.UrlEncode(item.GetValue(t, null).ToString()));
            }
            if (expires > -1) cookie.Expires = DateTime.Now.AddMinutes(expires);
            if (isUpdate)
                HttpContext.Current.Response.Cookies.Set(cookie);
            else HttpContext.Current.Response.Cookies.Add(cookie);
        }

        /// <summary>
        /// 从cookie中获取实体对象
        /// </summary>
        /// <param name="cookieName"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        public static T ReadCookieToModel<T>(string cookieName)
        {
            T t = Activator.CreateInstance<T>();
            Type type = typeof(T);
            PropertyInfo[] p = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            if (HttpContext.Current.Request.Cookies[cookieName] != null)
            {
                foreach (PropertyInfo item in p)
                {
                    if (!item.CanWrite) { continue; }
                    object val = GetVlues(item.PropertyType.Name.ToLower(), HttpUtility.UrlDecode(HttpContext.Current.Request.Cookies[cookieName][item.Name]));
                    if (val == null) continue;
                    item.SetValue(t, val, null);
                }
            }
            return t;
        }
        private static object GetVlues(string type, string values)
        {
            object obj = null;
            switch (type)
            {
                case "int32":
                    obj = Convert.ToInt32(values);
                    break;
                case "double":
                    obj = Convert.ToDouble(values);
                    break;
                case "datetime":
                    obj = Convert.ToDateTime(values);
                    break;
                case "decimal":
                    obj = Convert.ToDecimal(values);
                    break;
                case "boolean":
                    obj = Convert.ToBoolean(values);
                    break;
                default:
                    obj = values;
                    break;
            }
            return obj;
        }

        /// <summary>
        /// 将Object对象写cookie
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="cookieName"></param>
        /// <param name="t"></param>
        public static void WriteCookieByObject<T>(string cookieName, T t)
        {
            WriteCookieByObject<T>(cookieName, t, 0, 120);
        }

        /// <summary>
        /// 将Object对象写cookie
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="cookieName"></param>
        /// <param name="size"></param>
        /// <param name="t"></param>
        /// <param name="expires"></param>
        public static void WriteCookieByObject<T>(string cookieName, T t, int size, int expires)
        {
            if (t == null) return;
            string strNew = SerializableHelper.SerializableObjectToStrong<T>(t);
            if (string.IsNullOrEmpty(strNew)) return;
            List<string> slist = new List<string>();
            SetCookiesStrings(strNew, size <= 0 ? 2048 : size, ref slist);
            ClearCookies(cookieName, 0);
            for (int i = 0; i < slist.Count; i++)
            {
                HttpCookie cookie = new HttpCookie(cookieName + i);
                cookie.Value = slist[i];
                cookie.Path = "/";
                if (expires > -1) cookie.Expires = DateTime.Now.AddMinutes(expires);
                HttpContext.Current.Response.Cookies.Add(cookie);
            }
        }
        private static void SetCookiesStrings(string strNew, int size, ref List<string> slist)
        {
            if (strNew.Length < size) slist.Add(strNew);
            else
            {
                string nstr = strNew.Substring(0, size);
                slist.Add(nstr);
                nstr = strNew.Replace(nstr, string.Empty);
                SetCookiesStrings(nstr, size, ref slist);
            }
        }

        /// <summary>
        /// 从cookie中获取对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="cookieName"></param>
        /// <returns></returns>
        public static T ReadCookieToObject<T>(string cookieName)
        {
            StringBuilder str = new StringBuilder();
            GetCookiesStrings(cookieName, 0, ref str);
            if (str.Length == 0) return default(T);
            return SerializableHelper.DeserializeStringToObject<T>(str.ToString());
        }
        private static void GetCookiesStrings(string cookieName, int index, ref StringBuilder str)
        {
            HttpCookie cookie = HttpContext.Current.Request.Cookies[cookieName + index];
            index = index + 1;
            if (cookie != null)
            {
                str.Append(cookie.Value);
                GetCookiesStrings(cookieName, index, ref str);
            }
        }


        /// <summary>
        /// 清理Cookie
        /// </summary>
        /// <param name="cookieName"></param>
        public static void ClearCookies(string cookieName, int index)
        {
            HttpCookie cookie = HttpContext.Current.Request.Cookies[cookieName + index];
            index = index + 1;
            if (cookie != null)
            {
                cookie.Value = string.Empty;
                cookie.Expires = DateTime.Now.AddDays(-1);
                HttpContext.Current.Response.SetCookie(cookie);
                ClearCookies(cookieName, index);
            }
            else
            {
                if (index <= 10)
                    ClearCookies(cookieName, index);
            }
        }

        /// <summary>
        /// 删除cookie
        /// </summary>
        /// <param name="cookiename"></param>
        public static void ClearCookie(string cookieName)
        {
            if (HttpContext.Current.Request.Cookies[cookieName] != null)
            {
                HttpCookie cookie = HttpContext.Current.Request.Cookies[cookieName];
                cookie.Expires = DateTime.Now.AddMinutes(-1);
                HttpContext.Current.Response.Cookies.Set(cookie);
            }
        }

        /// <summary>
        /// 判断Cookie是否存在
        /// </summary>
        /// <param name="cookiename"></param>
        /// <returns></returns>
        public static bool IsExistCookies(string cookiename, int index)
        {
            HttpCookie cookie = HttpContext.Current.Request.Cookies[cookiename + index];
            return cookie != null;
        }

        /// <summary>
        /// 判断Cookie是否存在
        /// </summary>
        /// <param name="cookiename"></param>
        /// <returns></returns>
        public static bool IsExistCookie(string cookiename)
        {
            HttpCookie cookie = HttpContext.Current.Request.Cookies[cookiename];
            return cookie != null;
        }
    }
}
