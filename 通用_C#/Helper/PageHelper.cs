/* PageHelper.cs
*
* 功 能： PageHelper
* 类 名： PageHelper
*
* Ver    变更日期         负责人  变更内容
* ───────────────────────────────────
* V0.01  2013.12.1       金鑫    --
*/

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Collections;
using System.IO;
using System.Text.RegularExpressions;
using System.Web;

using ComponentArt.Web.UI;
using System.Diagnostics;



namespace Common
{

    public static class PageHelper
    {
        public const string C_HASRESULT = "查询结果";
        public const string C_NORESULT = "查询结果 - 当前查询条件下，查询结果为空";

        #region URL参数加、解密
        /// <summary>
        /// 自定义URL参数加密
        /// </summary>
        /// <param name="para"></param>
        /// <returns></returns>
        public static string UrlEncrypt(this string para)
        {
            if (string.IsNullOrEmpty(para)) return para;
            //自定义URL加密
            para = DESHelper.Encrypt(para, AppSettings.DESKey);
            //URL加密
            // para = HttpContext.Current.Server.UrlDecode(para);
            return para;
        }

        /// <summary>
        /// 自定义URL参数解密
        /// </summary>
        /// <param name="para"></param>
        /// <returns></returns>
        public static string UrlDecrypt(this string para)
        {
            if (string.IsNullOrEmpty(para)) return para;

            bool isHexa = para.Length % 16 == 0;
            if (!isHexa) return para;
            //自定义URL解密
            isHexa = Regex.IsMatch(para, "^[0-9A-Fa-f]+$");
            if (!isHexa) return para;
            para = DESHelper.Decrypt(para, AppSettings.DESKey);

            //URL加密
            //para = HttpContext.Current.Server.UrlDecode(para);
            return para;
        }
        #endregion

        #region Bind数据源
        /// <summary>
        /// 绑定DropDownList数据源
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ddl"></param>
        /// <param name="TextField"></param>
        /// <param name="ValueField"></param>
        public static void BindDropDownList<T>(this DropDownList ddl, string firstText = "", int firtValue = -99)
        {
            DataTable dt = Utility.GetEnumData<T>();
            ddl.DataValueField = "Value";
            ddl.DataTextField = "Name";
            ddl.DataSource = dt;
            ddl.DataBind();

            if (!string.IsNullOrEmpty(firstText))
            {
                ListItem listitems = new ListItem(firstText, firtValue.ToString());
                ddl.Items.Insert(0, listitems);
            }
        }

        /// <summary>
        /// 绑定DropDownList数据源
        /// </summary>
        /// <param name="ddl"></param>
        /// <param name="TextField">文本字段</param>
        /// <param name="ValueField">值字段</param>
        /// <param name="DataSource">数据源</param>
        public static void BindDropDownList(this DropDownList ddl, string textField, string valueField, object dataSource, bool hasDefault)
        {
            ddl.DataSource = dataSource;
            ddl.DataValueField = valueField;
            ddl.DataTextField = textField;
            ddl.DataBind();

            if (hasDefault)
            {
                ddl.Items.Insert(0, new ListItem()
                {
                    Text = ControlConstant.DDL_DEFAULT_TEXT,
                    Value = ControlConstant.DDL_DEFAULT_VALUE
                });
            }
        }

        /// <summary>
        /// 绑定RadioButtonList数据源
        /// </summary>
        /// <param name="rbl"></param>
        /// <param name="TextField">文本字段</param>
        /// <param name="ValueField">值字段</param>
        /// <param name="DataSource">数据源</param>
        public static void BindRadioButtonList(this RadioButtonList rbl, string textField, string valueField, object dataSource)
        {
            rbl.DataSource = dataSource;
            rbl.DataValueField = valueField;
            rbl.DataTextField = textField;
            rbl.DataBind();
        }

        /// <summary>
        /// 绑定CheckBoxList数据源
        /// </summary>
        /// <param name="cbl"></param>
        /// <param name="TextField">文本字段</param>
        /// <param name="ValueField">值字段</param>
        /// <param name="DataSource">数据源</param>
        public static void BindCheckBoxList(this CheckBoxList cbl, string textField, string valueField, object dataSource)
        {
            cbl.DataSource = dataSource;
            cbl.DataValueField = valueField;
            cbl.DataTextField = textField;
            cbl.DataBind();
        }
        #endregion

