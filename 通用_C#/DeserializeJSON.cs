using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;

/// <summary>
/// DeserializeJSON 的摘要说明
/// http://www.tuicool.com/articles/7b6fU3i
/// </summary>
/// 
public class DeserializeJSON
{
    public DeserializeJSON()
    {
        //
        // TODO: 在此处添加构造函数逻辑
        //
    }

    private object GetProjectFromJSON(string json)
    {
        JavaScriptSerializer ser = new JavaScriptSerializer();
        dynamic projectNode = ser.Deserialize<dynamic>(json);
        return new
        {
            Name = projectNode["name"],
            Description = projectNode["description"],
            SourceUrl = projectNode["html_url"],
            Url = projectNode["homepage"]
        };
    }



}