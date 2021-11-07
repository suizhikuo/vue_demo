using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Web.SessionState;
using System.Reflection;
using System.Text.RegularExpressions;

namespace BSF.Portal
{
    public class SQLDefense : IHttpModule
    {
        //加入要驗證的特定文字 
        public static string[] blackList = { "--", "OR", "DELETE\\s+", "INSERT\\s+", "UPDATE\\s+", "TRUNCATE\\s+", "DROP\\s+", "CREATE\\s+", "ALTER\\s+" };
        public void Dispose()
        {
            //no-op    
        }

        //當網站每一個ASPX網頁執行時,會先跑Init事件   
        public void Init(HttpApplication app)
        {
            app.BeginRequest += new EventHandler(app_BeginRequest);
        }

        //加入要驗證的參數類型,包含QueryString,Form,Cookies   
        void app_BeginRequest(object sender, EventArgs e)
        {
            HttpRequest Request = (sender as HttpApplication).Context.Request;
            if (HttpContext.Current.Request.Url.ToString().IndexOf("Default.aspx") >= 0
                || HttpContext.Current.Request.Url.ToString().IndexOf("Content.aspx") >= 0
                || HttpContext.Current.Request.Url.ToString().IndexOf("header.aspx") >= 0
                || HttpContext.Current.Request.Url.ToString().IndexOf("HomePage.aspx") >= 0
                || HttpContext.Current.Request.Url.ToString().IndexOf("ModuleTree.aspx") >= 0
                || HttpContext.Current.Request.Url.ToString().IndexOf("PhoneContent.aspx") >= 0
                || HttpContext.Current.Request.Url.ToString().IndexOf("PhoneDefault.aspx") >= 0
                || HttpContext.Current.Request.Url.ToString().IndexOf("PhoneHeader.aspx") >= 0
                || HttpContext.Current.Request.Url.ToString().IndexOf("FrameSet.aspx") >= 0
                || HttpContext.Current.Request.Url.ToString().IndexOf("Error.aspx") >= 0
                //|| HttpContext.Current.Request.Url.ToString().IndexOf("Logon.aspx") >= 0
                || HttpContext.Current.Request.Url.ToString().IndexOf("PhoneLogon.aspx") >= 0
                || HttpContext.Current.Request.Url.ToString().IndexOf("Module/ModuleModify.aspx") >= 0
                || HttpContext.Current.Request.Url.ToString().IndexOf("Notice/SubSystemNoticeModify.aspx") >= 0
                || HttpContext.Current.Request.Url.ToString().IndexOf("Schedule/ScheduleModify.aspx") >= 0
                || HttpContext.Current.Request.Url.ToString().IndexOf("Notice/NoticeModify.aspx") >= 0)
            {
                //如果是以上页面，则不校验

            }
            else
            {
                foreach (string key in Request.QueryString)
                    CheckInput(Request.QueryString[key], key);
                foreach (string key in Request.Form)
                    CheckInput(Request.Form[key], key);
                CheckInput(Request.UserAgent, Request.UserAgent);
            }
        }

        //執行特定文字的驗證   
        private void CheckInput(string parameter, string fla)
        {
            if (String.IsNullOrEmpty(parameter) || String.IsNullOrEmpty(fla))
            {
                return;
            }
            if (fla == "__VIEWSTATE" || fla == "__EVENTVALIDATION" || fla == "s_sq" || fla == "odrid" || fla.IndexOf("_EventList") >= 0 || fla.IndexOf("grd") >= 0)
            {
                return;
            }

            string temp = fla;
            for (int i = 0; i < blackList.Length; i++)
            {
                if (parameter.Length > 11)
                {
                    if (parameter.Substring(0, 10) == "javascript")
                    {
                        continue;
                    }
                }
                string blckValue = blackList[i];
                if(Regex.IsMatch(parameter.ToUpper(), blackList[i]))
                //if ((parameter.IndexOf(blackList[i], StringComparison.OrdinalIgnoreCase) >= 0))
                {
                    //找到特定文字,跳至錯誤頁  
                    string aatest = blackList[i];
                    string err = "您输入了不合法的参数" + blackList[i].Replace("^","").Replace("$","");
                    HttpContext.Current.Response.Redirect("~/Error.aspx?_ErrDesc=" + err);
                }
            }
        }
    }
}
