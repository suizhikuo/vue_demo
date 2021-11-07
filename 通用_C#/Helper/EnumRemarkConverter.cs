using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Common
{

    public class EnumConverter : JsonConverter
    {
        private readonly Dictionary<Type, Dictionary<string, string>> _enumMemberNamesPerType = new Dictionary<Type, Dictionary<string, string>>();

        public override bool CanConvert(Type objectType)
        {
            Type type = ReflectionUtils.IsNullableType(objectType) ? Nullable.GetUnderlyingType(objectType) : objectType;
            return type.IsEnum;
        }

        private Dictionary<string, string> GetEnumNameMap(Type t)
        {
            Dictionary<string, string> dictionary;
            if (!this._enumMemberNamesPerType.TryGetValue(t, out dictionary))
            {
                lock (this._enumMemberNamesPerType)
                {
                    if (this._enumMemberNamesPerType.TryGetValue(t, out dictionary))
                    {
                        return dictionary;
                    }
                    dictionary = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                    foreach (FieldInfo info in t.GetFields())
                    {
                        string str3;
                        string name = info.Name;                        
                        string second =  (from a in info.GetCustomAttributes(typeof(RemarkAttribute), true).Cast<RemarkAttribute>() select a.Remark).SingleOrDefault<string>() ?? info.Name;
                        if (dictionary.TryGetValue(second, out str3))
                        {
                            throw new Exception(string.Format("Enum name '{0}' already exists on enum '{1}'.", second, t.Name));
                        }
                        dictionary[name] = second;
                    }
                    this._enumMemberNamesPerType[t] = dictionary;
                }
            }
            return dictionary;
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            bool flag = ReflectionUtils.IsNullableType(objectType);
            Type t = flag ? Nullable.GetUnderlyingType(objectType) : objectType;
            if (reader.TokenType == JsonToken.Null)
            {
                if (!ReflectionUtils.IsNullableType(objectType))
                {
                    throw new Exception(string.Format("Cannot convert null value to {0}.", objectType.Name));
                }
                return null;
            }
            try
            {
                if (reader.TokenType == JsonToken.String)
                {
                    string str2;
                    string enumText = reader.Value.ToString();
                    if ((enumText == string.Empty) && flag)
                    {
                        return null;
                    }
                    Dictionary<string, string> enumNameMap = this.GetEnumNameMap(t);
                    if (enumText.IndexOf(',') != -1)
                    {
                        string[] strArray = enumText.Split(new char[] { ',' });
                        for (int i = 0; i < strArray.Length; i++)
                        {
                            string str3 = strArray[i].Trim();
                            strArray[i] = ResolvedEnumName(enumNameMap, str3);
                        }
                        str2 = string.Join(", ", strArray);
                    }
                    else
                    {
                        str2 = ResolvedEnumName(enumNameMap, enumText);
                    }
                    return Enum.Parse(t, str2, true);
                }
                if (reader.TokenType == JsonToken.Integer)
                {
                    TypeConverter convert = TypeDescriptor.GetConverter(reader.Value);
                    return convert.ConvertTo(reader.Value, t);
                }
            }
            catch (Exception exception)
            {

                throw new Exception(string.Format("Error converting value {0} to type '{1}'.", reader.Value.GetType().Name, t.Name), exception);
            }
            throw new Exception(string.Format("Unexpected token when parsing enum. Expected String or Integer, got {0}.", reader.TokenType));
        }

        private static string ResolvedEnumName(Dictionary<string, string> map, string enumText)
        {
            string str;
            map.TryGetValue(enumText, out str);
            return (str ?? enumText);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value == null)
            {
                writer.WriteNull();
            }
            else
            {
                Enum enum2 = (Enum)value;
                string str = enum2.ToString("G");
                if (char.IsNumber(str[0]) || (str[0] == '-'))
                {
                    writer.WriteValue(value);
                }
                else
                {
                    Dictionary<string, string> enumNameMap = this.GetEnumNameMap(enum2.GetType());
                    string[] strArray = str.Split(new char[] { ',' });
                    for (int i = 0; i < strArray.Length; i++)
                    {
                        string str3;
                        string first = strArray[i].Trim();
                        enumNameMap.TryGetValue(first, out str3);
                        str3 = str3 ?? first;
                        if (this.CamelCaseText)
                        {
                            str3 = StringUtils.ToCamelCase(str3);
                        }
                        strArray[i] = str3;
                    }
                    string str4 = string.Join(", ", strArray);
                    writer.WriteValue(str4);
                }
            }
        }

        public bool CamelCaseText { get; set; }
    }

    public class ReflectionUtils
    {
        public static bool IsNullableType(Type t)
        {
            return (t.IsGenericType && (t.GetGenericTypeDefinition() == typeof(Nullable<>)));
        }

    }

    public class StringUtils
    {
        public static string ToCamelCase(string s)
        {
            if (string.IsNullOrEmpty(s))
            {
                return s;
            }
            if (!char.IsUpper(s[0]))
            {
                return s;
            }
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < s.Length; i++)
            {
                bool flag = (i + 1) < s.Length;
                if (((i == 0) || !flag) || char.IsUpper(s[i + 1]))
                {
                    char ch = char.ToLower(s[i], CultureInfo.InvariantCulture);
                    builder.Append(ch);
                }
                else
                {
                    builder.Append(s.Substring(i));
                    break;
                }
            }
            return builder.ToString();
        }



    }

    public class DataTableConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType.Name == typeof(DataTable).Name;
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            return null;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {

            if (value == null)
            {
                writer.WriteNull();
            }
            else
            {
                DataTable dt = value as DataTable;

                List<string> header = new List<string>();
                foreach (DataColumn column in dt.Columns)
                {
                    header.Add(column.ColumnName);
                }


                List<object> rows = new List<object>();
                foreach (DataRow row in dt.Rows)
                {
                    List<string> rowData = new List<string>();
                    foreach (DataColumn column in dt.Columns)
                    {

                        if (row[column] == null && row[column] == DBNull.Value)
                        {
                            rowData.Add(string.Empty);
                        }
                        else
                        {
                            if (column.DataType == typeof(DateTime))
                            {
                                DateTime datetime = (DateTime)row[column];
                                rowData.Add(datetime.ToString(serializer.DateFormatString));
                            }
                            else
                            {
                                rowData.Add(row[column].ToString());
                            }
                        }

                    }
                    rows.Add(rowData);
                }


                var data = new { Header = header, Rows = rows };

                serializer.Serialize(writer, data);
            }
        }
    }


}
