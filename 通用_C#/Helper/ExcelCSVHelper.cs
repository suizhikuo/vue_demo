/* ExcelCSVHelper.cs
*
* 功 能： 操作Excel数据库、CSV文件帮助类
* 说明：从原来的框架中移植来的
* 类 名： ExcelCSVHelper
*
* Ver    变更日期         负责人  变更内容
* ───────────────────────────────────
* V0.01  2013.7.2       陈辉    初版
*/

using System;
using System.Data;
using System.Data.OleDb;
using System.Data.Odbc;
using System.Web.UI.HtmlControls;
using System.Collections;
using System.Collections.Specialized;
using System.IO;
using System.Text;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;

namespace Common
{
    /// <summary>
    /// 操作Excel数据库、CSV文件帮助类
    /// </summary>
    public class ExcelCSVHelper
    {
        private static string strCurFileName;	//使用组合函数输出Excel文件时使用的文件名
        private static FileStream oStream;		//当前文件流
        private static StreamWriter oWriter;	//当前输出流

        public static string GetExcelConnectionStr(string fileName)
        {
            return "provider=Microsoft.Jet.OLEDB.4.0" +
                   ";data source=" + fileName +
                   ";Extended Properties=\"Excel 8.0;HDR=YES;IMEX=1;\"";
        }
        public static string GetTextConnectionStr(string fileName)
        {
            string strPath = Path.GetDirectoryName(fileName);
            return "Provider=Microsoft.Jet.OLEDB.4.0" +
                   ";Data Source=" + strPath +
                   ";Extended Properties=\"text;HDR=Yes;IMEX=1;\"";//FMT=Delimited;
        }
        public static DataSet GetDBFromTextFile(string fn)
        {
            string fileName = Path.GetFileName(fn);
            string strSql = "SELECT * From " + fileName;
            string connStr = GetTextConnectionStr(fn);
            DataSet ds = new DataSet("DataSet1");
            System.Data.OleDb.OleDbDataAdapter oAdap = new OleDbDataAdapter(strSql, connStr);
            oAdap.Fill(ds);
            return ds;
        }
        /// <summary>
        /// 从一个指定的Excel文件中获取对应的DataSet数据类对象
        /// </summary>
        /// <param name="fileName">文件名称</param>
        /// <param name="tableName">Sql字符串</param>
        /// <returns></returns>
        public static DataSet GetDBFromExcel(string fileName, string strSql)
        {
            DataSet ds = new DataSet("DataSet1");
            string connStr = GetExcelConnectionStr(fileName);
            System.Data.OleDb.OleDbDataAdapter adap = new OleDbDataAdapter(strSql, connStr);
            adap.Fill(ds);
            return ds;
        }