        #region CheckedItems
        /// <summary>
        /// 获取选中的Items
        /// </summary>
        /// <param name="grid"></param>
        /// <returns></returns>
        public static GridItemCollection GetCheckedItems(Grid grid)
        {
            GridItemCollection ret = grid.GetCheckedItems(grid.Levels[0].Columns[0]);
            foreach (GridItem item in ret)
            {
                item.ToArray()[0] = false;
            }
            return ret;
        }

        /// <summary>
        /// 获取选中的items的IDs
        /// </summary>
        /// <param name="grid"></param>
        /// <param name="IDCol">ID所在列数</param>
        /// <returns></returns>
        public static int[] GetCheckedIDi(Grid grid, int IDCol)
        {
            int[] ret = null;
            GridItemCollection items = GetCheckedItems(grid);
            int len = items.Count;
            ret = new int[len];
            for (int i = 0; i < len; i++)
            {
                ret[i] = int.Parse(items[i].ToArray()[IDCol].ToString());
            }
            return ret;
        }

        /// <summary>
        /// 获取选中的items的IDs
        /// </summary>
        /// <param name="grid"></param>
        /// <param name="IDCol">ID所在列数</param>
        /// <returns></returns>
        public static string[] GetCheckedIDs(Grid grid, int IDCol)
        {
            string[] ret = null;
            try
            {
                GridItemCollection items = GetCheckedItems(grid);
                int len = items.Count;
                ret = new string[len];
                for (int i = 0; i < len; i++)
                {
                    ret[i] = items[i].ToArray()[IDCol].ToString();
                }
            }
            catch (Exception)
            {
            }
            return ret;
        }

        /// <summary>
        ///  获取选中的items的ID串（1，2，3，4，。。。）
        /// </summary>
        /// <param name="grid"></param>
        /// <param name="IDCol">ID所在列数</param>
        /// <returns></returns>
        public static string GetCheckedIDiStr(Grid grid, int IDCol)
        {
            string ret = "";
            int[] rets = GetCheckedIDi(grid, IDCol);
            foreach (int i in rets)
            {
                ret += i + ",";
            }
            ret = ret.TrimEnd(',');
            return ret;
        }

        /// <summary>
        /// 获取选中的items的ID串（'1'，'2'，'3'，'4'，。。。）
        /// </summary>
        /// <param name="grid"></param>
        /// <param name="IDCol">ID所在列数</param>
        /// <returns></returns>
        public static string GetCheckedIDsStr(Grid grid, int IDCol)
        {
            string ret = "";
            string[] rets = GetCheckedIDs(grid, IDCol);
            foreach (string i in rets)
            {
                ret += "'" + i + "',";
            }
            ret = ret.TrimEnd(',');
            return ret;
        }

        #endregion

        # region SelectedItems

        /// <summary>
        /// 获取选中的Items
        /// </summary>
        /// <param name="grid"></param>
        /// <returns></returns>
        public static GridItemCollection GetSelectedItems(Grid grid)
        {
            GridItemCollection ret = grid.SelectedItems;
            return ret;
        }

        /// <summary>
        /// 获取选中的items的IDs
        /// </summary>
        /// <param name="grid"></param>
        /// <param name="IDCol">ID所在列数</param>
        /// <returns></returns>
        public static int[] GetSelectedIDi(Grid grid, int IDCol)
        {
            int[] ret = null;
            GridItemCollection items = GetSelectedItems(grid);
            int len = items.Count;
            ret = new int[len];
            for (int i = 0; i < len; i++)
            {
                ret[i] = int.Parse(items[i].ToArray()[IDCol].ToString());
            }
            return ret;
        }

        /// <summary>
        /// 获取选中的items的IDs
        /// </summary>
        /// <param name="grid"></param>
        /// <param name="IDCol">ID所在列数</param>
        /// <returns></returns>
        public static string[] GetSelectedIDs(Grid grid, int IDCol)
        {
            string[] ret = null;
            try
            {
                GridItemCollection items = GetSelectedItems(grid);
                int len = items.Count;
                ret = new string[len];
                for (int i = 0; i < len; i++)
                {
                    ret[i] = items[i].ToArray()[IDCol].ToString();
                }
            }
            catch (Exception)
            {
            }
            return ret;
        }

