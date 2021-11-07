using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Common
{
    public class OtherHelper
    {
        public static void WriteResponseTime(TimeSpan ts)
        {
            AppDomain domain = null;
            try
            {
                domain = AppDomain.CreateDomain("LMSWRT");
                var obj1 = domain.CreateInstanceFromAndUnwrap(Utility.GetMapPath("~/bin/BusinessLogic.dll"),
                    "BusinessLogic.OtherBusiness");
                var tp = obj1.GetType();
                System.Reflection.MethodInfo method = tp.GetMethod("SetResponseTime");

                StringBuilder jsonString = new StringBuilder("{");
                jsonString.AppendFormat("\"Authority\":\"{0}\",", HttpContext.Current.Request.Url.Authority);
                jsonString.AppendFormat("\"AbsolutePath\":\"{0}\",", HttpContext.Current.Request.Url.AbsolutePath);
                jsonString.AppendFormat("\"RequestType\":\"{0}\",", HttpContext.Current.Request.RequestType);
                jsonString.AppendFormat("\"Forms\":\"{0}\",", HttpContext.Current.Request.Form.ToString());
                jsonString.AppendFormat("\"QueryString\":\"{0}\",", HttpContext.Current.Request.QueryString.ToString());
                jsonString.AppendFormat("\"ResponseSecond\":\"{0}\",", ts.TotalSeconds);
                jsonString.AppendFormat("\"ResponseMilliseconds\":\"{0}\"", ts.TotalMilliseconds);
                jsonString.Append("}");

                int result = (int)method.Invoke(obj1, new object[] { jsonString.ToString() });

            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally {
                if (domain != null)
                    AppDomain.Unload(domain);
            }
        }
    }
}