        /// <summary>
        /// 从一个指定的Excel文件中获取对应的DataSet数据类对象
        /// </summary>
        /// <param name="fileName">文件名称</param>
        /// <param name="tableName">Sql字符串</param>
        /// <returns></returns>
        public static DataSet GetDBFromExcel(string fileName)
        {
            DataSet ds = new DataSet("DataSet1");
            int num = 1;
            while (true)
            {
                try
                {
                    string strSql = "SELECT * FROM [Sheet" + num.ToString() + "$]";
                    DataSet ds2 = GetDBFromExcel(fileName, strSql);
                    ds2.Tables[0].TableName = "Table" + num.ToString();
                    UpdateTableColumn(ds2.Tables[0]);
                    ds.Merge(ds2);
                    num++;
                }
                catch (Exception ex)
                {
                    ex.ToString();
                    break;
                }
            }
            //清除数据
            ClearNullData(ds);
            return ds;
        }
        /// <summary>
        /// 更新DataTable的Column名称
        /// </summary>
        /// <param name="dt"></param>
        private static void UpdateTableColumn(DataTable dt)
        {
            if (dt == null)
                return;
            foreach (DataColumn col in dt.Columns)
                col.ColumnName = col.ColumnName.Trim();
        }
        /// <summary>
        /// 情空Null数据
        /// </summary>
        /// <param name="ds"></param>
        /// <returns></returns>
        private static DataSet ClearNullData(DataSet ds)
        {
            if (ds == null)
                return ds;
            foreach (DataTable dt in ds.Tables)
            {
                //清空错误的列
                ArrayList alCol = new ArrayList();
                if (dt.Rows.Count == 0)
                    continue;
                //				DataRow titleRow=dt.Rows[0];
                //				foreach(DataColumn col in dt.Columns)
                //				{
                //					if(titleRow[col]==null||
                //						titleRow[col].ToString().Trim()=="")
                //						alCol.Add(col);
                //				}
                //				foreach(DataColumn col in alCol)
                //					dt.Columns.Remove(col);

                //清空错误的行
                ArrayList alRow = new ArrayList();
                foreach (DataRow row in dt.Rows)
                {
                    if (IsNull(row))
                        alRow.Add(row);
                }
                foreach (DataRow row in alRow)
                    dt.Rows.Remove(row);
            }
            return ds;
        }
        /// <summary>
        /// 判断一个DataRow是否全部为空数据
        /// </summary>
        /// <param name="row"></param>
        private static bool IsNull(DataRow row)
        {
            object[] items = row.ItemArray;
            bool isNull = true;
            foreach (object item in items)
            {
                if (item != null && item.ToString().Trim() != "")
                {
                    isNull = false;
                    break;
                }
            }
            return isNull;
        }
        /// <summary>
        /// 获取临时文件目录路径
        /// </summary>
        /// <returns></returns>
        public static string GetTempDirectory()
        {
            string directory = System.Configuration.ConfigurationSettings.AppSettings["TempDirectory"];
            if (!System.IO.Directory.Exists(directory))
                System.IO.Directory.CreateDirectory(directory);
            return directory;
        }
        /// <summary>
        /// 获取模版文件目录路径
        /// </summary>
        /// <returns></returns>
        public static string GetTemplateDirectory()
        {
            string directory = System.Configuration.ConfigurationSettings.AppSettings["TemplateDirectory"];
            //if(!System.IO.Directory.Exists(directory))
            //System.IO.Directory.CreateDirectory(directory);
            return directory;
        }
        /// <summary>
        /// 获取一个临时文件名称
        /// </summary>
        /// <returns></returns>
        public static string GetTempFileName(string extName, string userCode, int type)
        {
            return GetTempDirectory() + @"\" + Math.Abs(System.Guid.NewGuid().ToString().GetHashCode()).ToString() + "(" + userCode + ")" + extName;
        }
        /// <summary>
        /// 获取一个临时文件名称
        /// </summary>
        /// <param name="directory">临时目录</param>
        /// <param name="extName">临时文件扩展名称</param>
        /// <returns></returns>
        public static string GetTempFileName(string directory, string extName)
        {
            return directory + @"\" + System.Guid.NewGuid().ToString() + "." + extName;
        }

        /// <summary>
        /// 保存临时目录
        /// </summary>
        /// <param name="fileUp"></param>
        /// <returns></returns>
        public static string SaveTempFile(System.Web.UI.HtmlControls.HtmlInputFile fileUp, string userCode)
        {
            string extName = Path.GetExtension(fileUp.PostedFile.FileName);
            string tempFile = GetTempFileName(extName, userCode, 0);
            fileUp.PostedFile.SaveAs(tempFile);
            return tempFile;
        }
        /// <summary>
        /// 保存并且获取临时文件数据
        /// </summary>
        /// <param name="fileUp"></param>
        /// <param name="beDelete"></param>
        /// <returns></returns>
        public static DataSet GetTempFileDB(System.Web.UI.HtmlControls.HtmlInputFile fileUp, string userCode)
        {
            //保存临时文件
            string fn = SaveTempFile(fileUp, userCode);

            DataSet ds = null;
            //获取数据
            if (IsTextFile(fn))
            {
                ds = new DataSet("DataSet1");
                DataTable dt = CSVToDataTable(fn);
                dt.Rows.RemoveAt(0);
                UpdateTableColumn(dt);
                ds.Tables.Add(dt);
            }
            else
                ds = GetDBFromExcel(fn);

            return ds;
        }

        /// <summary>
        /// 保存并且获取临时文件数据-保留标题
        /// </summary>
        /// <param name="fileUp"></param>
        /// <param name="beDelete"></param>
        /// <returns></returns>
        public static DataSet GetTempFileDBWithTitle(System.Web.UI.HtmlControls.HtmlInputFile fileUp, string userCode)
        {
            //保存临时文件
            string fn = SaveTempFile(fileUp, userCode);

            DataSet ds = null;
            //获取数据
            if (IsTextFile(fn))
            {
                ds = new DataSet("DataSet1");
                DataTable dt = CSVToDataTable(fn);
                UpdateTableColumn(dt);
                ds.Tables.Add(dt);
            }
            else
                ds = GetDBFromExcel(fn);

            return ds;
        }

