using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI;
using Microsoft.BSF.Common.Entity;
using NPOI.HPSF;
using NPOI.HSSF.Record;
using NPOI.HSSF.UserModel;
using NPOI.HSSF.Util;
using NPOI.SS.UserModel;
using System.Configuration;
using System.Diagnostics;
using Common.Helper.MQSerializableClass;

namespace Common
{
    public partial class ExcelHelper
    {
        #region 写excel相关
        /// <summary>
        /// 直接从dataset生成Excel文件
        /// </summary>
        public static void DataSet2File(DataSet ds, string Path)
        {
            HSSFWorkbook hssfworkbook = DataSet2HSSFWorkbook(ds);
            WriteToFile(hssfworkbook, Path);
        }

        public static void ExportExcel(DataSet ds, string rarName, HSSFWorkbook hssfworkbook, Dictionary<string, string> dic)
        {
            ISheet sheet1 = hssfworkbook.GetSheetAt(0);
            {
                foreach (DataTable dataTable in ds.Tables)
                {
                    int rowIndex = 2;
                    for (int i = 0; i < dataTable.Rows.Count; i++)
                    {

                        IRow row = sheet1.CreateRow(rowIndex);
                        int j = 1;
                        foreach (var k in dic.Keys)
                        {
                            #region 设置单元格数据和格式
                            ICell cell = row.CreateCell(j);
                            switch (k)
                            {
                                case "Qty":
                                case "UnitPrice":
                                case "SuggestPrice":
                                case "MA":
                                case "dutyrate":
                                case "Otherfee":
                                case "cost":
                                case "BMCadder":
                                case "NONBMCadder":
                                case "NONBMCRate":
                                case "BidRefPrice":
                                case "COC":
                                    int x = (int)double.Parse(dataTable.Rows[i][k].ToString());
                                    cell.SetCellValue(x);
                                    break;
                                case "NetRev":
                                case "duty":
                                case "GM":
                                case "GM%":
                                case "NONBMC":
                                case "GP":
                                case "GP%":
                                case "Discount":
                                    cell.SetCellType(CellType.Formula);//公式 单元格格式
                                    cell.SetCellFormula(dic[k].FormatWith(i + 3));
                                    break;
                                case "CD":
                                case "VAT":
                                    cell.SetCellValue(double.Parse(dataTable.Rows[i][k].ToString()));
                                    HSSFCellStyle cellStyle = (HSSFCellStyle)hssfworkbook.CreateCellStyle();
                                    cellStyle.DataFormat = HSSFDataFormat.GetBuiltinFormat("0%");
                                    cell.CellStyle = cellStyle;
                                    break;
                                default:
                                    cell.SetCellValue(dataTable.Rows[i][k].ToString());
                                    break;
                            }
                            #endregion
                            j++;
                        }
                        rowIndex++;
                    }
                    IRow rowTitle = sheet1.CreateRow(rowIndex);
                    rowTitle = sheet1.GetRow(2);
                }
                //公式计算 -- 打开
                sheet1.ForceFormulaRecalculation = true;
            }
        }
        private void InsertRow(HSSFWorkbook wb, HSSFSheet sheet, int starRow, int rows)
        {
            /*
             * ShiftRows(int startRow, int endRow, int n, bool copyRowHeight, bool resetOriginalRowHeight); 
             * 
             * startRow 开始行
             * endRow 结束行
             * n 移动行数
             * copyRowHeight 复制的行是否高度在移
             * resetOriginalRowHeight 是否设置为默认的原始行的高度
             * 
             */

            sheet.ShiftRows(starRow + 1, sheet.LastRowNum, rows, true, true);

            starRow = starRow - 1;

            for (int i = 0; i < rows; i++)
            {

                HSSFRow sourceRow = null;
                HSSFRow targetRow = null;

                short m;

                starRow = starRow + 1;
                sourceRow = (HSSFRow)sheet.GetRow(starRow);
                targetRow = (HSSFRow)sheet.CreateRow(starRow + 1);
                targetRow.HeightInPoints = sourceRow.HeightInPoints;

                for (m = (short)sourceRow.FirstCellNum; m < sourceRow.LastCellNum; m++)
                {

                    var sourceCell = (HSSFCell)sourceRow.GetCell(m);
                    var targetCell = (HSSFCell)targetRow.CreateCell(m);

                    //targetCell.Encoding = sourceCell.Encoding;
                    targetCell.CellStyle = sourceCell.CellStyle;
                    targetCell.SetCellType(sourceCell.CellType);

                }
            }

        }

        /// <summary>
        /// 导出Excel
        /// </summary>
        /// <param name="DataSet">dsSource</param>
        /// <param name="filePath">filePath</param>
        public static void ExportExcel(DataSet dsSource, string filePath)
        {
            HSSFWorkbook excelWorkbook = CreateExcelFile();
            for (int i = 0; i < dsSource.Tables.Count; i++)
            {
                ISheet newsheet = excelWorkbook.CreateSheet(dsSource.Tables[i].TableName);

                IRow headerRow = newsheet.CreateRow(0);

                foreach (DataColumn column in dsSource.Tables[i].Columns)
                {
                    headerRow.CreateCell(column.Ordinal).SetCellValue(column.ColumnName);
                }
                int rowIndex = 0;
                int sheetCount = 1;
                foreach (DataRow row in dsSource.Tables[i].Rows)
                {
                    rowIndex++;
                    if (rowIndex == 15000)
                    {
                        rowIndex = 1;
                        sheetCount++;
                        newsheet = excelWorkbook.CreateSheet(dsSource.Tables[i].TableName + sheetCount);
                        headerRow = newsheet.CreateRow(0);
                        foreach (DataColumn column in dsSource.Tables[i].Columns)
                        {
                            headerRow.CreateCell(column.Ordinal).SetCellValue(column.ColumnName);
                        }
                    }
                    IRow dataRow = newsheet.CreateRow(rowIndex);
                    foreach (DataColumn column in dsSource.Tables[i].Columns)
                    {
                        dataRow.CreateCell(column.Ordinal).SetCellValue(row[column].ToString());
                    }
                }
                //InsertRow(dsSource.Tables[i], dsSource.Tables[i].TableName, excelWorkbook);
            }
            SaveExcelFile(excelWorkbook, filePath);
        }

        /// <summary>
        /// 下载文件
        /// </summary>
        /// <param name="fileName">文件名</param>
        public static void FIM_DownLoadFiles(string fileName)
        {
            string filePath = HttpContext.Current.Server.MapPath("~/") + "/Temp/" + fileName;
            FileInfo DownloadFile = new FileInfo(filePath);
            HttpContext.Current.Response.Clear();
            HttpContext.Current.Response.ClearHeaders();
            HttpContext.Current.Response.Buffer = false;
            HttpContext.Current.Response.ContentType = "application/octet-stream";
            HttpContext.Current.Response.AppendHeader("Content-Disposition", "attachment;filename=" + HttpUtility.UrlEncode(DownloadFile.Name, Encoding.UTF8));
            HttpContext.Current.Response.AppendHeader("Content-Length", DownloadFile.Length.ToString());
            HttpContext.Current.Response.WriteFile(DownloadFile.FullName);
            HttpContext.Current.Response.Flush();
            HttpContext.Current.Response.End();
        }

        /// <summary>
        /// 创建Excel文件
        /// </summary>
        /// <param name="filePath"></param>
        protected static HSSFWorkbook CreateExcelFile()
        {
            HSSFWorkbook hssfworkbook = new HSSFWorkbook();
            return hssfworkbook;
        }

        /// <summary>
        /// 保存Excel文件
        /// </summary>
        /// <param name="excelWorkBook"></param>
        /// <param name="filePath"></param>
        protected static void SaveExcelFile(HSSFWorkbook excelWorkBook, string filePath)
        {
            FileStream file = null;
            try
            {
                if (!string.IsNullOrEmpty(filePath))
                {
                    string path = filePath.Substring(0, filePath.LastIndexOf("/"));
                    if (!File.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }
                }
                file = new FileStream(filePath, FileMode.Create);
                excelWorkBook.Write(file);
            }
            finally
            {
                if (file != null)
                {
                    file.Close();
                }
            }
        }

