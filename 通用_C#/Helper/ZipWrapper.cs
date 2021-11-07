using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Helper
{
    public class ZipWrapper
    {
        public static bool NeedCompress(string str)
        {
            //需要压缩文本最小长度
            return Encoding.UTF8.GetBytes(str).Length > 1024;
        }

        public static string Compress(string str)
        {
            //因输入的字符串不是Base64所以转换为Base64,因为HTTP如果不传递Base64则会发生http 400错误

            return Convert.ToBase64String(Compress(Convert.FromBase64String(Convert.ToBase64String(Encoding.UTF8.GetBytes(str)))));
        }

        public static string Decompress(string str)
        {
            return Encoding.UTF8.GetString(Decompress(Convert.FromBase64String(str)));
        }

        public static byte[] Compress(byte[] bytes)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                GZipStream Compress = new GZipStream(ms, CompressionMode.Compress);

                Compress.Write(bytes, 0, bytes.Length);

                Compress.Close();

                return ms.ToArray();

            }
        }

        public static byte[] Decompress(Byte[] bytes)
        {
            using (MemoryStream tempMs = new MemoryStream())
            {
                using (MemoryStream ms = new MemoryStream(bytes))
                {
                    GZipStream Decompress = new GZipStream(ms, CompressionMode.Decompress);

                    Decompress.CopyTo(tempMs);

                    Decompress.Close();

                    return tempMs.ToArray();
                }
            }
        }
    }

}