        /// <summary>
        /// 判断是否为CSV文件
        /// </summary>
        /// <param name="fileName"></param>
        public static bool IsTextFile(string fileName)
        {
            string extName = System.IO.Path.GetExtension(fileName).ToLower();
            if (extName == ".csv" || extName == ".txt")
                return true;
            return false;
        }
        /// <summary>
        /// 根据名称获取指定表中对应数据的的ID，
        /// 如果返回0表示不存在对应的数据。
        /// </summary>
        /// <param name="tn">表名称</param>
        /// <param name="strName">名称</param>
        /// <returns></returns>
        //public static int GetIDByName(TableNames tn, string strName)
        //{
        //    string strSql = string.Format("SELECT TOP 1 {0} FROM {1} WHERE Name='{2}'",
        //        tn.ToString() + "ID", tn.ToString(), strName);
        //    object retValue = CommonHelper.ExecuteScale(CommandType.Text, strSql, false, null);
        //    if (retValue == null)
        //        return 0;
        //    try
        //    {
        //        return int.Parse(retValue.ToString());
        //    }
        //    catch
        //    {
        //        return 0;
        //    }
        //}

        /// <summary>
        /// 把一个DataTable转化为一个CSV文件
        /// </summary>
        /// <param name="dt">DataTable对象</param>
        /// <param name="title">总的标题</param>
        /// <param name="ld">dbColName---->表的标题</param>
        /// <param name="isShowDBColName">是否现实数据库列名称</param>
        /// <param name="page"></param>
        /// <returns></returns>
        public static string DataTableToCSV(DataTable dt, string title, ListDictionary ld, bool isShowDBColName, System.Web.UI.Page page)
        {
            string path = page.MapPath(System.Web.HttpRuntime.AppDomainAppVirtualPath) + "/Temp/";

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            string fileName = System.Guid.NewGuid().ToString() + ".csv";
            FileStream oStream = File.Create(path + fileName);
            StreamWriter oWriter = new StreamWriter(oStream, Encoding.GetEncoding("GB2312"));
            //保证不能超出Excel所能承受的最大行数
            if (dt != null && dt.Rows.Count >= 65535)
            {
                oWriter.Write("请增加查询条件，减少导出数据量。Excel最大支持65535行数据！" + ",");
                oWriter.Write(oWriter.NewLine);
                oWriter.Flush();
                oWriter.Close();
                oStream.Close();
                return path + fileName;
            }
            //添加标题
            if (title != string.Empty)
            {
                oWriter.Write(title + ",");
                //oWriter.Write("本报价单仅供参考，正式产品价格以合同为准！");
                oWriter.Write(oWriter.NewLine);
            }
            //显示表的标题
            if (ld != null)
            {
                foreach (string name in ld.Values)
                    oWriter.Write(name.Trim() + ",");
                oWriter.Write(oWriter.NewLine);
            }
            if (dt != null)
            {
                //添加Column名称列
                if (isShowDBColName)
                {
                    foreach (string col in ld.Keys)
                        oWriter.Write(col + ",");
                    oWriter.Write(oWriter.NewLine);
                }
                //添加具体数据
                foreach (DataRow row in dt.Rows)
                {
                    foreach (string col in ld.Keys)
                    {
                        string str = row[col].ToString().Trim().Replace("\"", "").Replace("1900-1-1", "").Replace(" 0:00:00", "").Replace(",", "/").Replace("\r\n", " ");
                        oWriter.Write(str + ",");
                    }
                    oWriter.Write(oWriter.NewLine);
                }
            }
            oWriter.Flush();
            oWriter.Close();
            oStream.Close();
            return path + fileName;
        }