        /// <summary>
        ///  获取选中的items的ID串（1，2，3，4，。。。）
        /// </summary>
        /// <param name="grid"></param>
        /// <param name="IDCol">ID所在列数</param>
        /// <returns></returns>
        public static string GetSelectedIDiStr(Grid grid, int IDCol)
        {
            string ret = "";
            int[] rets = GetSelectedIDi(grid, IDCol);
            foreach (int i in rets)
            {
                ret += i + ",";
            }
            ret = ret.TrimEnd(',');
            return ret;
        }

        /// <summary>
        /// 获取选中的items的ID串（'1'，'2'，'3'，'4'，。。。）
        /// </summary>
        /// <param name="grid"></param>
        /// <param name="IDCol">ID所在列数</param>
        /// <returns></returns>
        public static string GetSelectedIDsStr(Grid grid, int IDCol)
        {
            string ret = "";
            string[] rets = GetSelectedIDs(grid, IDCol);
            foreach (string i in rets)
            {
                ret += "'" + i + "',";
            }
            ret = ret.TrimEnd(',');
            return ret;
        }

        #endregion

        public static void ViewGrid(Grid Grid1, HtmlGenericControl divResult, HtmlGenericControl divOperate, bool b)
        {
            Grid1.Visible = b;
            if (divOperate != null)
                divOperate.Visible = b;
            if (b)
                divResult.InnerText = C_HASRESULT;
            else
                divResult.InnerText = C_NORESULT;
        }

        /// <summary>
        /// 动态加载用户控件的内容
        /// </summary>
        /// <param name="virtualPath">用户控件所在的虚拟路径</param>
        /// <param name="parameter">加载时为空间提供的参数</param>
        /// <returns></returns>
        public static string LoadControl(this Page page, string virtualPath, Hashtable parameter)
        {
            try
            {

           
            page = new Page();
            Control control = page.LoadControl(virtualPath);
            Type type = control.GetType();
            if (parameter != null)
            {
                foreach (string name in parameter.Keys)
                {
                   
                    type.GetProperty(name).SetValue(control, parameter[name], null);
                }
            }
            page.Controls.Add(control);
          
            StringWriter writer = new StringWriter();
            page.Server.Execute(page, writer, true);
            return writer.ToString();
            }
            catch (Exception ec)
            {
                EventLog.WriteEntry( "LoadControlException",ec.Message, EventLogEntryType.Information);
            }
            return null;
        }

        /// <summary>
        /// 设置页面某控件下的所有对象的enable属性，应用在view页面
        /// </summary>
        /// <param name="ctl"></param>
        /// <param name="b"></param>
        public static void SetEnabled(Control ctl, bool b)
        {
            if (ctl.Controls != null)
            {
                foreach (Control c in ctl.Controls)
                {
                    SetEnabled(c, b);
                    switch (c.GetType().ToString())
                    {
                        case "System.Web.UI.WebControls.TextBox":
                            ((TextBox)c).ReadOnly = !b;
                            break;
                        case "System.Web.UI.WebControls.DropDownList":
                        case "System.Web.UI.WebControls.CheckBox":
                        case "System.Web.UI.WebControls.ListBox":
                        case "System.Web.UI.WebControls.RadioButton":
                            ((System.Web.UI.WebControls.WebControl)c).Enabled = b;
                            break;
                        case "System.Web.UI.WebControls.Button":
                            ((System.Web.UI.WebControls.WebControl)c).Visible = b;
                            break;
                        case "System.Web.UI.HtmlControls.HtmlInputText":
                            ((System.Web.UI.HtmlControls.HtmlInputText)c).Disabled = !b;
                            break;
                        case "System.Web.UI.HtmlControls.HtmlInputButton":
                            if (((HtmlInputButton)c).Value != "返回")
                                ((System.Web.UI.HtmlControls.HtmlInputButton)c).Visible = b;
                            break;
                    }
                }
            }
        }
    }
}
