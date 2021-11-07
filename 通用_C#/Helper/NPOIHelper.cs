using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NPOI.SS.UserModel;
using NPOI.HSSF.UserModel;
using System.IO;
using NPOI.HPSF;
using NPOI.HSSF.Util;
using NPOI.SS.Util;

namespace Common
{
    public class NPOIHelper
    {
        public static int GetColumnIndexByName(ISheet sheet, int startRowIndex, int startCellIndex, string columnName)
        {
            for (int i = startCellIndex; i < 1000; i++)
            {

                if (    GetCellValue(sheet, startRowIndex, i)!=null && GetCellValue(sheet, startRowIndex, i).ToString().Trim() == columnName)
                {
                    return i;
                }
            }
            return -1;
        }
        public static void SetCellValue(ISheet sheet,int rowIndex,int cellIndex,string value)
        {
            IRow row = sheet.GetRow(rowIndex);
            if (row == null)
            {
                row = sheet.CreateRow(rowIndex);
            }

            ICell cell = row.GetCell(cellIndex);

            if (cell == null)
            {
                cell = row.CreateCell(cellIndex);
            }

            cell.SetCellValue(value);
        }
        public static void SetCellValue(ISheet sheet, int rowIndex, int cellIndex, string value,CellType t)
        {
            IRow row = sheet.GetRow(rowIndex);
            if (row == null)
            {
                row = sheet.CreateRow(rowIndex);
            }

            ICell cell = row.GetCell(cellIndex);

            if (cell == null)
            {
                cell = row.CreateCell(cellIndex);
            }

            if (t == CellType.String)
            {
                cell.SetCellValue(value);
            }
            else if(t==CellType .Numeric)
            {
                if (!string.IsNullOrEmpty(value))
                {
                    cell.SetCellValue(Convert.ToDouble(value));
                }
                else
                {
                    cell.SetCellValue(value);
                }
            }
        }

        public static string GetCellStringValue(ISheet sheet, int rowIndex, int cellIndex)
        {
            string cellString = "";

            object objCell = GetCellValue(sheet, rowIndex, cellIndex);

            if (objCell != null)
            {
                cellString = objCell.ToString();
            }

            return cellString.Trim();
;
        }


        public static object GetCellValue(ISheet sheet, int rowIndex, int cellIndex)
        {
            IRow row = sheet.GetRow(rowIndex);

            if (row == null)
            {
                return null;
            }

            ICell cell = row.GetCell(cellIndex);

            if (cell == null)
            {
                return null;
            }

            //if (DateUtil.IsCellDateFormatted(cell))
            //{
            //    return cell.DateCellValue;
            //}
            if (cell.CellType == CellType.Boolean)
            {
                return cell.BooleanCellValue;
            }
            else if (cell.CellType == CellType.Numeric)
            {
                return cell.NumericCellValue;
            }
            else if (cell.CellType == CellType.String)
            {
                return cell.StringCellValue;
            }

            return cell.StringCellValue;
 
        }

        public static HSSFWorkbook hssfworkbook;
        public static void WriteToFile(string ExcelName)
        {
            //Write a stream data of wookbook to the root directory
            FileStream file = new FileStream(@ExcelName, FileMode.Open);
            hssfworkbook.Write(file);
            file.Close();
        }
        public static void WriteToFile(FileStream file)
        {
            //Write a stream data of wookbook to the root directory          
            hssfworkbook.Write(file);
            file.Close();
        }

        public static void InitializeWorkbook(string ExcelName, string Company, string Subject)
        {
            FileStream file = new FileStream(@""+ExcelName+"", FileMode.Open, FileAccess.Read, FileShare.ReadWrite);

            hssfworkbook = new HSSFWorkbook();

            //create a entry of DocumentSummaryInformation
            DocumentSummaryInformation dsi = PropertySetFactory.CreateDocumentSummaryInformation();
            dsi.Company = Company;
            hssfworkbook.DocumentSummaryInformation = dsi;

            // create a entry of SummaryInformation

            SummaryInformation si = PropertySetFactory.CreateSummaryInformation();
            si.Subject = Subject;
            hssfworkbook.SummaryInformation = si;
        }

        public static void FillColor(ISheet sheet,int rowIndex,int cellIndex,ICellStyle  s)
        {
            try
            {
                sheet.GetRow(rowIndex).Cells[cellIndex].CellStyle = s;
            }
            catch (Exception ex)
            {
                
                throw;
            }
            
        }

        public static ICellStyle SetBackGroundColor(HSSFWorkbook workbook, short colorIndex)
        {
            ICellStyle bgColor = workbook.CreateCellStyle();
            bgColor.FillPattern = FillPattern.SolidForeground;
            bgColor.FillForegroundColor = colorIndex;
            bgColor.FillBackgroundColor = colorIndex;
            return bgColor;
        }


        public static ICellStyle SetFontStyle(HSSFWorkbook workbook)
        {
            //HSSFRow row = (HSSFRow)sheet.GetRow(rowIndex);
            //string cellvalue = GetCellStringValue(sheet, rowIndex, cellIndex);
            //HSSFCell cell = (HSSFCell)row.GetCell(cellIndex);
                ICellStyle style = workbook.CreateCellStyle();
          
                // set font center 
                style.Alignment = HorizontalAlignment.Center;

                IFont font = workbook.CreateFont();
                font.FontName = "楷体";
                font.FontHeightInPoints = 20;
                font.Color = HSSFColor.Black.Index;
                font.FontHeight = 20 * 20;
                style.SetFont(font);
            
               return style;

        }

        /// <summary>
        /// 合并单元格
        /// </summary>
        /// <param name="sheet">要合并单元格所在的sheet</param>
        /// <param name="rowstart">开始行的索引</param>
        /// <param name="rowend">结束行的索引</param>
        /// <param name="colstart">开始列的索引</param>
        /// <param name="colend">结束列的索引</param>
        public static void SetCellRangeAddress(ISheet sheet, int rowstart, int rowend, int colstart, int colend)
        {
            CellRangeAddress cellRangeAddress = new CellRangeAddress(rowstart, rowend, colstart, colend);
            sheet.AddMergedRegion(cellRangeAddress);
        }
    }
}