        /// <summary>
        /// 把一个DataTable转化为一个CSV文件
        /// </summary>
        /// <param name="dt">DataTable对象</param>
        /// <param name="title">总的标题</param>
        /// <param name="ld">dbColName---->表的标题</param>
        /// <param name="isShowDBColName">是否现实数据库列名称</param>
        /// <param name="page"></param>
        /// <returns></returns>
        public static string DataTableToCSV(DataTable dt, string title, System.Web.UI.Page page)
        {
            string path = page.MapPath(System.Web.HttpRuntime.AppDomainAppVirtualPath) + "/Temp/";

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            string fileName = System.Guid.NewGuid().ToString() + ".csv";
            FileStream oStream = File.Create(path + fileName);
            StreamWriter oWriter = new StreamWriter(oStream, Encoding.GetEncoding("GB2312"));
            //保证不能超出Excel所能承受的最大行数
            if (dt != null && dt.Rows.Count >= 65535)
            {
                oWriter.Write("请增加查询条件，减少导出数据量。Excel最大支持65535行数据！" + ",");
                oWriter.Write(oWriter.NewLine);
                oWriter.Flush();
                oWriter.Close();
                oStream.Close();
                return path + fileName;
            }
            //添加标题
            if (title != string.Empty)
            {
                oWriter.Write(title + ",");
                //oWriter.Write("本报价单仅供参考，正式产品价格以合同为准！");
                oWriter.Write(oWriter.NewLine);
            }
            foreach (DataColumn dc in dt.Columns)
            {
                oWriter.Write(dc.Caption.Trim() + ",");

            }
            oWriter.Write(oWriter.NewLine);
            if (dt != null)
            {
                //添加具体数据
                foreach (DataRow row in dt.Rows)
                {
                    foreach (DataColumn col in dt.Columns)
                    {
                        string str = row[col].ToString().Trim().Replace("\"", "").Replace("1900-1-1", "").Replace(" 0:00:00", "").Replace(",", "/").Replace("\r\n", " ");
                        oWriter.Write(str + ",");
                    }
                    oWriter.Write(oWriter.NewLine);
                }
            }
            oWriter.Flush();
            oWriter.Close();
            oStream.Close();
            return path + fileName;
        }

        /// <summary>
        /// 把一个DataTable转化为一个CSV文件(商机查询统计专用)
        /// </summary>
        /// <param name="dt">DataTable对象</param>
        /// <param name="dtSales">代理客户表</param>
        /// <param name="title">总的标题</param>
        /// <param name="ld">dbColName---->表的标题</param>
        /// <param name="isShowDBColName">是否现实数据库列名称</param>
        /// <param name="page"></param>
        /// <returns></returns>
        public static string DataTableToCSV(DataTable dt, DataTable dtSales, string title, ListDictionary ld, bool isShowDBColName, System.Web.UI.Page page)
        {
            string path = page.MapPath(System.Web.HttpRuntime.AppDomainAppVirtualPath) + "/Temp/";

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            string fileName = System.Guid.NewGuid().ToString() + ".csv";
            FileStream oStream = File.Create(path + fileName);
            StreamWriter oWriter = new StreamWriter(oStream, Encoding.GetEncoding("GB2312"));
            //保证不能超出Excel所能承受的最大行数
            if (dt != null && dt.Rows.Count >= 65535)
            {
                oWriter.Write("请增加查询条件，减少导出数据量。Excel最大支持65535行数据！" + ",");
                oWriter.Write(oWriter.NewLine);
                oWriter.Flush();
                oWriter.Close();
                oStream.Close();
                return path + fileName;
            }
            //添加标题
            oWriter.Write(title + ",");
            oWriter.Write(oWriter.NewLine);
            //显示表的标题
            if (ld != null)
            {
                foreach (string name in ld.Values)
                    oWriter.Write(name.Trim() + ",");
                oWriter.Write(oWriter.NewLine);
            }
            if (dt != null)
            {
                //添加Column名称列
                if (isShowDBColName)
                {
                    foreach (string col in ld.Keys)
                        oWriter.Write(col + ",");
                    oWriter.Write(oWriter.NewLine);
                }
                //添加具体数据
                foreach (DataRow row in dt.Rows)
                {
                    int index = 0;

                    foreach (string col in ld.Keys)
                    {
                        if (col.StartsWith("增值伙伴"))
                        {
                            DataRow[] arySales = dtSales.Select("BOID='" + row["BOID"].ToString() + "'");

                            if (arySales.Length > index)
                            {
                                string str = arySales[index]["AgentID"].ToString().Trim().Replace("\"", "").Replace("1900-1-1", "").Replace(" 0:00:00", "").Replace(",", "/").Replace("\r\n", " ");
                                oWriter.Write(str + ",");
                            }
                            else
                            {
                                string str = "";
                                oWriter.Write(str + ",");
                            }
                        }
                        else
                        {
                            if (col.StartsWith("代理名称"))
                            {
                                DataRow[] arySales = dtSales.Select("BOID='" + row["BOID"].ToString() + "'");

                                if (arySales.Length > index)
                                {
                                    string str = arySales[index]["AgentName"].ToString().Trim().Replace("\"", "").Replace("1900-1-1", "").Replace(" 0:00:00", "").Replace(",", "/").Replace("\r\n", " ");
                                    oWriter.Write(str + ",");
                                }
                                else
                                {
                                    string str = "";
                                    oWriter.Write(str + ",");
                                }

                                index++;
                            }
                            else
                            {
                                string str = row[col].ToString().Trim().Replace("\"", "").Replace("1900-1-1", "").Replace(" 0:00:00", "").Replace(",", "/").Replace("\r\n", " ");
                                //string str = row[col].ToString().Trim().Replace(",", "/");
                                oWriter.Write(str + ",");
                            }
                        }
                    }
                    oWriter.Write(oWriter.NewLine);
                }
            }
            oWriter.Flush();
            oWriter.Close();
            oStream.Close();
            return path + fileName;
        }

