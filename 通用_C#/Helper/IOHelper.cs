/* IOHelper.cs
*
* 功 能： IOHelper
* 类 名： IOHelper
*
* Ver    变更日期         负责人  变更内容
* ───────────────────────────────────
* V0.01  2013.12.1       金鑫    --
*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace Common
{
    public class IOHelper
    {
        /// <summary>
        /// 返回文件是否存在
        /// </summary>
        /// <param name="filename">文件名</param>
        /// <returns>是否存在</returns>
        public static bool FileExists(string filename)
        {
            return File.Exists(filename);
        }

        /// <summary>
        /// 删除文件
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public static bool FileDelete(string filename)
        {
            if (!FileExists(filename)) return false;
            File.Delete(filename);
            return true;
        }

        /// <summary>
        /// 删除文件
        /// </summary>
        /// <param name="filename">文件路径组</param>
        /// <returns></returns>
        public static bool FileDelete(string[] filenames)
        {
            if (filenames == null) return false;
            foreach (string filename in filenames)
            {
                if (FileExists(filename))
                    File.Delete(filename);
            }
            return true;
        }

        /// <summary>
        /// 返回文件夹是否存在
        /// </summary>
        /// <returns></returns>
        public static bool DirectoryExists(string path)
        {
            return Directory.Exists(path);
        }

        /// <summary>
        /// 建立文件夹
        /// </summary>
        /// <param name="path">文件夹路径</param>
        /// <param name="isAbsolutelyPath">文件夹路径 是否是绝对路径</param>
        /// <returns></returns>
        public static bool CreateDir(string path, bool isAbsolutelyPath)
        {
            if (!isAbsolutelyPath)
                path = Utility.GetMapPath(path);

            if (!DirectoryExists(path))
                return MakeSureDirectoryPathExists(path);
            else return true;
        }

        /// <summary>
        /// 保存上载文件
        /// </summary>
        /// <param name="path"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static bool SaveAsFile(string path, string fileName, HttpPostedFileBase file, ref string errmesg)
        {
            try
            {
                errmesg = string.Empty;
                path = Utility.GetMapPath(path);
                if (!DirectoryExists(path))
                {
                    if (MakeSureDirectoryPathExists(path))
                        file.SaveAs(Path.Combine(path, fileName));
                    else return false;
                }
                file.SaveAs(Path.Combine(path, fileName));
                return true;
            }
            catch (Exception ex)
            {
                errmesg = ex.Message;
                return false;
            }
        }

        /// <summary>
        /// 保存上载文件
        /// </summary>
        /// <param name="path"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static bool SaveAsFile(string path, string fileName, HttpPostedFile file, ref string errmesg)
        {
            try
            {
                errmesg = string.Empty;
                path = Utility.GetMapPath(path);
                if (!DirectoryExists(path))
                {
                    if (MakeSureDirectoryPathExists(path))
                        file.SaveAs(Path.Combine(path, fileName));
                    else return false;
                }
                file.SaveAs(Path.Combine(path, fileName));
                return true;
            }
            catch (Exception ex)
            {
                errmesg = ex.Message;
                return false;
            }
        }

        /// <summary>
        /// 输出硬盘文件，提供下载
        /// </summary>  
        /// <param name="fileName">下载文件名</param>
        /// <param name="path">带文件名下载路径</param>
        /// <param name="speed">每秒允许下载的字节数</param>
        /// <returns>返回是否成功</returns>
        /// <remarks>
        /// 调用说明：
        /// 如：bool loadif=ResponseFile(fileName, fullPath, 10000);
        /// 当speed设置为10000时：下载速度为12KB左右
        /// </remarks>
        public static void DownLoadFile(string fileName, string path, long speed)
        {
            HttpRequest _Request = System.Web.HttpContext.Current.Request;
            HttpResponse _Response = System.Web.HttpContext.Current.Response;
            try
            {
                FileStream myFile = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                BinaryReader br = new BinaryReader(myFile);
                try
                {
                    _Response.AddHeader("Accept-Ranges", "bytes");
                    _Response.Buffer = false;
                    long fileLength = myFile.Length;
                    long startBytes = 0;

                    int pack = 10240; //10K bytes
                    //int sleep = 200;   //每秒5次   即5*10K bytes每秒
                    int sleep = (int)Math.Floor((decimal)1000 * pack / speed) + 1;
                    if (_Request.Headers["Range"] != null)
                    {
                        _Response.StatusCode = 206;
                        string[] range = _Request.Headers["Range"].Split(new char[] { '=', '-' });
                        startBytes = Convert.ToInt64(range[1]);
                    }
                    _Response.AddHeader("Content-Length", (fileLength - startBytes).ToString());
                    if (startBytes != 0)
                    {
                        _Response.AddHeader("Content-Range", string.Format(" bytes {0}-{1}/{2}", startBytes, fileLength - 1, fileLength));
                    }
                    _Response.AddHeader("Connection", "Keep-Alive");
                    _Response.ContentType = "application/octet-stream";
                    _Response.Charset = "UTF-8";
                    _Response.ContentEncoding = Encoding.UTF8;
                    _Response.AddHeader("Content-Disposition", "attachment;filename=" + HttpUtility.UrlEncode(fileName, Encoding.UTF8));

                    br.BaseStream.Seek(startBytes, SeekOrigin.Begin);
                    int maxCount = (int)Math.Floor((decimal)(fileLength - startBytes) / pack) + 1;

                    for (int i = 0; i < maxCount; i++)
                    {
                        if (_Response.IsClientConnected)
                        {
                            _Response.BinaryWrite(br.ReadBytes(pack));
                            Thread.Sleep(sleep);
                        }
                        else
                        {
                            i = maxCount;
                        }
                    }
                    _Response.End();
                }
                catch (Exception er)
                {
                    throw er;
                }
                finally
                {
                    br.Close();
                    myFile.Close();
                }
            }
            catch (Exception err)
            {
                throw err;
            }
        }

        /// <summary>
        /// 从共享目录下载文件
        /// </summary>
        /// <param name="FilePath">文件路径</param>
        /// <returns></returns>
        public static void DownLoadFile(string FilePath)
        {
            HttpRequest _Request = System.Web.HttpContext.Current.Request;
            HttpResponse _Response = System.Web.HttpContext.Current.Response;
            try
            {
                byte[] filebuffer = File.ReadAllBytes(FilePath);
                string[] strs = FilePath.Split('\\');
                string fileName = strs[strs.Length - 1];
                _Response.Clear();
                _Response.AddHeader("Content-Disposition", "attachment;filename=" + HttpUtility.UrlEncode(fileName, System.Text.Encoding.UTF8));
                _Response.BinaryWrite(filebuffer);
                _Response.End();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 创建目录
        /// </summary>
        /// <param name="name">名称</param>
        /// <returns>创建是否成功</returns>
        [DllImport("dbgHelp", SetLastError = true)]
        public static extern bool MakeSureDirectoryPathExists(string name);


        /// <summary>
        /// 备份文件
        /// </summary>
        /// <param name="sourceFileName">源文件名</param>
        /// <param name="destFileName">目标文件名</param>
        /// <param name="overwrite">当目标文件存在时是否覆盖</param>
        /// <returns>操作是否成功</returns>
        public static bool BackupFile(string sourceFileName, string destFileName, bool overwrite)
        {
            if (!System.IO.File.Exists(sourceFileName))
                throw new FileNotFoundException(sourceFileName + "文件不存在！");

            if (!overwrite && System.IO.File.Exists(destFileName))
                return false;

            try
            {
                System.IO.File.Copy(sourceFileName, destFileName, true);
                return true;
            }
            catch (Exception e)
            {
                throw e;
            }
        }


        /// <summary>
        /// 备份文件,当目标文件存在时覆盖
        /// </summary>
        /// <param name="sourceFileName">源文件名</param>
        /// <param name="destFileName">目标文件名</param>
        /// <returns>操作是否成功</returns>
        public static bool BackupFile(string sourceFileName, string destFileName)
        {
            return BackupFile(sourceFileName, destFileName, true);
        }


        /// <summary>
        /// 恢复文件
        /// </summary>
        /// <param name="backupFileName">备份文件名</param>
        /// <param name="targetFileName">要恢复的文件名</param>
        /// <param name="backupTargetFileName">要恢复文件再次备份的名称,如果为null,则不再备份恢复文件</param>
        /// <returns>操作是否成功</returns>
        public static bool RestoreFile(string backupFileName, string targetFileName, string backupTargetFileName)
        {
            try
            {
                if (!System.IO.File.Exists(backupFileName))
                    throw new FileNotFoundException(backupFileName + "文件不存在！");

                if (backupTargetFileName != null)
                {
                    if (!System.IO.File.Exists(targetFileName))
                        throw new FileNotFoundException(targetFileName + "文件不存在！无法备份此文件！");
                    else
                        System.IO.File.Copy(targetFileName, backupTargetFileName, true);
                }
                System.IO.File.Delete(targetFileName);
                System.IO.File.Copy(backupFileName, targetFileName);
            }
            catch (Exception e)
            {
                throw e;
            }
            return true;
        }

        public static bool RestoreFile(string backupFileName, string targetFileName)
        {
            return RestoreFile(backupFileName, targetFileName, null);
        }

        /// <summary>
        /// 转换长文件名为短文件名
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="repstring"></param>
        /// <param name="leftnum"></param>
        /// <param name="rightnum"></param>
        /// <param name="charnum"></param>
        /// <returns></returns>
        public static string ConvertSimpleFileName(string fullname, string repstring, int leftnum, int rightnum, int charnum)
        {
            string simplefilename = "", leftstring = "", rightstring = "", filename = "";
            string extname = GetFileExtName(fullname);

            if (string.IsNullOrEmpty(extname))
                throw new Exception("字符串不含有扩展名信息");

            int filelength = 0, dotindex = 0;

            dotindex = fullname.LastIndexOf('.');
            filename = fullname.Substring(0, dotindex);
            filelength = filename.Length;
            if (dotindex > charnum)
            {
                leftstring = filename.Substring(0, leftnum);
                rightstring = filename.Substring(filelength - rightnum,      rightnum);
                if (repstring == "" || repstring == null)
                    simplefilename = leftstring + rightstring + "." + extname;
                else
                    simplefilename = leftstring + repstring + rightstring + "." + extname;
            }
            else
                simplefilename = fullname;

            return simplefilename;
        }

        /// <summary>
        /// 获取指定文件的扩展名
        /// </summary>
        /// <param name="fileName">指定文件名</param>
        /// <returns>扩展名</returns>
        public static string GetFileExtName(string fileName)
        {
            if (string.IsNullOrEmpty(fileName) || fileName.IndexOf('.') <= 0)
                return "";

            fileName = fileName.ToLower().Trim();

            return fileName.Substring(fileName.LastIndexOf('.'), fileName.Length - fileName.LastIndexOf('.'));
        }

        public static void DownLoadFileByByte(byte[] buffers, HttpResponse response, string  fileName)
        {
            if (buffers == null)
            {
                buffers = new byte[0];
            }

            response.Clear();
            response.AddHeader("Content-Disposition", "attachment;filename=" + HttpUtility.UrlEncode(fileName, Encoding.UTF8));
            response.AppendHeader("Content-Length ", buffers.Length.ToString());
            response.ContentType = "application/octet-stream";
            response.BinaryWrite(buffers);
            response.Flush(); 
            //response.End();
            //response.Close();
        }

        public static void DownLoadFileByByteEX(byte[] buffers, HttpResponse response, string fileName, string filePath)
        {           
            response.Clear();
            response.ClearContent();
            response.AddHeader("Content-Disposition", "attachment;filename=" + HttpUtility.UrlEncode(fileName, Encoding.UTF8));
            response.AppendHeader("Content-Length ", buffers.Length.ToString());
            response.ContentType = "application/pdf";
            response.TransmitFile(filePath);
            //response.Flush();
            //response.End();
        }
    }
}