        /// <summary>
        /// 创建Excel文件头 
        /// </summary>
        /// <param name="excelWorkbook">HSSFWorkbook</param>
        /// <param name="sheet">Sheet</param>
        /// <param name="columnName">列名称（分号分隔）</param>
        /// <param name="columnWidth">列宽（分号分隔）</param>
        /// <param name="ColumnOrder">列次，从1开始</param>
        /// <returns>IRow</returns>
        protected static IRow FIM_CreateExcelHead(HSSFWorkbook excelWorkbook, ISheet sheet, string columnName, string columnWidth, int ColumnOrder)
        {
            IRow irow = null;
            if (ColumnOrder == 1)
            {
                irow = sheet.CreateRow(0);
            }
            else
            {
                irow = sheet.GetRow(0);
            }
            irow.Height = 20 * 25;
            //垂直上下左右居中
            ICellStyle style = excelWorkbook.CreateCellStyle();
            style.VerticalAlignment = VerticalAlignment.Center;
            style.Alignment = HorizontalAlignment.Center;
            //背景色
            style.FillForegroundColor = HSSFColor.CornflowerBlue.Index;
            style.FillPattern = FillPattern.SolidForeground;

            string[] arrColumnName = columnName.Split(';');
            string[] arrColumnWidth = columnWidth.Split(';');
            int CellIndex = (ColumnOrder - 1) * arrColumnName.Length;
            for (int i = 0; i < arrColumnName.Length; i++)
            {
                ICell icell = irow.CreateCell(CellIndex);
                icell.SetCellValue(arrColumnName[i]);
                icell.CellStyle = style;
                int width = Convert.ToInt32(arrColumnWidth[i]);
                sheet.SetColumnWidth(CellIndex, 20 * width);
                CellIndex++;
            }
            //冻结首行
            //sheet.CreateFreezePane(ColumnOrder * arrColumnName.Length, 1, ColumnOrder * arrColumnName.Length+1, 1);
            return irow;
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
        public static string DataTableToCSV(DataTable dt, string title, ListDictionary ld, bool isShowDBColName, Page page)
        {
            string path = page.MapPath(HttpRuntime.AppDomainAppVirtualPath) + "/Temp/";

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            string fileName = Guid.NewGuid().ToString() + ".csv";
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
                        string str = string.Empty;

                        if (col == "CharacteristicValueCode")
                        {
                            str = row[col].ToString().Trim().PadLeft(25, '#');
                        }
                        else if (col == "CharacteristicCode")
                        {
                            str = row[col].ToString().Trim().PadLeft(15, '#');
                        }

                        else
                        {
                            str = row[col].ToString().Trim() == "0.00" ? "" : row[col].ToString().Trim().Replace("\"", "").Replace("1900-1-1", "").Replace(" 0:00:00", "").Replace(",", "/").Replace("\r\n", " ");
                        }


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
        /// 直接从dataset生成Excel文件流
        /// </summary>
        public static MemoryStream DataSet2Stream(DataSet ds)
        {
            HSSFWorkbook hssfworkbook = DataSet2HSSFWorkbook(ds);
            return WriteToMemoryStream(hssfworkbook);
        }

        /// <summary>
        /// 直接从datatable生成Excel文件
        /// </summary>
        public static void DataTable2File(DataTable dt, string Path)
        {
            HSSFWorkbook hssfworkbook = DataTable2HSSFWorkbook(dt);
            WriteToFile(hssfworkbook, Path);
        }

        /// <summary>
        /// 直接从datatable生成Excel文件流
        /// </summary>
        public static MemoryStream DataTable2Stream(DataTable dt)
        {
            HSSFWorkbook hssfworkbook = DataTable2HSSFWorkbook(dt);
            return WriteToMemoryStream(hssfworkbook);
        }

        /// <summary>
        /// 直接从datatable生成Excel文件
        /// </summary>
        public static void DataTable2File(DataTable dt, string Path, NameValueCollection nvc)
        {
            HSSFWorkbook hssfworkbook = DataTable2HSSFWorkbook(dt, nvc);
            WriteToFile(hssfworkbook, Path);
        }

        /// <summary>
        /// 直接从datatable生成Excel文件流
        /// </summary>
        public static MemoryStream DataTable2Stream(DataTable dt, NameValueCollection nvc)
        {
            HSSFWorkbook hssfworkbook = DataTable2HSSFWorkbook(dt, nvc);
            return WriteToMemoryStream(hssfworkbook);
        }

        private static HSSFWorkbook DataSet2HSSFWorkbook(DataSet ds)
        {
            HSSFWorkbook hssfworkbook = CreateWorkbook();
            foreach (DataTable dt in ds.Tables)
            {

                DataTable2HSSFWorkbook(hssfworkbook, dt);
            }

            return hssfworkbook;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="nvc">key为Excel中显示的字段名</param>
        private static HSSFWorkbook DataTable2HSSFWorkbook(DataTable dt, NameValueCollection nvc)
        {

            HSSFWorkbook hssfworkbook = CreateWorkbook();
            //防止excel大于6万条
            int count = Convert.ToInt32(Math.Floor(Convert.ToDouble(dt.Rows.Count / 60000)));

            int ColumnsCount = nvc.Count;

            //表头格式
            IFont font = hssfworkbook.CreateFont();
            font.Boldweight = (short)FontBoldWeight.Bold;

            ICellStyle style1 = hssfworkbook.CreateCellStyle();
            style1.SetFont(font);

            for (int i = 0; i < count; i++)
            {
                ISheet sheet1 = hssfworkbook.CreateSheet(dt.TableName.ToString() + "i");

                ///excel的表头
                IRow ColumnsRow = sheet1.CreateRow(0);
                for (int j = 0; j < ColumnsCount; j++)
                {
                    ICell Cell = ColumnsRow.CreateCell(j);
                    Cell.CellStyle = style1;
                    Cell.SetCellValue(nvc.GetKey(j));
                }


                ///填充具体数据
                ///
                for (int k = 0; k < dt.Rows.Count; k++)
                {
                    IRow row = sheet1.CreateRow(k + 1);
                    for (int j = 0; j < ColumnsCount; j++)
                    {
                        ICell Cell = row.CreateCell(j);
                        if (dt.Columns[j].DataType.Name.ToLower() == "decimal" || dt.Columns[j].DataType.Name.ToLower() == "double")
                        {
                            Cell.SetCellValue(Convert.ToDouble(dt.Rows[k][nvc[j].ToString()].ToString()));
                        }
                        else
                        {
                            Cell.SetCellValue(dt.Rows[k][nvc[j].ToString()].ToString());
                        }
                    }
                }
            }

            //生成剩余的行数
            ISheet sheetLast = hssfworkbook.CreateSheet(dt.TableName.ToString());
            ///excel的表头
            IRow ColumnsRowLast = sheetLast.CreateRow(0);
            for (int j = 0; j < ColumnsCount; j++)
            {
                ICell Cell = ColumnsRowLast.CreateCell(j);
                Cell.CellStyle = style1;
                Cell.SetCellValue(nvc.GetKey(j));
            }


            ///填充具体数据        
            for (int k = 0; k < dt.Rows.Count; k++)
            {
                IRow row = sheetLast.CreateRow(k + 1);
                for (int j = 0; j < ColumnsCount; j++)
                {
                    ICell Cell = row.CreateCell(j);
                    if (dt.Columns[j].DataType.Name.ToLower() == "decimal" || dt.Columns[j].DataType.Name.ToLower() == "double")
                    {
                        Cell.SetCellValue(Convert.ToDouble(dt.Rows[k][nvc[j].ToString()].ToString()));
                    }
                    else
                    {
                        Cell.SetCellValue(dt.Rows[k][nvc[j].ToString()].ToString());
                    }
                }

            }

            return hssfworkbook;
        }

        private static HSSFWorkbook DataTable2HSSFWorkbook(HSSFWorkbook hssfworkbook, DataTable dt)
        {

            //防止excel大于6万条
            int count = Convert.ToInt32(Math.Floor(Convert.ToDouble(dt.Rows.Count / 60000)));

            int ColumnsCount = dt.Columns.Count;

            //表头格式
            IFont font = hssfworkbook.CreateFont();
            font.Boldweight = (short)FontBoldWeight.Bold;
            ICellStyle style1 = hssfworkbook.CreateCellStyle();
            style1.BorderBottom = BorderStyle.Thin;
            style1.BorderLeft = BorderStyle.Thin;
            style1.BorderRight = BorderStyle.Thin;
            style1.BorderTop = BorderStyle.Thin;
            style1.FillPattern = FillPattern.SolidForeground;
            style1.FillForegroundColor = 44;

            style1.SetFont(font);


            for (int i = 0; i < count; i++)
            {
                ISheet sheet1 = hssfworkbook.CreateSheet(dt.TableName.ToString() + "i");

                ///excel的表头
                IRow ColumnsRow = sheet1.CreateRow(0);
                for (int j = 0; j < ColumnsCount; j++)
                {
                    ICell Cell = ColumnsRow.CreateCell(j);
                    Cell.CellStyle = style1;
                    Cell.SetCellValue(dt.Columns[j].ToString());
                }
                sheet1.CreateFreezePane(0, 1, 0, 1);

                ///填充具体数据
                ///
                for (int k = 0; k < dt.Rows.Count; k++)
                {
                    IRow row = sheet1.CreateRow(k + 1);
                    for (int j = 0; j < ColumnsCount; j++)
                    {
                        ICell Cell = row.CreateCell(j);
                        if (dt.Columns[j].DataType.Name.ToLower() == "decimal" || dt.Columns[j].DataType.Name.ToLower() == "double")
                        {
                            Cell.SetCellValue(Convert.ToDouble(dt.Rows[k][j].ToString()));
                        }
                        else
                        {
                            Cell.SetCellValue(dt.Rows[k][j].ToString());
                        }
                    }

                }
                for (int j = 0; j < ColumnsCount; j++)
                    sheet1.AutoSizeColumn(j);

            }

            //生成剩余的行数
            ISheet sheetLast = hssfworkbook.CreateSheet(dt.TableName.ToString());
            ///excel的表头
            IRow ColumnsRowLast = sheetLast.CreateRow(0);
            for (int j = 0; j < ColumnsCount; j++)
            {
                ICell Cell = ColumnsRowLast.CreateCell(j);
                Cell.CellStyle = style1;
                Cell.SetCellValue(dt.Columns[j].ToString());
            }
            sheetLast.CreateFreezePane(0, 1, 0, 1);

            ///填充具体数据        
            for (int k = 0; k < dt.Rows.Count; k++)
            {
                IRow row = sheetLast.CreateRow(k + 1);
                for (int j = 0; j < ColumnsCount; j++)
                {
                    ICell Cell = row.CreateCell(j);
                    if (dt.Columns[j].DataType.Name.ToLower() == "decimal" || dt.Columns[j].DataType.Name.ToLower() == "double")
                    {
                        Cell.SetCellValue(Convert.ToDouble(dt.Rows[k][j].ToString()));
                    }
                    else
                    {
                        Cell.SetCellValue(dt.Rows[k][j].ToString());
                    }
                }
            }
            for (int j = 0; j < ColumnsCount; j++)
                sheetLast.AutoSizeColumn(j);
            return hssfworkbook;
        }

        private static HSSFWorkbook DataTable2HSSFWorkbook(DataTable dt)
        {
            HSSFWorkbook hssfworkbook = CreateWorkbook();
            DataTable2HSSFWorkbook(hssfworkbook, dt);
            return hssfworkbook;

        }

        private static HSSFWorkbook CreateWorkbook()
        {
            HSSFWorkbook hssfworkbook = new HSSFWorkbook();
            ////create a entry of DocumentSummaryInformation
            DocumentSummaryInformation dsi = PropertySetFactory.CreateDocumentSummaryInformation();
            dsi.Company = "KP";
            hssfworkbook.DocumentSummaryInformation = dsi;

            ////create a entry of SummaryInformation
            SummaryInformation si = PropertySetFactory.CreateSummaryInformation();
            si.Subject = "";
            hssfworkbook.SummaryInformation = si;
            return hssfworkbook;
        }

        /// <summary>
        /// 直接生成Excel的文件流
        /// </summary>
        /// <param name="hssfworkbook"></param>
        /// <returns></returns>
        public static MemoryStream WriteToMemoryStream(HSSFWorkbook hssfworkbook)
        {
            MemoryStream file = new MemoryStream();
            hssfworkbook.Write(file);
            return file;
        }

        /// <summary>
        /// 直接生成文件保存
        /// </summary>
        /// <param name="hssfworkbook"></param>
        /// <param name="Path"></param>
        public static void WriteToFile(HSSFWorkbook hssfworkbook, string Path)
        {
            //Write the stream data of workbook to the root directory
            FileStream file = new FileStream(@Path, FileMode.Create);
            hssfworkbook.Write(file);
            file.Close();
        }
        #endregion

        #region 读取excel相关

        public static HSSFWorkbook ReadWorkbookFromExcelFile(string Path)
        {
            FileStream file = new FileStream(@Path, FileMode.Open, FileAccess.Read);
            HSSFWorkbook hssfworkbook = new HSSFWorkbook(file);
            return hssfworkbook;
        }
        /// <summary>
        /// 读取excel转换为datatable
        /// </summary>
        /// <param name="ExcelFileStream">fuUpload.FileContent</param>
        /// <param name="SheetIndex">读取第几个sheet</param>
        /// <param name="HeaderRowIndex">表头的位置</param>
        /// <returns></returns>
        public static DataTable RenderDataTableFromExcel(Stream ExcelFileStream, int SheetIndex, int HeaderRowIndex)
        {

            IWorkbook workbook = null;

            workbook = new HSSFWorkbook(ExcelFileStream);

            ISheet sheet = workbook.GetSheetAt(SheetIndex);

            DataTable table = new DataTable();

            IRow headerRow = sheet.GetRow(HeaderRowIndex);
            if (headerRow != null)
            {
                int cellCount = headerRow.LastCellNum;

                for (int i = headerRow.FirstCellNum; i < cellCount; i++)
                {

                    DataColumn column = new DataColumn(headerRow.GetCell(i).StringCellValue);

                    table.Columns.Add(column);
                }

                int rowCount = sheet.LastRowNum + 1;

                for (int i = (sheet.FirstRowNum + 1); i < rowCount; i++)
                {
                    IRow row = sheet.GetRow(i);

                    DataRow dataRow = table.NewRow();

                    for (int j = row.FirstCellNum; j < cellCount; j++)
                    {
                        if (row.GetCell(j) != null)
                        {
                            dataRow[j] = getCellValue(row.GetCell(j));
                        }
                    }

                    table.Rows.Add(dataRow);
                }

            }
            ExcelFileStream.Close();

            workbook = null;

            sheet = null;

            return table;

        }

        public static object getHSSFCellValue(ICell cell)
        {
            if (cell == null)
            {
                return "";
            }

            switch (cell.CellType)
            {

                case CellType.Blank:
                    return "";
                case CellType.Boolean:
                    return cell.BooleanCellValue;
                case CellType.Numeric:
                    if (DateUtil.IsCellDateFormatted(cell))
                    {
                        return cell.DateCellValue;
                    }
                    else
                    {
                        return cell.NumericCellValue;
                    }

                case CellType.String:
                    return cell.StringCellValue;
                case CellType.Error:
                    return cell.ErrorCellValue;
                case CellType.Formula://公式
                    try
                    { return cell.StringCellValue; }
                    catch
                    {
                        try
                        { return cell.NumericCellValue; }
                        catch
                        {
                            try
                            { return cell.BooleanCellValue; }
                            catch
                            {
                                return cell.ToString();
                            }
                        }
                    }
                default:
                    return cell.ToString();
            }
        }

        private static object getCellValue(ICell cell)
        {
            switch (cell.CellType)
            {

                case CellType.Blank:
                    return "[null]";
                case CellType.Boolean:
                    return cell.BooleanCellValue;
                case CellType.Numeric:
                    if (DateUtil.IsCellDateFormatted(cell))
                    {
                        return cell.DateCellValue;
                    }
                    else
                    {
                        return cell.NumericCellValue;
                    }

                case CellType.String:
                    return cell.StringCellValue;
                case CellType.Error:
                    return cell.ErrorCellValue;
                case CellType.Formula://公式
                    try
                    { return cell.StringCellValue; }
                    catch
                    {
                        try
                        { return cell.NumericCellValue; }
                        catch
                        {
                            try
                            { return cell.BooleanCellValue; }
                            catch
                            {
                                return cell.ToString();
                            }
                        }
                    }
                default:
                    return cell.ToString();
            }
        }

        /// <summary>
        /// 从Excel文件流中读取DataTable
        /// </summary>
        /// <param name="ExcelFileStream"></param>
        /// <param name="SheetName"></param>
        /// <param name="HeaderRowIndex"></param>
        /// <returns></returns>
        public static DataTable RenderDataTableFromExcel(Stream ExcelFileStream, string SheetName, int HeaderRowIndex)
        {
            try
            {
                HSSFWorkbook workbook = new HSSFWorkbook(ExcelFileStream);

                ISheet sheet = workbook.GetSheet(SheetName);

                DataTable table = new DataTable();

                IRow headerRow = sheet.GetRow(HeaderRowIndex);

                int cellCount = headerRow.LastCellNum;

                for (int i = headerRow.FirstCellNum; i < cellCount; i++)
                {

                    DataColumn column = new DataColumn(headerRow.GetCell(i).StringCellValue);

                    table.Columns.Add(column);

                }

                int rowCount = sheet.LastRowNum;

                for (int i = (sheet.FirstRowNum + 1); i < rowCount; i++)
                {
                    IRow row = sheet.GetRow(i);

                    DataRow dataRow = table.NewRow();

                    for (int j = row.FirstCellNum; j < cellCount; j++)

                        dataRow[j] = row.GetCell(j).ToString();
                }

                ExcelFileStream.Close();
                workbook = null;
                sheet = null;
                return table;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }

        }

        public static string Isbold(Stream Path)
        {
            string s = "";
            HSSFWorkbook hssw = new HSSFWorkbook(Path);
            IFont Font = null;
            ISheet sheet = hssw.GetSheetAt(0);
            IRow Row = sheet.GetRow(1);
            ICell Cell = Row.GetCell(1);

            if (Cell != null)
            {
                Font = Cell.CellStyle.GetFont(hssw);
                s = Font.Boldweight.ToString();
            }

            return s;
        }


        public static DataSet ToDataSet(Stream ExcelFileStream, int titleRowIndex = 0, params SheetParam[] sheetParams)
        {
            HSSFWorkbook workbook = new HSSFWorkbook(ExcelFileStream);

            DataSet ds = new DataSet();
            for (int i = 0; i < workbook.NumberOfSheets; i++)
            {
                Dictionary<int, int> columnMapping = new Dictionary<int, int>();
                ISheet sheet = workbook.GetSheetAt(i);
                DataTable dt = new DataTable(sheet.SheetName);
                if (null != sheetParams)
                {
                    SheetParam sp = sheetParams.ToList().Find(t => t.SheetName.Equals(sheet.SheetName, StringComparison.OrdinalIgnoreCase));
                    if (sp != null)
                    {
                        titleRowIndex = sp.TitleRowIndex;
                    }
                }
                IRow titleRow = sheet.GetRow(titleRowIndex);

                for (int t = titleRow.FirstCellNum, c = 0; t < titleRow.LastCellNum; t++, c++)
                {
                    ICell cell = titleRow.Cells[t];
                    DataColumn column = new DataColumn { ColumnName = cell.StringCellValue, DataType = typeof(string) };
                    columnMapping.Add(t, c);
                    dt.Columns.Add(column);
                }

                for (int r = titleRowIndex + 1; r < sheet.LastRowNum; r++)
                {
                    IRow row = sheet.GetRow(r);
                    DataRow dataRow = dt.NewRow();

                    foreach (ICell cell in row.Cells)
                    {
                        dataRow[columnMapping[cell.ColumnIndex]] = cell.ToString();
                    }

                    dt.Rows.Add(dataRow);

                }
                ds.Tables.Add(dt);
            }
            return ds;
        }
        #endregion
    }

    public partial class ExcelHelper
    {
        /// <summary>
        /// 单元格格式----千位符整数
        /// </summary>
        public static HSSFCellStyle CellStyleThousandSeparatorsInteger { set; get; }

        /// <summary>
        /// 单元格格式----百分号整数
        /// </summary>
        public static HSSFCellStyle CellStylePercentInteger { set; get; }

        /// <summary>
        /// 导出GP文件
        /// </summary>
        /// <param name="projectId"></param>
        /// <param name="dataSet"></param>
        /// <param name="downloadPath"></param>
        public static void ExportGp(int projectId, DataSet dataSet, ref string downloadPath)
        {
            if (downloadPath == null) throw new ArgumentNullException("downloadPath");
            string path = HttpContext.Current.Server.MapPath("~\\BOFeedBack\\QuotationModel\\");
            downloadPath = path + "GPTamplate" + DateTime.Now.ToString("yyyyMMddHHmmssms") + ".xls";
            HSSFWorkbook hssfworkbook = InitializeWorkbook(path + "GPTamplate.xls", downloadPath);
            var option = new DataTable(); //option 6
            if (dataSet.Tables[0] != null)
            {
                option = dataSet.Tables[0];
            }

            var cto = new DataTable();
            if (dataSet.Tables[1] != null)
            {
                cto = dataSet.Tables[1];
            }

            var ctos = new DataTable();
            if (dataSet.Tables[2] != null)
            {
                ctos = dataSet.Tables[2];
            }

            var svp = new DataTable();
            if (dataSet.Tables[3] != null)
            {
                svp = dataSet.Tables[3];
            }
            string strSubTotal = "";
            string strTotal = "";

            HSSFCellStyle cellStyle1 = (HSSFCellStyle)hssfworkbook.CreateCellStyle();
            cellStyle1.DataFormat = HSSFDataFormat.GetBuiltinFormat("#,##0");
            CellStyleThousandSeparatorsInteger = cellStyle1;

            HSSFCellStyle cellStyle2 = (HSSFCellStyle)hssfworkbook.CreateCellStyle();
            cellStyle2.DataFormat = HSSFDataFormat.GetBuiltinFormat("0%");
            CellStylePercentInteger = cellStyle2;

            ISheet sheet1 = hssfworkbook.GetSheetAt(0);
            {
                ISheet sheet2 = hssfworkbook.GetSheetAt(1);
                int rowIndex = 1;
                var dic = GetTemplateMap();

                #region X Config CTOs
                for (int i = 0; i < ctos.Rows.Count; i++)
                {
                    //设置表头
                    if (ctos.Rows[i]["ProductType"].ToString().Equals("8"))
                    {
                        SetTitle(sheet1, sheet2.GetRow(0), ref rowIndex, "X Config");
                    }
                    else
                    {
                        SetTitle(sheet1, sheet2.GetRow(0), ref rowIndex, "CTOs");
                    }

                    int startRow = rowIndex + 1;

                    #region 对应ctos的产品
                    var configs = cto.Select("ProductID=" + ctos.Rows[i]["ProductID"]);

                    FormatPriceGp(configs, ctos.Rows[i]);

                    foreach (var c in configs)
                    {
                        //设置单元格数据和格式
                        SetRow(c, sheet1, dic, ref rowIndex);
                    }
                    #endregion

                    //设置运费行
                    SetFreightCharge(sheet1, GetFreightChargeMap(), ref rowIndex);

                    //设置单元格数据和格式 ----主品
                    SetMainProduct(ctos.Rows[i], sheet1, GetMainProductMap(), ref rowIndex, startRow, sheet2.GetRow(1));
                    SetSubTotal(sheet1, GetSubtotalMap(), ref rowIndex, sheet2.GetRow(2), ctos.Rows[i]);
                    strSubTotal += "{0}" + rowIndex + "+";
                }
                #endregion

                //空2行
                rowIndex += 1;
                rowIndex += 1;

                #region option
                if (option.Rows.Count > 0)
                {
                    //设置表头
                    SetTitle(sheet1, sheet2.GetRow(0), ref rowIndex, "Options");
                    IRow rowTitle = sheet1.GetRow(rowIndex - 1);
                    ICell cell = rowTitle.CreateCell(25);
                    cell.CellStyle = rowTitle.GetCell(3).CellStyle;
                    cell.SetCellValue("IR");

                    int startRowOption = rowIndex + 1;

                    for (int i = 0; i < option.Rows.Count; i++)
                    {
                        var dicOption = GetTemplateMap();
                        dicOption.Add("IR", "");//option多IR列
                        //设置单元格数据和格式
                        SetRow(option.Rows[i], sheet1, dicOption, ref rowIndex);
                    }
                    SetOptionSubtotal(sheet1, GetOptionSubtotalMap(), ref rowIndex, sheet2.GetRow(2), startRowOption);
                    strSubTotal += "{0}" + rowIndex + "+";
                }
                #endregion


                //空1行
                rowIndex += 1;
                strSubTotal = strSubTotal.TrimEnd('+');
                #region Sub total of X-Series
                SetSubTotalOfXSeries(sheet1, GetSubTotalOfXSeriesMap(), ref rowIndex, sheet2.GetRow(3), strSubTotal);
                strTotal += "{0}" + rowIndex + "+";
                #endregion

                //空1行
                rowIndex += 1;

                #region SVP
                if (svp.Rows.Count > 0)
                {
                    //设置表头
                    SetTitle(sheet1, sheet2.GetRow(0), ref rowIndex, "SVP");

                    int startRowOption = rowIndex + 1;
                    for (int i = 0; i < svp.Rows.Count; i++)
                    {
                        //设置单元格数据和格式
                        SetRowSvp(svp.Rows[i], sheet1, dic, ref rowIndex);
                    }
                    SetSvpSubtotal(sheet1, GetSvpMap(), ref rowIndex, sheet2.GetRow(2), startRowOption);
                    strTotal += "{0}" + rowIndex;
                }
                #endregion

                //空1行
                rowIndex += 1;
                strTotal = strTotal.TrimEnd('+');
                #region Total Package(Includes Storage & MA)
                SetTotalPackage(sheet1, GetTotalPackageMap(), ref rowIndex, sheet2.GetRow(3), strTotal);
                #endregion

                //公式计算 -- 打开
                sheet1.ForceFormulaRecalculation = true;
            }

            WriteToFile(downloadPath, hssfworkbook);
        }

        private static void FormatPriceGp(DataRow[] configs, DataRow i)
        {
            //价格差值解决方案
            Dictionary<string, int> aryAmount = new Dictionary<string, int>();
            Dictionary<string, int> aryUnitPrice = new Dictionary<string, int>();
            Dictionary<string, int> arySuggestPrice = new Dictionary<string, int>();
            int unitPrice = 0;
            int suggestPrice = 0;
            int x = 0;
            foreach (var c in configs)
            {
                int price1 = int.Parse(Math.Floor(double.Parse(c["UnitPrice"].ToString())).ToString());
                int price2 = int.Parse(Math.Floor(double.Parse(c["SuggestPrice"].ToString())).ToString());
                unitPrice += price1 * int.Parse(c["Qty"].ToString());
                suggestPrice += price2 * int.Parse(c["Qty"].ToString());

                aryAmount.Add(c["PartNo"].ToString() + x, int.Parse(c["Qty"].ToString()));
                aryUnitPrice.Add(c["PartNo"].ToString() + x, price1);
                arySuggestPrice.Add(c["PartNo"].ToString() + x, price2);
                x++;
            }
            var remantUnitPrice = int.Parse(Math.Floor(double.Parse(i["ExecuteUnitPrice"].ToString())).ToString()) - unitPrice;
            var remantSuggestPrice = int.Parse(Math.Floor(double.Parse(i["ApprovedPrice"].ToString())).ToString()) - suggestPrice;
            Dictionary<string, int> unitPriceResult = AnalyzeRemnant(aryAmount, aryUnitPrice, remantUnitPrice);
            Dictionary<string, int> remantSuggestPriceResult = AnalyzeRemnant(aryAmount, arySuggestPrice, remantSuggestPrice);

            unitPrice = 0;
            suggestPrice = 0;
            x = 0;
            foreach (var c in configs)
            {
                c["UnitPrice"] = unitPriceResult[c["PartNo"].ToString() + x];
                c["SuggestPrice"] = remantSuggestPriceResult[c["PartNo"].ToString() + x];

                int price1 = int.Parse(Math.Floor(double.Parse(c["UnitPrice"].ToString())).ToString());
                if (price1 < 1) throw new Exception("价格拆分出现异常1，请联系系统管理员");
                int price2 = int.Parse(Math.Floor(double.Parse(c["SuggestPrice"].ToString())).ToString());
                if (price2 < 1) throw new Exception("价格拆分出现异常2，请联系系统管理员");
                unitPrice += price1 * int.Parse(c["Qty"].ToString());
                suggestPrice += price2 * int.Parse(c["Qty"].ToString());
                x++;
            }
            VerifyAnalyze(unitPrice, int.Parse(Math.Floor(double.Parse(i["ExecuteUnitPrice"].ToString())).ToString()));
            VerifyAnalyze(suggestPrice, int.Parse(Math.Floor(double.Parse(i["ApprovedPrice"].ToString())).ToString()));
        }

        public static void ExportPms(DataSet dataSet, string name, string wordType)
        {
            try
            {
                DataTable dt = dataSet.Tables[dataSet.Tables.Count - 1];
                if (dt != null && dt.Rows.Count > 0)
                {
                    //增值伙伴销售的Sales Channel为单层：G 增值伙伴销售的Sales Channel为多层：:DIZZ 直销方式的默认为：A（后续会增加逻辑）
                    var cellValue2 = dt.Rows[0]["SalesType"].ToString().Trim();
                    var cellValue4 = dt.Rows[0]["RDCID"].ToString();
                    if (wordType.Equals("Sbo") || wordType.Equals("Opr"))
                    {
                        cellValue4 = dt.Rows[0]["agentRDCID"].ToString();
                    }
                    var cellValue10 =
                        DateTime.Parse(dt.Rows[0]["ApproveValidityDateStrart"].ToString()).ToString("yyyy.MM.dd");
                    var cellValue11 =
                        DateTime.Parse(dt.Rows[0]["ApproveValidityDateEnd"].ToString()).ToString("yyyy.MM.dd");


                    string path = HttpContext.Current.Server.MapPath("~\\TemplateFiles\\");
                    string fileName = name + ".xls";
                    string downloadPath = path + fileName;
                    HSSFWorkbook hssfworkbook = InitializeWorkbook(path + "PMS.xls", downloadPath);

                    var sheet1 = hssfworkbook.GetSheetAt(0);
                    {
                        dataSet.Tables.Remove(dt);
                        int j = 1;
                        
                        foreach (DataTable table in dataSet.Tables.Cast<DataTable>().Where(table => table != null))
                        {
                            var list = table.Select().ToList();
                            var listGroupBy = list.Select(p => p["ProductID"].ToString()).Distinct();
                            ;
                            foreach (string productId in listGroupBy)
                            {
                                var l =
                                    list.Where(
                                        p =>
                                            p["ProductID"].ToString().Equals(productId) &&
                                            p["ProductType"].ToString().Equals("8")).ToArray();
                                FormatPrice(l);
                            }
                            
                            foreach (DataRow t in list)
                            {
                                IRow row = sheet1.CreateRow(j);
                                j++;

                                ICell cell0 = row.CreateCell(0);
                                cell0.SetCellValue("ZR00");

                                ICell cell1 = row.CreateCell(1);
                                cell1.SetCellValue("684");

                                ICell cell2 = row.CreateCell(2); //dischannel 销售方式对应的值
                                cell2.SetCellValue(cellValue2);

                                ICell cell3 = row.CreateCell(3); //价格文件号
                                cell3.SetCellValue(name);

                                ICell cell4 = row.CreateCell(4); //8位RDC NO (T1\SP\最终客户)
                                cell4.SetCellValue(cellValue4);

                                ICell cell5 = row.CreateCell(5); //7位SAP NO  (T1\SP\最终客户)
                                cell5.SetCellValue(t["MTM"].ToString());

                                ICell cell6 = row.CreateCell(6); //税前价格，等于执行价\1.17。保留两位小数
                                cell6.SetCellValue((double.Parse(t["Price"].ToString()) / 1.17).ToString("##.00"));

                                ICell cell7 = row.CreateCell(7); //下单数量
                                cell7.SetCellValue(t["Qty"].ToString());

                                ICell cell8 = row.CreateCell(8); //审批后价格
                                cell8.SetCellValue(Math.Floor(double.Parse(t["Price"].ToString())).ToString());

                                ICell cell9 = row.CreateCell(9);
                                cell9.SetCellValue("B");

                                ICell cell10 = row.CreateCell(10); //价格文件有效期
                                cell10.SetCellValue(cellValue10);

                                ICell cell11 = row.CreateCell(11); //价格文件有效期
                                cell11.SetCellValue(cellValue11);
                            }
                        }
                    }
                    SavePms(path, fileName, hssfworkbook);
                    IOHelper.FileDelete(path + fileName);
                }
            }
            catch (Exception ex)
            {
                LogHelper.SendErrorLog(new BSFErrorEnt { DomainID = 1, Domain = "新客户模式商机系统", ModuleID = -1, Module = "价格文件导出（PMS）", LogDate = DateTime.Now, Proposer = "" }, new { 异常信息 = ex });
            }
        }
        public static void ExportPmsTest(DataSet dataSet, string name, string wordType)
        {
            try
            {
                DataTable dt = dataSet.Tables[dataSet.Tables.Count - 1];
                if (dt != null && dt.Rows.Count > 0)
                {
                    //增值伙伴销售的Sales Channel为单层：G 增值伙伴销售的Sales Channel为多层：:DIZZ 直销方式的默认为：A（后续会增加逻辑）
                    var cellValue2 = dt.Rows[0]["SalesType"].ToString().Trim();
                    var cellValue4 = dt.Rows[0]["RDCID"].ToString();
                    if (wordType.Equals("Sbo") || wordType.Equals("Opr"))
                    {
                        cellValue4 = dt.Rows[0]["agentRDCID"].ToString();
                    }
                    var cellValue10 =
                        DateTime.Parse(dt.Rows[0]["ApproveValidityDateStrart"].ToString()).ToString("yyyy.MM.dd");
                    var cellValue11 =
                        DateTime.Parse(dt.Rows[0]["ApproveValidityDateEnd"].ToString()).ToString("yyyy.MM.dd");


                    string path = HttpContext.Current.Server.MapPath("~\\TemplateFiles\\");
                    string fileName = name + ".xls";
                    string downloadPath = path + fileName;
                    HSSFWorkbook hssfworkbook = InitializeWorkbook(path + "PMS.xls", downloadPath);

                    ISheet sheet1 = hssfworkbook.GetSheetAt(0);
                    {
                        dataSet.Tables.Remove(dt);
                        int j = 1;

                        foreach (DataTable table in dataSet.Tables.Cast<DataTable>().Where(table => table != null))
                        {
                            var list = table.Select().ToList();
                            var listGroupBy = list.Select(p => p["ProductID"].ToString()).Distinct();
                            ;
                            foreach (string productId in listGroupBy)
                            {
                                var l =
                                    list.Where(
                                        p =>
                                            p["ProductID"].ToString().Equals(productId) &&
                                            p["ProductType"].ToString().Equals("8")).ToArray();
                                FormatPrice(l);
                            }

                            foreach (DataRow t in list)
                            {
                                IRow row = sheet1.CreateRow(j);
                                j++;

                                ICell cell0 = row.CreateCell(0);
                                cell0.SetCellValue("ZR00");

                                ICell cell1 = row.CreateCell(1);
                                cell1.SetCellValue("684");

                                ICell cell2 = row.CreateCell(2); //dischannel 销售方式对应的值
                                cell2.SetCellValue(cellValue2);

                                ICell cell3 = row.CreateCell(3); //价格文件号
                                cell3.SetCellValue(name);

                                ICell cell4 = row.CreateCell(4); //8位RDC NO (T1\SP\最终客户)
                                cell4.SetCellValue(cellValue4);

                                ICell cell5 = row.CreateCell(5); //7位SAP NO  (T1\SP\最终客户)
                                cell5.SetCellValue(t["MTM"].ToString());

                                ICell cell6 = row.CreateCell(6); //税前价格，等于执行价\1.17。保留两位小数
                                cell6.SetCellValue((double.Parse(t["Price"].ToString()) / 1.17).ToString("##.00"));

                                ICell cell7 = row.CreateCell(7); //下单数量
                                cell7.SetCellValue(t["Qty"].ToString());

                                ICell cell8 = row.CreateCell(8); //审批后价格
                                cell8.SetCellValue(Math.Floor(double.Parse(t["Price"].ToString())).ToString());

                                ICell cell9 = row.CreateCell(9);
                                cell9.SetCellValue("B");

                                ICell cell10 = row.CreateCell(10); //价格文件有效期
                                cell10.SetCellValue(cellValue10);

                                ICell cell11 = row.CreateCell(11); //价格文件有效期
                                cell11.SetCellValue(cellValue11);
                            }
                        }
                    }
                    WriteToFile(path + fileName, hssfworkbook);
                    FileInfo downloadFile = new FileInfo(path + fileName);
                    HttpContext.Current.Response.Clear();
                    HttpContext.Current.Response.ClearHeaders();
                    HttpContext.Current.Response.Buffer = false;
                    HttpContext.Current.Response.ContentType = "application/octet-stream";
                    HttpContext.Current.Response.AppendHeader("Content-Disposition", "attachment;filename=" + HttpUtility.UrlEncode(downloadFile.Name, Encoding.UTF8));
                    HttpContext.Current.Response.AppendHeader("Content-Length", downloadFile.Length.ToString());
                    HttpContext.Current.Response.WriteFile(downloadFile.FullName);
                    HttpContext.Current.Response.Flush();
                    HttpContext.Current.Response.End();
                }
            }
            catch (Exception ex)
            {
                LogHelper.SendErrorLog(new BSFErrorEnt { DomainID = 1, Domain = "新客户模式商机系统", ModuleID = -1, Module = "价格文件导出（PMS）", LogDate = DateTime.Now, Proposer = "" }, new { 异常信息 = ex });
            }
        }

        /// <summary>
        /// PMS文件保存进消息队列
        /// </summary>
        /// <param name="path">路径</param>
        /// <param name="fileName">名称</param>
        /// <param name="hssfworkbook">HSSFWorkbook</param>
        private static void SavePms(string path, string fileName, HSSFWorkbook hssfworkbook)
        {
            using (FileStream file = new FileStream(path + fileName, FileMode.Create))
            {
                hssfworkbook.Write(file);
                BinaryReader r = new BinaryReader(file);
                r.BaseStream.Seek(0, SeekOrigin.Begin);    //将文件指针设置到文件开
                Pms pms = new Pms
                {
                    PmsByte = r.ReadBytes((int)r.BaseStream.Length),
                    FileName = fileName
                };
                MessageQueueHelper<Pms> helper = new MessageQueueHelper<Pms>(MSMQFormatNameString, 1);
                helper.Send(pms);
            }
        }

        /// <summary>
        /// 设置表头
        /// </summary>
        /// <param name="sheet1"></param>
        /// <param name="oRow"></param>
        /// <param name="rowIndex"></param>
        /// <param name="titleFirstCell"></param>
        private static void SetTitle(ISheet sheet1, IRow oRow, ref int rowIndex, string titleFirstCell)
        {
            //设置表头
            IRow rowTitle = sheet1.CreateRow(rowIndex);
            for (int z = 0; z < oRow.Cells.Count; z++)
            {
                ICell cell = rowTitle.CreateCell(z);
                cell.CellStyle = oRow.Cells[z].CellStyle;
                cell.SetCellValue(z == 0 ? titleFirstCell : oRow.GetCell(z).ToString());
            }
            rowIndex++;
        }

        /// <summary>
        /// 设置单元格数据和格式Thousand separators integer
        /// </summary>
        /// <param name="rows"></param>
        /// <param name="sheet1"></param>
        /// <param name="dic"></param>
        /// <param name="rowIndex"></param>
        private static void SetRow(DataRow rows, ISheet sheet1, Dictionary<string, string> dic, ref int rowIndex)
        {
            IRow row = sheet1.CreateRow(rowIndex);
            int j = 1;

            foreach (var k in dic.Keys)
            {
                #region 设置单元格数据和格式
                ICell cell = row.CreateCell(j);
                switch (k)
                {
                    case "dutyrate":
                    case "COC":
                        cell.SetCellValue(double.Parse(rows[k].ToString()));
                        break;
                    case "Qty":
                    case "Otherfee":
                    case "NONBMCRate":
                        cell.SetCellValue((int)double.Parse(rows[k].ToString()));
                        break;
                    case "MA":
                        cell.SetCellValue(double.Parse(rows[k].ToString()));
                        cell.CellStyle = CellStyleThousandSeparatorsInteger;
                        break;
                    case "UnitPrice":
                    case "SuggestPrice":
                        cell.SetCellValue(int.Parse(Math.Floor(double.Parse(rows[k].ToString())).ToString()));
                        cell.CellStyle = CellStyleThousandSeparatorsInteger;
                        break;
                    case "cost":
                        cell.SetCellValue(double.Parse(rows[k].ToString()));
                        break;
                    case "BMCadder":
                    case "NONBMCadder":
                    case "BidRefPrice":
                        cell.SetCellValue((int)double.Parse(rows[k].ToString()));
                        cell.CellStyle = CellStyleThousandSeparatorsInteger;
                        break;
                    case "NetRev":
                    case "GM":
                    case "GP":
                        cell.SetCellFormula(dic[k].FormatWith(rowIndex + 1));
                        cell.CellStyle = CellStyleThousandSeparatorsInteger;
                        break;
                    case "duty":
                    case "NONBMC":
                        cell.SetCellFormula(dic[k].FormatWith(rowIndex + 1));
                        break;
                    case "Discount":
                    case "GM%":
                    case "GP%":
                        cell.SetCellFormula(dic[k].FormatWith(rowIndex + 1));
                        cell.CellStyle = CellStylePercentInteger;
                        break;
                    case "CD":
                    case "VAT":
                        cell.SetCellValue(double.Parse(rows[k].ToString()) / 100);
                        cell.CellStyle = CellStylePercentInteger;
                        break;
                    case "IR":
                        cell.SetCellValue(rows[k].Equals(1).ToString());
                        break;
                    default:
                        cell.SetCellValue(rows[k].ToString());
                        break;
                }

                #endregion
                j++;
            }
            rowIndex++;
        }

        private static void SetRowSvp(DataRow rows, ISheet sheet1, Dictionary<string, string> dic, ref int rowIndex)
        {
            IRow row = sheet1.CreateRow(rowIndex);
            int j = 1;

            foreach (var k in dic.Keys)
            {
                #region 设置单元格数据和格式
                ICell cell = row.CreateCell(j);
                switch (k)
                {
                    case "dutyrate":
                    case "COC":
                        cell.SetCellValue(double.Parse(rows[k].ToString()));
                        cell.CellStyle = CellStyleThousandSeparatorsInteger;
                        break;
                    case "Qty":
                    case "Otherfee":
                    case "NONBMCRate":
                        cell.SetCellValue((int)double.Parse(rows[k].ToString()));
                        break;
                    case "MA":
                        cell.SetCellValue(int.Parse(Math.Floor(double.Parse(rows["SuggestPrice"].ToString())).ToString()));
                        cell.CellStyle = CellStyleThousandSeparatorsInteger;
                        break;
                    case "UnitPrice":
                    case "SuggestPrice":
                        cell.SetCellValue(int.Parse(Math.Floor(double.Parse(rows[k].ToString())).ToString()));
                        cell.CellStyle = CellStyleThousandSeparatorsInteger;
                        break;
                    case "cost":
                    case "BMCadder":
                    case "NONBMCadder":
                    case "BidRefPrice":
                        cell.SetCellValue((int)double.Parse(rows[k].ToString()));
                        cell.CellStyle = CellStyleThousandSeparatorsInteger;
                        break;
                    case "NetRev":
                    case "GM":
                    case "GP":
                        cell.SetCellFormula(dic[k].FormatWith(rowIndex + 1));
                        cell.CellStyle = CellStyleThousandSeparatorsInteger;
                        break;
                    case "duty":
                    case "NONBMC":
                        cell.SetCellFormula(dic[k].FormatWith(rowIndex + 1));
                        break;
                    case "Discount":
                    case "GM%":
                    case "GP%":
                        cell.SetCellFormula(dic[k].FormatWith(rowIndex + 1));
                        cell.CellStyle = CellStylePercentInteger;
                        break;
                    case "CD":
                    case "VAT":
                        cell.SetCellValue(double.Parse(rows[k].ToString()) / 100);
                        cell.CellStyle = CellStylePercentInteger;
                        break;
                    case "IR":
                        cell.SetCellValue(rows[k].Equals(1).ToString());
                        break;
                    default:
                        cell.SetCellValue(rows[k].ToString());
                        break;
                }

                #endregion
                j++;
            }
            rowIndex++;
        }

        /// <summary>
        /// 设置主品单元格数据和格式
        /// </summary>
        /// <param name="rows"></param>
        /// <param name="sheet1"></param>
        /// <param name="dic"></param>
        /// <param name="rowIndex"></param>
        /// <param name="startRow"></param>
        /// <param name="oRow"></param>
        private static void SetMainProduct(DataRow rows, ISheet sheet1, Dictionary<string, string> dic, ref int rowIndex, int startRow, IRow oRow)
        {
            IRow row = sheet1.CreateRow(rowIndex);
            int j = 1;
            foreach (var k in dic.Keys)
            {
                #region 设置单元格数据和格式
                ICell cell = row.CreateCell(j);
                ICell oCell = oRow.Cells[j];
                cell.CellStyle = oCell.CellStyle;
                switch (k)
                {
                    case "COC":
                        cell.SetCellValue(rows[k].ToString());
                        //cell.CellStyle = SetCellStyle(oCell, 0);
                        break;
                    case "dutyrate":
                        cell.SetCellValue(double.Parse(rows[k].ToString()));
                        //cell.CellStyle = SetCellStyle(oCell, 0);
                        break;
                    case "Qty":
                        cell.SetCellValue(1);//主品该列必须设置为1
                        break;
                    case "NONBMCRate":
                    case "MA":
                        cell.SetCellValue(double.Parse(rows[k].ToString()));
                        break;
                    case "UnitPrice":
                    case "SuggestPrice":
                        cell.SetCellFormula(dic[k].FormatWith(startRow, rowIndex - 1));
                        //cell.CellStyle = SetCellStyle(oCell, 0);
                        break;
                    case "Otherfee":
                    case "cost":
                    case "BMCadder":
                    case "NONBMCadder":
                    case "BidRefPrice":
                        cell.SetCellFormula(dic[k].FormatWith(startRow, rowIndex - 1));
                        break;
                    case "duty":
                        cell.SetCellFormula(dic[k].FormatWith(rowIndex + 1));
                        //cell.CellStyle = SetCellStyle(oCell, 0);
                        break;
                    case "NetRev":
                    case "NONBMC":
                        cell.SetCellFormula(dic[k].FormatWith(rowIndex + 1));
                        break;
                    case "GM":
                    case "GP":
                        cell.SetCellFormula(dic[k].FormatWith(rowIndex + 1));
                        break;
                    case "GM%":
                    case "GP%":
                    case "Discount":
                        cell.SetCellFormula(dic[k].FormatWith(rowIndex + 1));
                        //cell.CellStyle = SetCellStyle(oCell, 1);
                        break;
                    case "CD":
                    case "VAT":
                        cell.SetCellValue(double.Parse(rows[k].ToString()) / 100);
                        //cell.CellStyle = SetCellStyle(oCell, 1);
                        break;
                    default:
                        cell.SetCellValue(rows[k].ToString());
                        break;
                }
                #endregion
                j++;
            }
            rowIndex++;
        }

        /// <summary>
        /// 设置运费行
        /// </summary>
        /// <param name="sheet1"></param>
        /// <param name="dic"></param>
        /// <param name="rowIndex"></param>
        private static void SetFreightCharge(ISheet sheet1, Dictionary<string, string> dic, ref int rowIndex)
        {
            IRow row = sheet1.CreateRow(rowIndex);
            ICell cell0 = row.CreateCell(0);
            cell0.SetCellValue("Freight charge");
            int j = 1;
            foreach (var k in dic.Keys)
            {
                #region 设置单元格数据和格式
                ICell cell = row.CreateCell(j);
                if (k.Equals("NONBMCadder"))
                {
                    int x = int.Parse(dic[k]);
                    cell.SetCellValue(x);
                }
                else
                {
                    cell.SetCellValue("");
                }

                #endregion
                j++;
            }
            rowIndex++;
        }

        /// <summary>
        /// 设置单元格数据和格式
        /// </summary>
        /// <param name="sheet1"></param>
        /// <param name="dic"></param>
        /// <param name="rowIndex"></param>
        /// <param name="oRow"></param>
        /// <param name="rowMain"></param>
        private static void SetSubTotal(ISheet sheet1, Dictionary<string, string> dic, ref int rowIndex, IRow oRow, DataRow rowMain)
        {
            IRow row = sheet1.CreateRow(rowIndex);
            int j = 1;
            foreach (var k in dic.Keys)
            {
                #region 设置单元格数据和格式
                ICell cell = row.CreateCell(j);
                ICell oCell = oRow.Cells[j];
                //设置样式
                cell.CellStyle = oCell.CellStyle;
                switch (k)
                {
                    case "COC":
                        cell.SetCellFormula(dic[k].FormatWith(rowIndex + 1, rowIndex));
                        break;
                    case "Qty":
                        cell.SetCellValue(int.Parse(rowMain["Qty"].ToString()));
                        break;
                    case "UnitPrice"://15*14
                    case "SuggestPrice":
                    case "MA":
                    case "Otherfee":
                    case "cost":
                    case "BMCadder":
                        cell.SetCellFormula(dic[k].FormatWith(rowIndex + 1, rowIndex));
                        break;
                    case "NetRev"://15
                    case "duty":
                    case "NONBMC":
                        cell.SetCellFormula(dic[k].FormatWith(rowIndex + 1));
                        break;
                    case "GM":
                    case "GP":
                        cell.SetCellFormula(dic[k].FormatWith(rowIndex + 1));
                        break;
                    case "GM%":
                    case "GP%":
                    case "Discount":
                        cell.SetCellFormula(dic[k].FormatWith(rowIndex + 1));
                        break;
                    case "NONBMCadder"://15*13
                        cell.SetCellFormula(dic[k].FormatWith(rowIndex + 1, rowIndex - 1));
                        break;
                    case "BidRefPrice":
                        cell.SetCellFormula(dic[k].FormatWith(rowIndex + 1, rowIndex));
                        break;
                    case "PartNo":
                        cell.SetCellValue("Subtotal");
                        break;
                    case "CD":
                        cell.SetCellValue(double.Parse(rowMain["CD"].ToString()) / 100);
                        break;
                    case "VAT":
                        cell.SetCellValue(double.Parse(rowMain["VAT"].ToString()) / 100);
                        break;
                    case "dutyrate":
                        cell.SetCellValue(double.Parse(rowMain["dutyrate"].ToString()));
                        break;
                    case "NONBMCRate":
                        cell.SetCellValue(double.Parse(rowMain["NONBMCRate"].ToString()));
                        break;
                    default:
                        cell.SetCellValue(dic[k]);
                        break;
                }
                #endregion
                j++;
            }
            rowIndex++;
        }

        /// <summary>
        /// 设置单元格数据和格式
        /// </summary>
        /// <param name="sheet1"></param>
        /// <param name="dic"></param>
        /// <param name="rowIndex"></param>
        /// <param name="oRow"></param>
        /// <param name="startRow"></param>
        private static void SetOptionSubtotal(ISheet sheet1, Dictionary<string, string> dic, ref int rowIndex, IRow oRow, int startRow)
        {
            IRow row = sheet1.CreateRow(rowIndex);
            int j = 1;
            foreach (var k in dic.Keys)
            {
                #region 设置单元格数据和格式
                ICell cell = row.CreateCell(j);
                ICell oCell = oRow.Cells[j];
                //设置样式
                cell.CellStyle = oCell.CellStyle;
                switch (k)
                {
                    case "COC":
                        cell.SetCellFormula(dic[k].FormatWith(startRow, rowIndex));
                        break;
                    case "MA":
                    case "UnitPrice":
                    case "SuggestPrice":
                    case "Otherfee":
                    case "NetRev":
                    case "cost":
                    case "duty":
                    case "NONBMC":
                    case "BMCadder":
                    case "NONBMCadder":
                    case "BidRefPrice":
                        cell.SetCellFormula(dic[k].FormatWith(startRow, rowIndex));
                        break;
                    case "GM":
                    case "GP":
                        cell.SetCellFormula(dic[k].FormatWith(rowIndex + 1));
                        break;
                    case "GM%":
                    case "GP%":
                    case "Discount":
                        cell.SetCellFormula(dic[k].FormatWith(rowIndex + 1));
                        break;
                    case "PartNo":
                        cell.SetCellValue("Subtotal");
                        break;
                    case "CD":
                        break;
                    case "VAT":
                        break;
                    case "dutyrate":
                        break;
                    case "NONBMCRate":
                        break;
                    default:
                        cell.SetCellValue(dic[k]);
                        break;
                }
                #endregion
                j++;
            }
            rowIndex++;
        }

        /// <summary>
        /// 设置单元格数据和格式
        /// </summary>
        /// <param name="sheet1"></param>
        /// <param name="dic"></param>
        /// <param name="rowIndex"></param>
        /// <param name="oRow"></param>
        /// <param name="startRow"></param>
        private static void SetSvpSubtotal(ISheet sheet1, Dictionary<string, string> dic, ref int rowIndex, IRow oRow, int startRow)
        {
            IRow row = sheet1.CreateRow(rowIndex);
            int j = 1;
            foreach (var k in dic.Keys)
            {
                #region 设置单元格数据和格式
                ICell cell = row.CreateCell(j);
                ICell oCell = oRow.Cells[j];
                //设置样式
                cell.CellStyle = oCell.CellStyle;
                switch (k)
                {
                    case "COC":
                        cell.SetCellFormula(dic[k].FormatWith(startRow, rowIndex));
                        break;
                    case "NONBMCadder"://15*13
                        cell.SetCellFormula(dic[k].FormatWith(rowIndex, rowIndex));
                        break;
                    case "dutyrate":
                        break;
                    case "Qty":
                    case "UnitPrice":
                    case "SuggestPrice":
                    case "MA":
                    case "Otherfee":
                    case "NetRev":
                    case "cost":
                    case "duty":
                    case "NONBMC":
                    case "BMCadder":
                    case "BidRefPrice":
                        cell.SetCellFormula(dic[k].FormatWith(startRow, rowIndex));
                        break;
                    case "GM":
                    case "GP":
                        cell.SetCellFormula(dic[k].FormatWith(rowIndex + 1));
                        break;
                    case "GM%":
                    case "GP%":
                    case "Discount":
                        cell.SetCellFormula(dic[k].FormatWith(rowIndex + 1));
                        break;
                    default:
                        cell.SetCellValue(dic[k]);
                        break;
                }
                #endregion
                j++;
            }
            rowIndex++;
        }

        /// <summary>
        /// 设置单元格数据和格式
        /// </summary>
        /// <param name="sheet1"></param>
        /// <param name="dic"></param>
        /// <param name="rowIndex"></param>
        /// <param name="oRow"></param>
        /// <param name="strSubTotal"></param>
        private static void SetSubTotalOfXSeries(ISheet sheet1, Dictionary<string, string> dic, ref int rowIndex, IRow oRow, string strSubTotal)
        {
            IRow row = sheet1.CreateRow(rowIndex);
            ICell cell0 = row.CreateCell(0);
            cell0.SetCellValue("Sub Total");
            int j = 1;
            foreach (var k in dic.Keys)
            {
                #region 设置单元格数据和格式
                ICell cell = row.CreateCell(j);
                ICell oCell = oRow.Cells[j];
                //设置样式
                cell.CellStyle = oCell.CellStyle;
                switch (k)
                {
                    case "dutyrate":
                        break;
                    case "UnitPrice":
                        cell.SetCellFormula(dic[k].FormatWith(strSubTotal.FormatWith("E")));
                        break;
                    case "SuggestPrice":
                        cell.SetCellFormula(dic[k].FormatWith(strSubTotal.FormatWith("F")));
                        break;
                    case "MA":
                        cell.SetCellFormula(dic[k].FormatWith(strSubTotal.FormatWith("G")));
                        break;
                    case "Otherfee":
                        cell.SetCellFormula(dic[k].FormatWith(strSubTotal.FormatWith("H")));
                        break;
                    case "NetRev":
                        cell.SetCellFormula(dic[k].FormatWith(strSubTotal.FormatWith("K")));
                        break;
                    case "cost":
                        cell.SetCellFormula(dic[k].FormatWith(strSubTotal.FormatWith("L")));
                        break;
                    case "duty":
                        cell.SetCellFormula(dic[k].FormatWith(strSubTotal.FormatWith("N")));
                        break;
                    case "NONBMC":
                        cell.SetCellFormula(dic[k].FormatWith(strSubTotal.FormatWith("R")));
                        break;
                    case "BMCadder":
                        cell.SetCellFormula(dic[k].FormatWith(strSubTotal.FormatWith("U")));
                        break;
                    case "NONBMCadder":
                        cell.SetCellFormula(dic[k].FormatWith(strSubTotal.FormatWith("V")));
                        break;
                    case "BidRefPrice":
                        cell.SetCellFormula(dic[k].FormatWith(strSubTotal.FormatWith("W")));
                        break;
                    case "COC":
                        cell.SetCellFormula(dic[k].FormatWith(strSubTotal.FormatWith("Y")));
                        break;
                    case "GM":
                    case "GP":
                        cell.SetCellFormula(dic[k].FormatWith(rowIndex + 1));
                        break;
                    case "GM%":
                    case "GP%":
                    case "Discount":
                        cell.SetCellFormula(dic[k].FormatWith(rowIndex + 1));
                        break;
                    default:
                        cell.SetCellValue(dic[k]);
                        break;
                }
                #endregion
                j++;
            }
            rowIndex++;
        }

        //private static HSSFCellStyle SetCellStyle(ICell cell, int i)
        //{
        //    HSSFCellStyle cellStyle1 = (HSSFCellStyle)cell.CellStyle;
        //    switch (i)
        //    {
        //        case 0:
        //            cellStyle1.DataFormat = HSSFDataFormat.GetBuiltinFormat("#,##0");
        //            break;
        //        case 1:
        //            cellStyle1.DataFormat = HSSFDataFormat.GetBuiltinFormat("0%");
        //            break;
        //    }
        //    return cellStyle1;
        //}

        public static HSSFWorkbook InitializeWorkbook(string path, string downloadPath)
        {
            using (FileStream file = new FileStream(path, FileMode.Open, FileAccess.Read), file1 = new FileStream(downloadPath, FileMode.Create))
            {
                HSSFWorkbook hssfworkbook = new HSSFWorkbook(file);
                hssfworkbook.Write(file1);
                hssfworkbook = new HSSFWorkbook(file1);
                DocumentSummaryInformation dsi = PropertySetFactory.CreateDocumentSummaryInformation();
                dsi.Company = "x86";
                hssfworkbook.DocumentSummaryInformation = dsi;

                SummaryInformation si = PropertySetFactory.CreateSummaryInformation();
                si.Subject = "PMS File";
                hssfworkbook.SummaryInformation = si;
                return hssfworkbook;
            }
        }

        public static void WriteToFile(string path, HSSFWorkbook hssfworkbook)
        {
            using (FileStream file = new FileStream(path, FileMode.Open))
            {
                hssfworkbook.Write(file);
                file.Close();
            }
        }

        private static string MSMQFormatNameString
        {
            get
            {
                string s = ConfigurationManager.AppSettings["MSMQLMSX86PriceFile"];
                if (s == null)
                    throw new Exception("配置文件中缺少MSMQLMSX86PriceFile配置。");
                return s;
            }
        }

        /// <summary>
        /// 字典，对应CTOs主品
        /// </summary>
        /// <returns></returns>
        protected static Dictionary<string, string> GetMainProductMap()
        {
            Dictionary<string, string> dic = new Dictionary<string, string>
            {
                {"PartNo", ""},
                {"Description", ""},
                {"Qty", "1"},
                {"UnitPrice", "SUMPRODUCT(D{0}:D{1},E{0}:E{1})"},
                {"SuggestPrice", "SUMPRODUCT(D{0}:D{1},F{0}:F{1})"},
                {"MA", ""},//SUMPRODUCT(D{0}:D{1},G{0}:G{1})
                {"Otherfee", "SUMPRODUCT(D{0}:D{1},H{0}:H{1})"},
                {"CD", ""},
                {"VAT", ""},
                {"NetRev", "(F{0}-G{0}-H{0})*(1-I{0})/(1+J{0})"},
                {"cost", "SUMPRODUCT(D{0}:D{1},L{0}:L{1})"},
                {"dutyrate", ""},
                {"duty", "L{0}*M{0}"},
                {"GM", "K{0}-L{0}-U{0}-N{0}-Y{0}"},
                {"GM%", "O{0}/K{0}"},//(((Suggest Price-MA-Other fee)*(1-CD))/(1+VAT)-cost-BMC adder-duty)/((Suggest Price-MA-Other fee)*(1-CD)/(1+VAT))
                {"NONBMCRate", ""},
                {"NONBMC", "L{0}*Q{0}"},
                {"GP", "K{0}*T{0}"},
                {"GP%", "P{0}-Q{0}"},
                {"BMCadder", "SUMPRODUCT(D{0}:D{1},U{0}:U{1})"},
                {"NONBMCadder", "SUMPRODUCT(D{0}:D{1},V{0}:V{1})"},
                {"BidRefPrice", "SUMPRODUCT(D{0}:D{1},W{0}:W{1})"},
                {"Discount", "1-F{0}/W{0}"},
                {"COC", "SUM(D{0}:D{1},Y{0}:Y{1})"}
            };

            return dic;
        }

        /// <summary>
        /// 字典，对应excel的顺序   
        /// </summary>
        /// <returns></returns>
        protected static Dictionary<string, string> GetTemplateMap()
        {
            Dictionary<string, string> dic = new Dictionary<string, string>
            {
                {"PartNo", ""},
                {"Description", ""},
                {"Qty", ""},
                {"UnitPrice", ""},
                {"SuggestPrice", ""},
                {"MA", ""},
                {"Otherfee", "0"},
                {"CD", ""},
                {"VAT", ""},
                {"NetRev", "(F{0}-G{0}-H{0})*(1-I{0})/(1+J{0})"},
                {"cost", ""},
                {"dutyrate", ""},
                {"duty", "L{0}*M{0}"},
                {"GM", "K{0}-L{0}-U{0}-N{0}-Y{0}"},
                {"GM%", "O{0}/K{0}"},//(((Suggest Price-MA-Other fee)*(1-CD))/(1+VAT)-cost-BMC adder-duty)/((Suggest Price-MA-Other fee)*(1-CD)/(1+VAT))
                {"NONBMCRate", ""},
                {"NONBMC", "L{0}*Q{0}"},
                {"GP", "K{0}*T{0}"},
                {"GP%", "P{0}-Q{0}"},
                {"BMCadder", "0"},
                {"NONBMCadder", "0"},
                {"BidRefPrice", ""},
                {"Discount", "1-F{0}/W{0}"},
                {"COC", ""}
            };

            return dic;
        }

        /// <summary>
        /// 字典，对应运费
        /// </summary>
        /// <returns></returns>
        protected static Dictionary<string, string> GetFreightChargeMap()
        {
            Dictionary<string, string> dic = new Dictionary<string, string>
            {
                {"PartNo", ""},
                {"Description", ""},
                {"Qty", ""},
                {"UnitPrice", ""},
                {"SuggestPrice", ""},
                {"MA", ""},
                {"Otherfee", ""},
                {"CD", ""},
                {"VAT", ""},
                {"NetRev", ""},
                {"cost", ""},
                {"dutyrate", ""},
                {"duty", ""},
                {"GM", ""},
                {"GM%", ""},//(((Suggest Price-MA-Other fee)*(1-CD))/(1+VAT)-cost-BMC adder-duty)/((Suggest Price-MA-Other fee)*(1-CD)/(1+VAT))
                {"NONBMCRate", ""},
                {"NONBMC", ""},
                {"GP", ""},
                {"GP%", ""},
                {"BMCadder", ""},
                {"NONBMCadder", "0"},
                {"BidRefPrice", ""},
                {"Discount", ""},
                {"COC", ""}
            };

            return dic;
        }

        /// <summary>
        /// 字典，对应Subtotal
        /// </summary>
        /// <returns></returns>
        protected static Dictionary<string, string> GetSubtotalMap()
        {
            Dictionary<string, string> dic = new Dictionary<string, string>
            {
                {"PartNo", "Subtotal"},
                {"Description", ""},
                {"Qty", ""},
                {"UnitPrice", "D{0}*E{1}"},
                {"SuggestPrice", "D{0}*F{1}"},
                {"MA", "D{0}*G{1}"},
                {"Otherfee", "D{0}*H{1}"},
                {"CD", ""},
                {"VAT", ""},
                {"NetRev", "(F{0}-G{0}-H{0})*(1-I{0})/(1+J{0})"},
                {"cost", "D{0}*L{1}"},
                {"dutyrate", ""},
                {"duty", "L{0}*M{0}"},
                {"GM", "K{0}-L{0}-U{0}-N{0}-Y{0}"},
                {"GM%", "O{0}/K{0}"},
                {"NONBMCRate", ""},
                {"NONBMC", "L{0}*Q{0}"},
                {"GP", "K{0}*T{0}"},
                {"GP%", "P{0}-Q{0}"},
                {"BMCadder", "D{0}*U{1}"},
                {"NONBMCadder", "D{0}*V{1}"},
                {"BidRefPrice", "D{0}*W{1}"},
                {"Discount", "1-F{0}/W{0}"},
                {"COC", "D{0}*Y{1}"}
            };

            return dic;
        }

        /// <summary>
        /// 字典，对应Option Subtotal
        /// </summary>
        /// <returns></returns>
        protected static Dictionary<string, string> GetOptionSubtotalMap()
        {
            Dictionary<string, string> dic = new Dictionary<string, string>
            {
                {"PartNo", "Subtotal"},
                {"Description", ""},
                {"Qty", ""},
                {"UnitPrice", "SUMPRODUCT(D{0}:D{1},E{0}:E{1})"},
                {"SuggestPrice", "SUMPRODUCT(D{0}:D{1},F{0}:F{1})"},
                {"MA", "SUMPRODUCT(D{0}:D{1},G{0}:G{1})"},
                {"Otherfee", "SUMPRODUCT(D{0}:D{1},H{0}:H{1})"},
                {"CD", ""},
                {"VAT", ""},
                {"NetRev", "SUMPRODUCT(D{0}:D{1},K{0}:K{1})"},
                {"cost", "SUMPRODUCT(D{0}:D{1},L{0}:L{1})"},
                {"dutyrate", ""},
                {"duty", "SUMPRODUCT(D{0}:D{1},N{0}:N{1})"},
                {"GM", "K{0}-L{0}-U{0}-N{0}-Y{0}"},
                {"GM%", "O{0}/K{0}"},
                {"NONBMCRate", "0"},
                {"NONBMC", "SUMPRODUCT(D{0}:D{1},R{0}:R{1})"},
                {"GP", "K{0}*T{0}"},
                {"GP%", "P{0}-Q{0}"},
                {"BMCadder", "SUMPRODUCT(D{0}:D{1},U{0}:U{1})"},
                {"NONBMCadder", "SUMPRODUCT(D{0}:D{1},V{0}:V{1})"},
                {"BidRefPrice", "SUMPRODUCT(D{0}:D{1},W{0}:W{1})"},
                {"Discount", "1-F{0}/W{0}"},
                {"COC", "SUMPRODUCT(D{0}:D{1},Y{0}:Y{1})"}
            };

            return dic;
        }

        /// <summary>
        /// 字典，对应SVP
        /// </summary>
        /// <returns></returns>
        protected static Dictionary<string, string> GetSvpMap()
        {
            Dictionary<string, string> dic = new Dictionary<string, string>
            {
                {"PartNo", "Subtotal"},
                {"Description", ""},
                {"Qty", "SUM(D{0}:D{1})"},//D
                {"UnitPrice", "SUMPRODUCT(D{0}:D{1},E{0}:E{1})"},
                {"SuggestPrice", "SUMPRODUCT(D{0}:D{1},F{0}:F{1})"},
                {"MA", "SUMPRODUCT(D{0}:D{1},G{0}:G{1})"},
                {"Otherfee", "SUMPRODUCT(D{0}:D{1},H{0}:H{1})"},
                {"CD", ""},
                {"VAT", ""},
                {"NetRev", "SUMPRODUCT(D{0}:D{1},K{0}:K{1})"},
                {"cost", "SUMPRODUCT(D{0}:D{1},L{0}:L{1})"},
                {"dutyrate", ""},
                {"duty", "SUMPRODUCT(D{0}:D{1},N{0}:N{1})"},
                {"GM", "K{0}-L{0}-U{0}-N{0}-Y{0}"},
                {"GM%", "O{0}/K{0}"},
                {"NONBMCRate", ""},
                {"NONBMC", "SUMPRODUCT(D{0}:D{1},R{0}:R{1})"},
                {"GP", "K{0}*T{0}"},
                {"GP%", "P{0}-Q{0}"},
                {"BMCadder", "SUMPRODUCT(D{0}:D{1},U{0}:U{1})"},
                {"NONBMCadder", "SUMPRODUCT(D{0}:D{1},V{0}:V{1})"},
                {"BidRefPrice", "SUMPRODUCT(D{0}:D{1},W{0}:W{1})"},
                {"Discount", "1-F{0}/W{0}"},
                {"COC", "SUMPRODUCT(D{0}:D{1},Y{0}:Y{1})"}
            };

            return dic;
        }

        /// <summary>
        /// 字典，对应Sub total of X-Series
        /// </summary>
        /// <returns></returns>
        protected static Dictionary<string, string> GetSubTotalOfXSeriesMap()
        {
            Dictionary<string, string> dic = new Dictionary<string, string>
            {
                {"PartNo", "Sub total of X-Series"},
                {"Description", ""},
                {"Qty", ""},
                {"UnitPrice", "SUM({0})"},//E+ID
                {"SuggestPrice", "SUM({0})"},//F+ID
                {"MA", "SUM({0})"},//G+ID
                {"Otherfee", "SUM({0})"},//H+ID
                {"CD", ""},
                {"VAT", ""},
                {"NetRev", "SUM({0})"},//K+ID
                {"cost", "SUM({0})"},//L+ID
                {"dutyrate", ""},
                {"duty", "SUM({0})"},//N+ID
                {"GM", "K{0}-L{0}-U{0}-N{0}-Y{0}"},
                {"GM%", "O{0}/K{0}"},
                {"NONBMCRate", "0"},
                {"NONBMC", "SUM({0})"},//R+ID
                {"GP", "K{0}*T{0}"},
                {"GP%", "P{0}-Q{0}"},
                {"BMCadder", "SUM({0})"},//U+ID
                {"NONBMCadder", "SUM({0})"},//V+ID
                {"BidRefPrice", "SUM({0})"},//W+ID
                {"Discount", "1-F{0}/W{0}"},
                {"COC", "SUM({0})"}//Y+ID
            };

            return dic;
        }

        /// <summary>
        /// 字典，对应Sub total of X-Series
        /// </summary>
        /// <returns></returns>
        protected static Dictionary<string, string> GetTotalPackageMap()
        {
            Dictionary<string, string> dic = new Dictionary<string, string>
            {
                {"PartNo", "Total Package(Includes Storage & MA)"},
                {"Description", ""},
                {"Qty", ""},
                {"UnitPrice", "SUM({0})"},//E+ID
                {"SuggestPrice", "SUM({0})"},//F+ID
                {"MA", "SUM({0})"},//G+ID
                {"Otherfee", "SUM({0})"},//H+ID
                {"CD", ""},
                {"VAT", ""},
                {"NetRev", "SUM({0})"},//K+ID
                {"cost", "SUM({0})"},//L+ID
                {"dutyrate", ""},
                {"duty", "SUM({0})"},//N+ID
                {"GM", "K{0}-L{0}-U{0}-N{0}-Y{0}"},
                {"GM%", "O{0}/K{0}"},
                {"NONBMCRate", "0"},
                {"NONBMC", "SUM({0})"},//R+ID
                {"GP", "K{0}*T{0}"},
                {"GP%", "P{0}-Q{0}"},
                {"BMCadder", "SUM({0})"},//U+ID
                {"NONBMCadder", "SUM({0})"},//V+ID
                {"BidRefPrice", "SUM({0})"},//W+ID
                {"Discount", "1-F{0}/W{0}"},
                {"COC", "SUM({0})"}//Y+ID
            };

            return dic;
        }

        /// <summary>
        /// 设置单元格数据和格式
        /// </summary>
        /// <param name="sheet1"></param>
        /// <param name="dic"></param>
        /// <param name="rowIndex"></param>
        /// <param name="oRow"></param>
        /// <param name="strSubTotal"></param>
        private static void SetTotalPackage(ISheet sheet1, Dictionary<string, string> dic, ref int rowIndex, IRow oRow, string strSubTotal)
        {
            IRow row = sheet1.CreateRow(rowIndex);
            ICell cell0 = row.CreateCell(0);
            cell0.SetCellValue("Total");
            int j = 1;
            foreach (var k in dic.Keys)
            {
                #region 设置单元格数据和格式
                ICell cell = row.CreateCell(j);
                ICell oCell = oRow.Cells[j];
                //设置样式
                cell.CellStyle = oCell.CellStyle;
                switch (k)
                {
                    case "UnitPrice":
                        cell.SetCellFormula(dic[k].FormatWith(strSubTotal.FormatWith("E")));
                        break;
                    case "SuggestPrice":
                        cell.SetCellFormula(dic[k].FormatWith(strSubTotal.FormatWith("F")));
                        break;
                    case "MA":
                        cell.SetCellFormula(dic[k].FormatWith(strSubTotal.FormatWith("G")));
                        break;
                    case "Otherfee":
                        cell.SetCellFormula(dic[k].FormatWith(strSubTotal.FormatWith("H")));
                        break;
                    case "NetRev":
                        cell.SetCellFormula(dic[k].FormatWith(strSubTotal.FormatWith("K")));
                        break;
                    case "cost":
                        cell.SetCellFormula(dic[k].FormatWith(strSubTotal.FormatWith("L")));
                        break;
                    case "duty":
                        cell.SetCellFormula(dic[k].FormatWith(strSubTotal.FormatWith("N")));
                        break;
                    case "NONBMC":
                        cell.SetCellFormula(dic[k].FormatWith(strSubTotal.FormatWith("R")));
                        break;
                    case "BMCadder":
                        cell.SetCellFormula(dic[k].FormatWith(strSubTotal.FormatWith("U")));
                        break;
                    case "NONBMCadder":
                        cell.SetCellFormula(dic[k].FormatWith(strSubTotal.FormatWith("V")));
                        break;
                    case "BidRefPrice":
                        cell.SetCellFormula(dic[k].FormatWith(strSubTotal.FormatWith("W")));
                        break;
                    case "COC":
                        cell.SetCellFormula(dic[k].FormatWith(strSubTotal.FormatWith("Y")));
                        break;
                    case "GM":
                    case "GP":
                        cell.SetCellFormula(dic[k].FormatWith(rowIndex + 1));
                        break;
                    case "GM%":
                    case "GP%":
                    case "Discount":
                        cell.SetCellFormula(dic[k].FormatWith(rowIndex + 1));
                        break;
                    default:
                        cell.SetCellValue(dic[k]);
                        break;
                }
                #endregion
                j++;
            }
            rowIndex++;
        }

        public static void ExcelOutE(FileStream fileStream, string fileName)
        {
            long fileSize = fileStream.Length;
            fileStream.Close();
            HttpContext.Current.Response.Clear();
            HttpContext.Current.Response.Buffer = true;
            HttpContext.Current.Response.Charset = "GB2312";
            HttpContext.Current.Response.AppendHeader("Content-Disposition", "attachment;filename=Sheet1.xls");
            HttpContext.Current.Response.ContentEncoding = Encoding.GetEncoding("GB2312");
            HttpContext.Current.Response.ContentType = "application/ms-excel";
            HttpContext.Current.Response.AddHeader("Content-Length", fileSize.ToString());
            HttpContext.Current.Response.WriteFile(fileName);
            HttpContext.Current.Response.Flush();
            HttpContext.Current.Response.End();
        }
        /// <summary>
        /// 计算价格导出文件差值的分配方案
        /// </summary>
        /// <param name="aryAmount">key=Part No., value=数量</param>
        /// <param name="aryPrice">key=Part No., value=价格</param>
        /// <param name="remnant">价差</param>
        /// <returns></returns>
        public static Dictionary<string, int> AnalyzeRemnant(Dictionary<string, int> aryAmount, Dictionary<string, int> aryPrice, int remnant)
        {
            Dictionary<string, int> aryRet = new Dictionary<string, int>();

            //异常处理，假如差值是奇数，而数量全是偶数 TODO

            //除的尽的最大值
            {
                var result = from pair in aryAmount
                             join price in aryPrice on pair.Key equals price.Key
                             orderby Math.Abs(pair.Value), price.Value descending
                             select pair;
                foreach (var item in result)
                {
                    if (remnant % item.Value == 0)
                    {
                        //aryRet.Add(item.Key, aryPrice[item.Key] + remnant / item.Value);
                        aryPrice[item.Key] = (aryPrice[item.Key] + remnant / item.Value);
                        return aryPrice;
                    }
                }
            }
            //数量为1的
            {
                var result = from pair in aryAmount
                             join price in aryPrice on pair.Key equals price.Key
                             where Math.Abs(pair.Value) == 1
                             orderby price.Value descending
                             select pair;

                var keyValuePairs = result as KeyValuePair<string, int>[] ?? result.ToArray();
                if (keyValuePairs.Any())
                {
                    foreach (var item in keyValuePairs)
                    {
                        //aryRet.Add(item.Key, aryPrice[item.Key] + remnant / aryAmount[item.Key]);
                        aryPrice[item.Key] = (aryPrice[item.Key] + remnant / aryAmount[item.Key]);
                        return aryPrice;
                    }
                }
            }

            //凑数 TODO
            {
                var result = from pair in aryAmount
                           
                             join price in aryPrice on pair.Key equals price.Key
                             orderby Math.Abs(pair.Value), price.Value descending
                             select pair;
                foreach (var item in result)
                {
                    int remainder = remnant - item.Value;//减掉数量再取余
                    foreach (var item2 in result.Where(item2 => remainder % item2.Value == 0))
                    {
                        aryPrice[item.Key] = (aryPrice[item.Key] + 1);
                        aryPrice[item2.Key] = (aryPrice[item2.Key] + remainder / item2.Value);
                        return aryPrice;
                    }
                }
            }

            return aryRet;
        }

        public static Dictionary<string, int> FormatPrice(DataRow[] list)
        {
            //价格差值解决方案
            Dictionary<string, int> aryAmount = new Dictionary<string, int>();
            Dictionary<string, int> aryUnitPrice = new Dictionary<string, int>();
            int remantUnitPrice = 0;
            int price = 0;
            int totalPrice = 0;
            int x = 0;
            foreach (DataRow dr in list)
            {
                int price1 = int.Parse(Math.Floor(double.Parse(dr["Price"].ToString())).ToString());
                int qty = int.Parse(dr["Qty"].ToString());
                if (!dr["ISConfirmPrice"].ToString().Equals("1"))
                {
                    price += price1 * qty;

                    aryAmount.Add(dr["PartNo"].ToString() + x, qty);
                    aryUnitPrice.Add(dr["PartNo"].ToString() + x, price1);
                }
                if (totalPrice == 0)
                {
                    totalPrice = int.Parse(Math.Floor(double.Parse(dr["ApprovedPrice"].ToString())).ToString());
                }
                if (dr["ISConfirmPrice"].ToString().Equals("1"))
                {
                    totalPrice = totalPrice - (price1 * qty);
                }
                x++;
            }
            remantUnitPrice = totalPrice - price;//差价 ExecuteUnitPrice
            Dictionary<string, int> unitPriceResult = AnalyzeRemnant(aryAmount, aryUnitPrice, remantUnitPrice);
            int totalPrice1 = 0;
            x = 0;
            foreach (DataRow dr in list)
            {
                if (!dr["ISConfirmPrice"].ToString().Equals("1"))
                {
                    if (int.Parse(Math.Floor(double.Parse(dr["Price"].ToString())).ToString()) < 1) throw new Exception("价格拆分出现异常0，请联系系统管理员");
                    dr["Price"] = unitPriceResult[dr["PartNo"].ToString() + x];
                    totalPrice1 += (int.Parse(dr["Price"].ToString()) * int.Parse(dr["Qty"].ToString()));
                }
                    
                x++;
            }
            VerifyAnalyze(totalPrice, totalPrice1);
            return null;
        }

        /// <summary>
        /// 验证总价额拆分价是否相等
        /// </summary> 
        /// <param name="totalPrice"></param>
        /// <param name="price"></param>
        private static void VerifyAnalyze(int totalPrice, int price)
        {
            if (totalPrice != price)
            {
                throw new Exception("价格拆分出现异常4，请联系系统管理员");
            }
        }
    }

    public class SheetParam
    {
        public string SheetName { get; set; }

        public int TitleRowIndex { get; set; }
    }
}