        /// <summary>
        /// 把一个DataView转化为一个CSV文件
        /// </summary>
        /// <param name="dt">DataTable对象</param>
        /// <param name="title">总的标题</param>
        /// <param name="ld">dbColName---->表的标题</param>
        /// <param name="isShowDBColName">是否现实数据库列名称</param>
        /// <param name="page"></param>
        /// <returns></returns>
        public static string DataViewToCSV(DataView dv, string title, ListDictionary ld, bool isShowDBColName, System.Web.UI.Page page)
        {
            string path = page.MapPath(System.Web.HttpRuntime.AppDomainAppVirtualPath) + "/Temp/";

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            string fileName = System.Guid.NewGuid().ToString() + ".csv";
            FileStream oStream = File.Create(path + fileName);
            StreamWriter oWriter = new StreamWriter(oStream, Encoding.GetEncoding("GB2312"));
            //添加标题
            oWriter.Write(title + ",");
            oWriter.Write(oWriter.NewLine);
            //显示表的标题
            if (ld != null)
            {
                foreach (string name in ld.Values)
                    oWriter.Write(name.Trim() + ",");
                oWriter.Write(oWriter.NewLine);
            }
            if (dv != null)
            {
                //添加Column名称列
                if (isShowDBColName)
                {
                    foreach (string col in ld.Keys)
                        oWriter.Write(col + ",");
                    oWriter.Write(oWriter.NewLine);
                }
                //添加具体数据
                for (int i = 0; i < dv.Count; i++)
                {
                    foreach (string col in ld.Keys)
                    {
                        string str = dv[i][col].ToString().Trim();
                        oWriter.Write(str + ",");
                    }
                    oWriter.Write(oWriter.NewLine);
                }
                /*
                foreach(DataRow row in dt.Rows)
                {
                    foreach(string col in ld.Keys)
                    {
                        string str=row[col].ToString().Trim();
                        oWriter.Write(str+",");
                    }
                    oWriter.Write(oWriter.NewLine);
                }
                */
            }
            oWriter.Flush();
            oWriter.Close();
            oStream.Close();
            return path + fileName;
        }

        #region 处理多个DataTable写入一个文件的函数组合
        public static void BeginWrite(System.Web.UI.Page page)
        {
            string path = page.MapPath(System.Web.HttpRuntime.AppDomainAppVirtualPath) + "/Temp/";

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            string fileName = System.Guid.NewGuid().ToString() + ".csv";
            strCurFileName = path + fileName;
            oStream = File.Create(path + fileName);
            oWriter = new StreamWriter(oStream, Encoding.GetEncoding("GB2312"));
        }

        public static void WriteTitle(string title)
        {
            //添加标题
            oWriter.Write(title + ",");
            oWriter.Write(oWriter.NewLine);
        }

        public static void WriteContent(DataTable dt, ListDictionary ld, bool isShowDBColName)
        {
            if (dt != null)
            {
                //添加Column名称列
                if (isShowDBColName)
                {
                    if (ld != null)
                    {
                        foreach (string name in ld.Values)
                            oWriter.Write(name.Trim() + ",");
                        oWriter.Write(oWriter.NewLine);
                    }

                    //foreach(DataColumn col in dt.Columns)
                    //{
                    //	oWriter.Write(col.ColumnName+",");
                    //}
                    //oWriter.Write(oWriter.NewLine);

                    //foreach(string col in dt.Columns.c)
                    //	oWriter.Write(col+",");
                    //oWriter.Write(oWriter.NewLine);
                }
                //添加具体数据
                foreach (DataRow row in dt.Rows)
                {
                    foreach (string col in ld.Keys)
                    //foreach(DataColumn col in dt.Columns)
                    {
                        string str = row[col].ToString().Trim();
                        oWriter.Write(str + ",");
                    }
                    oWriter.Write(oWriter.NewLine);
                }
            }
        }

        public static string EndWrite()
        {
            oWriter.Flush();
            oWriter.Close();
            oStream.Close();
            return strCurFileName;
        }
        #endregion

