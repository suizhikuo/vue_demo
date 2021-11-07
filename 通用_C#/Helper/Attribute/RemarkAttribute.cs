using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Reflection;
using System.Data;

namespace Common
{
    /// <summary>
    /// 枚举值注释属性
    /// </summary>
    public class TestRemarkAttribute : Attribute
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="remark"></param>
        public TestRemarkAttribute( params string[] a)
        {
            _remark = a;
        }

        private string[]  _remark;
        /// <summary>
        /// 备注
        /// </summary>
        public string[] Remark
        {
            get { return _remark; }
            set { _remark = value; }
        }

        public static string GetEnumRemark(System.Enum enumImpl, string flag)
        {
            {
                string names = string.Empty;
                Type type = enumImpl.GetType();
                string[] fieldNames = enumImpl.ToString().Split(',');
                for (int i = 0; i < fieldNames.Length; i++)
                {
                    FieldInfo fd = type.GetField(fieldNames[i].Trim());
                    object[] attrs = fd.GetCustomAttributes(typeof(TestRemarkAttribute), false);
                    string name = string.Empty;
                    foreach (TestRemarkAttribute attr in attrs)
                    {
                        foreach (string str in attr.Remark)
                        {
                            name = str;
                        }
                        
                    }
                    names += name;
                    if (i < fieldNames.Length - 1)
                    {
                        names += ",";
                    }
                }

                return names;
            }
        }
    }

    /// <summary>
    /// 枚举值注释属性
    /// </summary>
    public class RemarkAttribute : Attribute
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="remark"></param>
        public RemarkAttribute(string remark)
        {
            _remark = remark;
        }

        private string _remark;
        /// <summary>
        /// 备注
        /// </summary>
        public string Remark
        {
            get { return _remark; }
            set { _remark = value; }
        }

        private static Hashtable _cache = Hashtable.Synchronized(new Hashtable());
        /// <summary>
        /// 得到枚举值的注释
        /// </summary>
        /// <param name="enumImpl"></param>
        /// <returns></returns>
        public static string GetEnumRemark(System.Enum enumImpl)
        {
            if (_cache.ContainsKey(enumImpl))
                return (string)_cache[enumImpl];
            else
            {
                string names = string.Empty;
                Type type = enumImpl.GetType();
                string[] fieldNames = enumImpl.ToString().Split(',');
                for (int i = 0; i < fieldNames.Length; i++)
                {
                    FieldInfo fd = type.GetField(fieldNames[i].Trim());
                    if (fd != null)
                    {
                        object[] attrs = fd.GetCustomAttributes(typeof(RemarkAttribute), false);

                        string name = string.Empty;
                        foreach (RemarkAttribute attr in attrs)
                        {
                            name = attr.Remark;
                        }
                        names += name;
                        if (i < fieldNames.Length - 1)
                        {
                            names += ",";
                        }
                    }
                }
                _cache.Add(enumImpl, names);
                return names;
            }
        }

        /// <summary>
        /// 获取枚举类型的所有注释
        /// </summary>
        /// <param name="enumValue"></param>
        /// <returns></returns>
        public static DataTable GetEnumAllRemark(Type enumValue)
        {
            int id = 0;
            FieldInfo fd = null;
            object[] attrs = null;
            FieldInfo[] fields = null;
            string name = string.Empty;

            DataTable table = new DataTable();
            table.Columns.AddRange(new DataColumn[] { new DataColumn("ID", typeof(int)), new DataColumn("NAME", typeof(string)) });

            if (enumValue.IsEnum)
            {
                fields = enumValue.GetFields();
                for (int i = 0; i < fields.Length; i++)
                {
                    fd = fields[i];
                    attrs = fd.GetCustomAttributes(typeof(RemarkAttribute), false);
                    if (attrs != null)
                    {
                        foreach (RemarkAttribute attr in attrs)
                        {
                            name = attr.Remark;
                            id = (int)fd.GetValue(Activator.CreateInstance(fd.FieldType));
                            table.Rows.Add(id, name);
                        }
                    }
                }
            }

            table.AcceptChanges();
            return table;
        }
    }
}

