using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Threading.Tasks;
using System.Data;

namespace Common
{
    public class LMSExceptionHelper
    {
        /// <summary>
        /// 写系统错误日志
        /// </summary>
        /// <param name="userCode">当前用户</param>
        /// <param name="source">来源（p and h）</param>
        /// <param name="ex">错误对象</param>
        /// <returns>新添加的错误ID</returns>
        public static int WriteLocalLog(string userCode,
            string errorCode, string source,
            Exception ex)
        {
            AppDomain domain = null;
            try
            {
                domain = AppDomain.CreateDomain("LMSLOG");
                var obj1 = domain.CreateInstanceFromAndUnwrap(Utility.GetMapPath("~/bin/BusinessLogic.dll"),
                    "BusinessLogic.SystemBusiness");
                var tp = obj1.GetType();
                System.Reflection.MethodInfo method = tp.GetMethod("Add_SysErrorLog");

                string text = string.Empty;
                string datas = string.Empty;
                foreach (var key in ex.Data.Keys) datas += ex.Data[key].ToString().Trim() + " > ";

                text = errorCode + "：" + ex.Message + "\r\n\t\t" +
                    ex.TargetSite + "\r\n\t\t" +
                    ex.Source + "\r\n\t\t" +
                    (!string.IsNullOrEmpty(datas) ? datas + "\r\n\t\t" : "") +
                    ex.HResult + " \r\n\t\t" +
                    ex.StackTrace + "\r\n\t\t" +
                    HttpContext.Current.Request.RawUrl + "\r\n";

                HttpBrowserCapabilities bc = HttpContext.Current.Request.Browser;

                int errCode = (int)method.Invoke(obj1, new object[] { 
                    userCode,
                    bc.Browser+"/"+bc.Version,
                    HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"]??string.Empty,
                    Utility.GetOSNameByUserAgent(HttpContext.Current.Request.UserAgent ?? "无"),
                    source,
                    text,
                    HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority) + HttpContext.Current.Request.Url.AbsolutePath,
                    HttpContext.Current.Request.Form.ToString(),
                    HttpContext.Current.Request.QueryString.ToString(),
                    DateTime.Now
                });

                return errCode;
            }
            catch
            {
                return 0;
            }
            finally
            {
                if (domain != null)
                    AppDomain.Unload(domain);
            }
        }
    }

    /// <summary>
    /// LMS系统Session自定义异常类
    /// </summary>
    public class LMSSessionException : Exception
    {
        public string GetErrorType { get { return this.GetType().ToString(); } }

        public LMSSessionException() { }

        public LMSSessionException(string message) : base(message) { }

        public LMSSessionException(string message, Exception inner)
            : base(message, inner) { }
    }

    /// <summary>
    ///  LMS系统事物异常类
    /// </summary>
    public class LMSThingException : Exception
    { 
         public string GetErrorType { get { return this.GetType().ToString(); } }

        public LMSThingException() { }

        public LMSThingException(string message) : base(message) { }

        public LMSThingException(string message, Exception inner)
            : base(message, inner) { }
    }

    /// <summary>
    /// LMS系统自定义异常类
    /// </summary>
    public class LMSException : Exception
    {
        public string GetErrorType { get { return this.GetType().ToString(); } }

        public LMSException() { }

        public LMSException(string message) : base(message) { }

        public LMSException(string message, Exception inner)
            : base(message, inner) { }
    }

    /// <summary>
    /// GP文件 自定义异常类
    /// </summary>
    public class LMSGPFileException : Exception
    {
        public string GetErrorType { get { return this.GetType().ToString(); } }

        public LMSGPFileException() { }

        public LMSGPFileException(string message) : base(message) { }

        public LMSGPFileException(string message, Exception inner)
            : base(message, inner) { }
    }
}