        #region 使用StreamReader将CVS文件转化为一个DataTable
        /// <summary>
        /// 将一个CVS文件转化为一个DataTable
        /// </summary>
        /// <param name="fn"></param>
        /// <returns></returns>
        public static DataTable CSVToDataTable(string fn)
        {
            FileStream stream = File.Open(fn, FileMode.Open);
            StreamReader reader = new StreamReader(stream, System.Text.Encoding.GetEncoding("GB2312"));
            string line = "";
            DataTable dt = new DataTable("sadf");

            while ((line = reader.ReadLine()) != null)
            {
                string[] values = line.Split(new char[] { ',' });

                int count = dt.Columns.Count;
                int addNum = values.Length - count;
                if (addNum > 0)
                    AddColumn(dt, addNum);

                DataRow row = dt.NewRow();
                for (int num = 0; num < values.Length; num++)
                    row[num] = values[num].Trim();
                dt.Rows.Add(row);
            }
            reader.Close();
            stream.Close();

            return dt;
        }
        /// <summary>
        /// 给DataTable添加DataColumn
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="colNum"></param>
        public static void AddColumn(DataTable dt, int addNum)
        {
            int count = dt.Columns.Count;
            for (int num = count - 1; num < count + addNum; num++)
            {
                int colIndex = num + 1;
                DataColumn col = new DataColumn(colIndex.ToString(), typeof(string));
                dt.Columns.Add(col);
            }
        }
        #endregion


        public static void ExcelOut(Page Page, string fileName)
        {
            HttpResponse Response = Page.Response;
            HttpServerUtility Server = Page.Server;

            string fn = Server.UrlDecode(fileName);
            FileStream fileStream = new FileStream(fn, FileMode.Open);
            long fileSize = fileStream.Length;
            fileStream.Close();
            Response.Clear();
            Response.Buffer = true;
            Response.Charset = "GB2312";
            Response.AppendHeader("Content-Disposition", "attachment;filename=Sheet1.csv");
            Response.ContentEncoding = System.Text.Encoding.GetEncoding("GB2312");
            Response.ContentType = "application/ms-excel";
            Response.AddHeader("Content-Length", fileSize.ToString());
            Page.EnableViewState = false;
            Response.WriteFile(fn);
            Response.Flush();
            Response.End();
        }

        public static void ExcelOutE(Page Page, string fileName)
        {
            HttpResponse Response = Page.Response;
            HttpServerUtility Server = Page.Server;

            string fn = Server.UrlDecode(fileName);
            FileStream fileStream = new FileStream(fn, FileMode.Open);
            long fileSize = fileStream.Length;
            fileStream.Close();
            Response.Clear();
            Response.Buffer = true;
            Response.Charset = "GB2312";
            Response.AppendHeader("Content-Disposition", "attachment;filename=Sheet1.xls");
            Response.ContentEncoding = System.Text.Encoding.GetEncoding("GB2312");
            Response.ContentType = "application/ms-excel";
            Response.AddHeader("Content-Length", fileSize.ToString());
            Page.EnableViewState = false;
            Response.WriteFile(fn);
            Response.Flush();
            Response.End();
        }

        public static void ExceloutE1(Page Page, string fileName)
        {
            HttpResponse Response = Page.Response;
            HttpServerUtility Server = Page.Server;

            string fn = Server.UrlDecode(fileName);
            FileStream fileStream = new FileStream(fn, FileMode.Open);
            long fileSize = fileStream.Length;
            fileStream.Close();
            Response.Clear();
            Response.Buffer = true;
            Response.Charset = "GB2312";
            Response.AppendHeader("Content-Disposition", "attachment;filename=ErrorSheet1.xls");
            Response.ContentEncoding = System.Text.Encoding.GetEncoding("GB2312");
            Response.ContentType = "application/ms-excel";
            Response.AddHeader("Content-Length", fileSize.ToString());
            Page.EnableViewState = false;
            Response.WriteFile(fn);
            Response.Flush();
            Response.End();
        }


