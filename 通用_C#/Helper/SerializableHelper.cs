using System;
using System.Data;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using System.Xml.Schema;
using System.Runtime.Serialization.Formatters.Binary;
using Newtonsoft.Json;
using System.Reflection;
using System.Runtime.Serialization;

namespace Common
{
    public static class SerializableHelper
    {
        //将对象序列化为xml
        public  static  string SerializableObjectToXml(object o, Type t)
        {
            XmlSerializer xs = new XmlSerializer(t);
            MemoryStream mem = new MemoryStream();
            xs.Serialize(mem, o);

            byte[] bytData = mem.ToArray();
            MemoryStream msTmp = new MemoryStream();

            ICSharpCode.SharpZipLib.GZip.GZipOutputStream gzip = new ICSharpCode.SharpZipLib.GZip.GZipOutputStream(msTmp);
            gzip.SetLevel(9);

            gzip.Write(bytData, 0, bytData.Length);
            gzip.Flush();
            gzip.Close();
            bytData = null;
            byte[] compressedData = (byte[])msTmp.ToArray();

            msTmp.Close();
            mem.Close();
            return Convert.ToBase64String(compressedData);
        }
        //将xml反序列化为对象
        public static  object DeserializeXmlToObject(string xmlString, Type t)
        {
            if (string.IsNullOrEmpty(xmlString))
            {
                return null;
            }

            MemoryStream msZip = new MemoryStream(Convert.FromBase64String(xmlString));
            MemoryStream ms = new MemoryStream();

            ICSharpCode.SharpZipLib.GZip.GZipInputStream bi = new ICSharpCode.SharpZipLib.GZip.GZipInputStream(msZip);
            byte[] bytData = new byte[4096];
            int size = 0;
            while ((size = bi.Read(bytData, 0, bytData.Length)) > 0)
            {
                ms.Write(bytData, 0, size);
            }

            ms.Seek(0, SeekOrigin.Begin);

            XmlSerializer xs = new XmlSerializer(t);
            object o = xs.Deserialize(ms);

            bi.Close();
            ms.Close();
            msZip.Close();
            return o;
        }

        /// <summary>
        /// 将对象序列化成字符串
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <returns></returns>
        public static string SerializableObjectToStrong<T>(T t)
        {
            if (t == null) return null;
            IFormatter ifm = new BinaryFormatter();
            string strNew = string.Empty;
            using (MemoryStream ms = new MemoryStream())
            {
                ifm.Serialize(ms, t);
                byte[] byt = new byte[ms.Length];
                byt = ms.ToArray();
                strNew = Convert.ToBase64String(byt);
                ms.Flush();
            }
            return strNew;
        }

        /// <summary>
        /// 将对象字符串反序列化成对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="objectString"></param>
        /// <returns></returns>
        public static T DeserializeStringToObject<T>(string objectString)
        {
            if (string.IsNullOrEmpty(objectString)) return default(T);
            byte[] byt = Convert.FromBase64String(objectString);
            using (Stream smNew = new MemoryStream(byt, 0, byt.Length))
            {
                IFormatter fmNew = new BinaryFormatter();
                return (T)fmNew.Deserialize(smNew);
            }
        }

        /// <summary>
        /// 通过比较字符串长度验证是否压缩成功
        /// </summary>
        /// <param name="srcLen">原始流长度</param>
        /// <param name="zipBytes">压缩后的字节组</param>
        /// <returns></returns>
        private static bool ZipTest(long srcLen, byte[] zipBytes)
        {
            bool ret = true;

            MemoryStream msZip = new MemoryStream(zipBytes);

            System.IO.Compression.DeflateStream gZip = new System.IO.Compression.DeflateStream(msZip, System.IO.Compression.CompressionMode.Decompress);

            MemoryStream ms = new MemoryStream();
            int n;
            byte[] bytes = new byte[10000];//一次最多读取10000字节,有时第一次能读到，有时要第二次才能读到。
            n = gZip.Read(bytes, 0, bytes.Length);
            if (n != 0)
            {
                ms.Write(bytes, 0, n);
            }
            while ((n = gZip.Read(bytes, 0, bytes.Length)) != 0)
            {
                ms.Write(bytes, 0, n);
            }

            if ((srcLen - ms.Length) > 1)//只差一个末尾结束标记认可为成功压缩，补完即可。
            {
                ret = false;
            }

            bytes = null;
            ms.Close();
            gZip.Close();

            return ret;
        }
    }

    public class SerializableDictionary<TKey, TValue>
           : Dictionary<TKey, TValue>, IXmlSerializable
    {
        #region IXmlSerializable 成员

        public XmlSchema GetSchema()
        {
            return null;
        }

        /// <summary>
        /// 反序列化
        /// </summary>
        /// <param name="reader"></param>
        public void ReadXml(XmlReader reader)
        {
            XmlSerializer keySerializer = new XmlSerializer(typeof(TKey));
            XmlSerializer valueSerializer = new XmlSerializer(typeof(TValue));
            if (reader.IsEmptyElement || !reader.Read())
            {
                return;
            }
            while (reader.NodeType != XmlNodeType.EndElement)
            {
                reader.ReadStartElement("item");

                reader.ReadStartElement("key");
                TKey key = (TKey)keySerializer.Deserialize(reader);
                reader.ReadEndElement();
                reader.ReadStartElement("value");
                TValue value = (TValue)valueSerializer.Deserialize(reader);
                reader.ReadEndElement();

                reader.ReadEndElement();
                reader.MoveToContent();
                this.Add(key, value);
            }
            reader.ReadEndElement();
        }

        /// <summary>
        /// 序列化
        /// </summary>
        /// <param name="writer"></param>
        public void WriteXml(XmlWriter writer)
        {
            XmlSerializer keySerializer = new XmlSerializer(typeof(TKey));
            XmlSerializer valueSerializer = new XmlSerializer(typeof(TValue));

            foreach (TKey key in this.Keys)
            {
                writer.WriteStartElement("item");

                writer.WriteStartElement("key");
                keySerializer.Serialize(writer, key);
                writer.WriteEndElement();
                writer.WriteStartElement("value");
                valueSerializer.Serialize(writer, this[key]);
                writer.WriteEndElement();

                writer.WriteEndElement();
            }
        }

        #endregion
    }
}