        public static DataSet GetUploadDS(HtmlInputFile File, string title, string userCode, ref string msg)
        {
            int i = 0;

            DataSet ds = null;

            if (File.PostedFile.FileName.Trim() != "")
            {
                ds = GetTempFileDBWithTitle(File, userCode);
            }
            else
            {
                msg = "请您输入要上传的文件名！";
                return null;
            }

            if (ds.Tables[0].Rows.Count <= 0)
            {
                msg = "上传的文件类型不符！";
                return null;
            }

            //判断是否有空行
            for (i = 1; i < ds.Tables[0].Rows.Count; i++)
            {
                string partypename = ds.Tables[0].Rows[i][0].ToString().Trim();
                if (partypename == "")
                {
                    int X = i + 1;
                    msg = "请重新操作您上载的csv格式文件,请您在excel中,使用右键删除不需要的行.！的" + X + "行！";
                    return null;
                }

            }

            //判断模板类型是否当前模块的模板
            if (title != ds.Tables[0].Rows[0][0].ToString().Trim())
            {
                msg = "模板错误：请不要修改模板标题行数据！";
                return null;
            }
            //删除标题行和字段名称行
            ds.Tables[0].Rows.RemoveAt(0);
            //ds.Tables[0].Rows.RemoveAt(0);

            return ds;
        }
        /// <summary>
        /// 追加内容
        /// </summary>
        /// <param name="strs"></param>
        public static void AppendLines(string filename, string[] strs)
        {
            if (!File.Exists(filename)) return;
            string fileName = System.Guid.NewGuid().ToString() + ".csv";
            FileStream oStream = File.Open(filename, FileMode.Append);
            StreamWriter oWriter = new StreamWriter(oStream, Encoding.GetEncoding("GB2312"));
            foreach (string str in strs)
            {
                oWriter.Write(str);
                oWriter.Write(oWriter.NewLine);
            }
            oWriter.Flush();
            oWriter.Close();
            oStream.Close();
        }

        /// <summary>
        /// 把Web页面的数据以流的方式导入到Excel
        /// </summary>
        /// <param name="ds">DataSet对象有两个DataTable,第一个表存查询条件，第二个存需要导出的数据</param>
        /// <param name="FileName">文件名</param>
        /// <param name="page">目标页对象</param>
        public static void CreateExcelCSV(DataSet ds, string FileName, Page page, bool IsTitle)
        {
            HttpResponse resp;
            resp = page.Response;
            resp.Charset = "GB2312";
            resp.ContentEncoding = System.Text.Encoding.GetEncoding("GB2312"); 
            resp.ContentType = "text/csv";
            resp.AppendHeader("Content-Disposition", "attachment;filename=" + HttpUtility.UrlEncode(FileName) + ".csv");
            string colHeaders = "", ls_item = "";

            //定义表对象与行对象，同时用DataSet对其值进行初始化 
            //DataTable dt = ds.Tables[0];
            foreach (DataTable dt in ds.Tables)
            {
                DataRow[] myRow = dt.Select();//可以类似dt.Select("id>10")之形式达到数据筛选目的
                int i = 0;
                int cl = dt.Columns.Count;
                string strClear = String.Empty;
                colHeaders = "";
                //取得数据表各列标题，各标题之间以\t分割，最后一个列标题后加回车符 
                if (IsTitle)//第一个表格不需要输出列名
                {
                    for (i = 0; i < cl; i++)
                    {
                        if (i == (cl - 1))//最后一列，加\n
                        {
                            colHeaders += dt.Columns[i].Caption.ToString().Replace("\"", "").Replace("1900-1-1", "").Replace(" 0:00:00", "").Replace(",", "/").Replace("\r\n", " ");
                        }
                        else
                        {
                            colHeaders += dt.Columns[i].Caption.ToString().ToString().Replace("\"", "").Replace("1900-1-1", "").Replace(" 0:00:00", "").Replace(",", "/").Replace("\r\n", " ") + ",";
                        }

                    }
                    resp.Write(colHeaders.Trim() + "\r\n");
                }
                IsTitle = true;//后来的表格都需要列
                //向HTTP输出流中写入取得的数据信息 

                //逐行处理数据   
                foreach (DataRow row in myRow)
                {
                    //当前行数据写入HTTP输出流，并且置空ls_item以便下行数据     
                    for (i = 0; i < cl; i++)
                    {
                        if (i == (cl - 1))//最后一列，加\n
                        {
                            //if (row[i].ToString().StartsWith("0") && row[i].ToString().Length > 1 && row[i].ToString().IndexOf(".") <= -1)
                            //{
                            //    if (dt.Columns[i].ColumnName.IndexOf("美元") > -1 && row[i].ToString().Trim().Length > 0)
                            //    {
                            //        ls_item += "$" + row[i].ToString().Trim().Replace("\"", "").Replace("1900-1-1", "").Replace(" 0:00:00", "").Replace(",", "/").Replace("\n", " ").Replace("\r", " ");
                            //    }
                            //    else
                            //    {
                            //        if (dt.Columns[i].ColumnName.IndexOf("ITCODE") > -1)
                            //        {
                            //            ls_item += row[i].ToString().Trim().ToLower().Replace("\"", "").Replace("1900-1-1", "").Replace(" 0:00:00", "").Replace(",", "/").Replace("\n", " ").Replace("\r", " ");
                            //        }
                            //        else
                            //        {
                            //            //解决以0开头的字符串
                            //            ls_item +="\t"+ row[i].ToString().Trim().Replace("\"", "").Replace("1900-1-1", "").Replace(" 0:00:00", "").Replace(",", "/").Replace("\n", " ").Replace("\r", " ");
                            //        }

                            //    }


                            //}
                            //else
                            //{
                            if (dt.Columns[i].ColumnName.IndexOf("美元") > -1 && row[i].ToString().Trim().Length > 0)
                            {
                                ls_item += "$" + row[i].ToString().Trim().Replace("\"", "").Replace("1900-1-1", "").Replace(" 0:00:00", "").Replace(",", "/").Replace("\n", " ").Replace("\r", " ");
                            }
                            else
                            {
                                if (dt.Columns[i].ColumnName.IndexOf("拜访人") > -1 || dt.Columns[i].ColumnName.IndexOf("提交人") > -1 || dt.Columns[i].ColumnName.ToLower().IndexOf("itcode") > -1)
                                {
                                    ls_item += row[i].ToString().Trim().ToLower().Replace("\"", "").Replace("1900-1-1", "").Replace(" 0:00:00", "").Replace(",", "/").Replace("\n", " ").Replace("\r", " ") + ",";
                                }
                                else
                                {
                                    ls_item += row[i].ToString().Trim().Replace("\"", "").Replace("1900-1-1", "").Replace(" 0:00:00", "").Replace(",", "/").Replace("\n", " ").Replace("\r", " ") + ",";
                                }

                            }

                            // }

                        }
                        else
                        {
                            //if (row[i].ToString().StartsWith("0") && row[i].ToString().Length > 1 && row[i].ToString().IndexOf(".") <= -1)
                            //{
                            //    if (dt.Columns[i].ColumnName.IndexOf("美元") > -1 && row[i].ToString().Trim().Length > 0)
                            //    {
                            //        ls_item += "$" + row[i].ToString().Trim().Replace("\"", "").Replace("1900-1-1", "").Replace(" 0:00:00", "").Replace(",", "/").Replace("\n", " ").Replace("\r", " ") + ",";
                            //    }
                            //    else
                            //    {
                            //        if (dt.Columns[i].ColumnName.IndexOf("ITCODE") > -1)
                            //        {
                            //            ls_item += row[i].ToString().Trim().ToLower().Replace("\"", "").Replace("1900-1-1", "").Replace(" 0:00:00", "").Replace(",", "/").Replace("\n", " ").Replace("\r", " ") + ",";
                            //        }
                            //        else
                            //        {
                            //            ls_item += "\t" + row[i].ToString().Trim().Replace("\"", "").Replace("1900-1-1", "").Replace(" 0:00:00", "").Replace(",", "/").Replace("\n", " ").Replace("\r", " ") + ",";
                            //        }
                            //    }
                            //}
                            //else
                            //{
                            if (dt.Columns[i].ColumnName.IndexOf("美元") > -1 && row[i].ToString().Trim().Length > 0)
                            {
                                ls_item += "$" + row[i].ToString().Trim().Replace("\"", "").Replace("1900-1-1", "").Replace(" 0:00:00", "").Replace(",", "/").Replace("\n", " ").Replace("\r", " ") + ",";
                            }
                            else
                            {
                                if (dt.Columns[i].ColumnName.IndexOf("拜访人") > -1 || dt.Columns[i].ColumnName.IndexOf("提交人") > -1 || dt.Columns[i].ColumnName.ToLower().IndexOf("itcode") > -1)
                                {
                                    ls_item += row[i].ToString().Trim().ToLower().Replace("\"", "").Replace("1900-1-1", "").Replace(" 0:00:00", "").Replace(",", "/").Replace("\n", " ").Replace("\r", " ") + ",";
                                }
                                else
                                {
                                    ls_item += row[i].ToString().Trim().Replace("\"", "").Replace("1900-1-1", "").Replace(" 0:00:00", "").Replace(",", "/").Replace("\n", " ").Replace("\r", " ").Replace("PC", "DT").Replace("NB", "ZY") + ",";
                                }
                            }
                            // }

                        }
                    }
                    resp.Write(ls_item.Trim() + "\r\n");
                    ls_item = "";

                }

            }
            resp.End();
        }
    }
}
