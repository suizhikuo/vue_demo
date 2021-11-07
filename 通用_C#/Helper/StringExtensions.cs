using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Common;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;

/// <summary>
/// String 扩展函数类
/// </summary>
public static class StringExtensions
{
    /// <summary>
    /// 将数字的字符串表示形式转换为它的等效 32 位有符号整数。一个指示转换是否成功的返回值。
    /// </summary>
    /// <param name="str">包含要转换的数字的字符串</param>
    /// <param name="defaultValue">默认值</param>
    /// <returns>如果 str 转换成功，则为 (int)str；否则为 defaultValue。</returns>
    public static int ToInt(this string str, int defaultValue)
    {
        int value;
        return str.Contains(".") ? int.TryParse(str.ToDouble(0).ToString(CultureInfo.InvariantCulture), out value)  ? value : defaultValue : int.TryParse(str, out value) ? value : defaultValue;
    }

    /// <summary>
    /// 转换为双精度浮点数,并按指定的小数位4舍5入
    /// </summary>
    /// <param name="str">数据</param>
    /// <param name="digits">小数位数</param>
    public static double ToDouble(this string str, int digits)
    {
        return Math.Round(str.ToDouble(), digits);
    }
  
    /// <summary>
    /// 转换为双精度浮点数
    /// </summary>
    /// <param name="str">数据</param>
    public static double ToDouble(this string str)
    {
        if (str == null)
            return 0;
        double result;
        return double.TryParse(str, out result) ? result : 0;
    }

    public static object ToEnum(Type protocolType, string enumStringValue)
    {
        object myObject = Enum.Parse(protocolType, enumStringValue, true);
        return myObject;
    }

    /// <summary>
    /// 将数字的字符串表示形式转换为它的等效 32 位有符号整数。一个指示转换是否成功的返回值。
    /// </summary>
    /// <param name="s">包含要转换的数字的字符串</param>
    /// <returns>如果 s 转换成功，则为 (int)s；否则为 0。</returns>
    public static int ToInt(this string s)
    {
        return ToInt(s, 0);
    }

    /// <summary>
    /// 将指定 System.String 中的格式项替换为指定数组中相应 System.Object 实例的值的文本等效项。
    /// </summary>
    /// <param name="format">复合格式字符串</param>
    /// <param name="args">包含零个或多个要格式化的对象的 System.Object 数组。</param>
    /// <returns>format 的一个副本，其中格式项已替换为 args 中相应 System.Object 实例的 System.String 等效项。</returns>
    public static string FormatWith(this string format, params object[] args)
    {
        return string.Format(format, args);
    }

    /// <summary>
    /// 将指定 System.String 中的格式项替换为指定数组中相应 System.Object 实例的值的文本等效项。
    /// </summary>
    /// <param name="format">复合格式字符串</param>
    /// <param name="arg0">要格式化的 System.Object。</param>
    /// <returns>format 的一个副本，其中格式项已替换为 args 中相应 System.Object 实例的 System.String 等效项。</returns>
    public static string FormatWith(this string format, object arg0)
    {
        return string.Format(format, arg0);
    }

    /// <summary>
    /// 将指定 System.String 中的格式项替换为指定数组中相应 System.Object 实例的值的文本等效项。
    /// </summary>
    /// <param name="format">复合格式字符串</param>
    /// <param name="arg0">要格式化的 System.Object。</param>
    /// <param name="arg1">要格式化的 System.Object。</param>
    /// <returns>format 的一个副本，其中格式项已替换为 args 中相应 System.Object 实例的 System.String 等效项。</returns>
    public static string FormatWith(this string format, object arg0, object arg1)
    {
        return string.Format(format, arg0, arg1);
    }

    /// <summary>
    /// 将指定 System.String 中的格式项替换为指定数组中相应 System.Object 实例的值的文本等效项。
    /// </summary>
    /// <param name="format">复合格式字符串</param>
    /// <param name="arg0">要格式化的 System.Object。</param>
    /// <param name="arg1">要格式化的 System.Object。</param>
    /// <param name="arg2">要格式化的 System.Object。</param>
    /// <returns>format 的一个副本，其中格式项已替换为 args 中相应 System.Object 实例的 System.String 等效项。</returns>
    public static string FormatWith(this string format, object arg0, object arg1, object arg2)
    {
        return string.Format(format, arg0, arg1, arg2);
    }

    /// <summary>
    /// 将指定 System.String 中的格式项替换为指定数组中相应 System.Object 实例的值的文本等效项。
    /// </summary>
    /// <param name="format">复合格式字符串</param>
    /// <param name="arg0">要格式化的 System.Object。</param>
    /// <param name="arg1">要格式化的 System.Object。</param>
    /// <param name="arg2">要格式化的 System.Object。</param>
    /// <param name="arg3">要格式化的 System.Object。</param>
    /// <returns>format 的一个副本，其中格式项已替换为 args 中相应 System.Object 实例的 System.String 等效项。</returns>
    public static string FormatWith(this string format, object arg0, object arg1, object arg2, object arg3)
    {
        return string.Format(format, arg0, arg1, arg2, arg3);
    }

    /// <summary>
    /// 将指定 System.String 中的格式项替换为指定数组中相应 System.Object 实例的值的文本等效项。
    /// </summary>
    /// <param name="format">复合格式字符串</param>
    /// <param name="arg0">要格式化的 System.Object。</param>
    /// <param name="arg1">要格式化的 System.Object。</param>
    /// <param name="arg2">要格式化的 System.Object。</param>
    /// <param name="arg3">要格式化的 System.Object。</param>
    /// <param name="arg4">要格式化的 System.Object。</param>
    /// <returns>format 的一个副本，其中格式项已替换为 args 中相应 System.Object 实例的 System.String 等效项。</returns>
    public static string FormatWith(this string format, object arg0, object arg1, object arg2, object arg3, object arg4)
    {
        return string.Format(format, arg0, arg1, arg2, arg3, arg4);
    }

    /// <summary>
    /// 指示 Regex 构造函数中指定的正则表达式在指定的输入字符串中是否找到了匹配项。
    /// </summary>
    /// <param name="input">要搜索匹配项的字符串。</param>
    /// <param name="pattern">要匹配的正则表达式模式。</param>
    /// <returns>如果正则表达式找到匹配项，则为 true；否则，为 false。</returns>
    public static bool IsMatch(this string input, string pattern)
    {
        return input != null && Regex.IsMatch(input, pattern);
    }

    /// <summary>
    /// 封装switch
    /// </summary>
    /// <typeparam name="TOutput">要输出的对象的类型。</typeparam>
    /// <typeparam name="TInput">要枚举的对象的类型。</typeparam>
    /// <param name="input">要枚举的对象的对象。</param>
    /// <param name="inputSource">公开枚举数，该枚举数支持在指定类型的集合上进行简单迭代。</param>
    /// <param name="outputSource">要输出的公开枚举，该枚举数支持在指定类型的集合上进行简单迭代。</param>
    /// <param name="defaultOutput">要输出的default对象。</param>
    /// <returns>集合中位于枚举数当前位置的元素。</returns>
    public static TOutput Switch<TOutput, TInput>(this TInput input, IEnumerable<TInput> inputSource, IEnumerable<TOutput> outputSource, TOutput defaultOutput)
    {
        IEnumerator<TInput> inputIterator = inputSource.GetEnumerator();
        IEnumerator<TOutput> outputIterator = outputSource.GetEnumerator();

        TOutput result = defaultOutput;
        while (inputIterator.MoveNext())
        {
            if (outputIterator.MoveNext())
            {
                if (input.Equals(inputIterator.Current))
                {
                    result = outputIterator.Current;
                    break;
                }
            }
            else break;
        }
        return result;
    }

    /// <summary>
    /// 封装while
    /// </summary>
    /// <typeparam name="T">要比较的对象的类型。（引用类型）</typeparam>
    /// <param name="t">要比较的对象的对象</param>
    /// <param name="predicate">定义一组条件并确定指定对象是否符合这些条件的方法。</param>
    /// <param name="action">此委托封装的方法的参数。</param>
    public static void While<T>(this T t, Predicate<T> predicate, Action<T> action) where T : class
    {
        while (predicate(t)) action(t);
    }

    /// <summary>
    /// 指示指定的字符串是 null 还是 Empty 字符串。
    /// </summary>
    /// <param name="value">要测试的字符串。</param>
    /// <returns>如果 value 参数为 null 或空字符串 ("")，则为 true；否则为 false。</returns>
    public static bool IsNullOrEmpty(this string value)
    {
        return string.IsNullOrEmpty(value);
    }

    /// <summary>
    /// 指示指定的字符串是 null、空还是仅由空白字符组成。
    /// </summary>
    /// <param name="value">要测试的字符串。</param>
    /// <returns>如果 value 参数为 null 或 String.Empty，或者如果 value 仅由空白字符组成，则为 true。</returns>
    public static bool IsEmpty(this string value)
    {
        return string.IsNullOrWhiteSpace(value);
    }

    /// <summary>
    /// 将指定 System.String 转换为Camel命名
    /// </summary>
    /// <param name="s">要转换的字符串</param>
    /// <returns></returns>
    public static string ToCamel(this string s)
    {
        if (s.IsNullOrEmpty()) return s;
        return s[0].ToString().ToLower() + s.Substring(1);
    }

    /// <summary>
    /// 将指定 System.String（单词） 转换为Pascal命名
    /// </summary>
    /// <param name="s">要转换的字符串</param>
    /// <returns>返回值</returns>
    public static string ToPascal(this string s)
    {
        if (s.IsNullOrEmpty()) return s;
        return s[0].ToString().ToUpper() + s.Substring(1).ToLower();
    }

    /// <summary>
    /// 获取decimal数据
    /// </summary>
    /// <param name="str">源字符串</param>
    /// <param name="defaultValue">默认值</param>
    /// <returns>返回值</returns>
    public static decimal GetDecimal(this string str, decimal defaultValue)
    {
        decimal value;
        return decimal.TryParse(str, out value) ? value : defaultValue;

    }

    /// <summary>
    /// 获取decimal数据，默认值：0
    /// </summary>
    /// <param name="str">源字符串</param>
    /// <returns>返回值</returns>
    public static decimal GetDecimal(this string str)
    {
        return GetDecimal(str, 0);
    }

    /// <summary>
    /// 获取日期型数据
    /// </summary>
    /// <param name="str">源字符串</param>
    /// <param name="defaultValue">默认值</param>
    /// <returns>日期型返回值</returns>
    public static DateTime GetDateTime(this string str, DateTime defaultValue)
    {
        DateTime value;
        return DateTime.TryParse(str, out value) ? value : defaultValue;
    }

    /// <summary>
    /// 获取日期型数据，默认值：1900-1-1 0：00：00
    /// </summary>
    /// <param name="str">源字符串</param>
    /// <returns>日期型返回值</returns>
    public static DateTime GetDateTime(this string str)
    {
        return GetDateTime(str, new DateTime(1900, 1, 1));
    }

    /// <summary>
    /// 获取布尔型数据
    /// </summary>
    /// <param name="str">源字符串</param>
    /// <param name="defaultValue">默认值</param>
    /// <returns>布尔型返回值</returns>
    public static bool GetBool(this string str, bool defaultValue)
    {
        bool value;
        return bool.TryParse(str, out value) ? value : defaultValue;
    }

    /// <summary>
    /// 获取布尔型数据，默认值：false
    /// </summary>
    /// <param name="str">源字符串</param>
    /// <returns>布尔型返回值</returns>
    public static bool GetBool(this string str)
    {
        return GetBool(str, false);
    }

    /// <summary>
    /// 获取Guid型数据
    /// </summary>
    /// <param name="str">源字符串</param>
    /// <param name="defaultValue">默认值</param>
    /// <returns>Guid型返回值</returns>
    public static Guid GetGuid(this string str, Guid defaultValue)
    {
        try
        {
            Guid value = new Guid(str);
            return value;
        }
        catch
        {
            return defaultValue;
        }
    }

    /// <summary>
    /// 获取Guid型数据，默认值：Guid.Empty
    /// </summary>
    /// <param name="str">源字符串</param>
    /// <returns>Guid型返回值</returns>
    public static Guid GetGuid(this string str)
    {
        return GetGuid(str, Guid.Empty);
    }

    /// <summary>
    /// 正则表达式匹配
    /// </summary>
    /// <param name="str">源字符串</param>
    /// <param name="reg">正则表达式对象</param>
    /// <returns>返回是否匹配</returns>
    public static bool RegIsMatch(this string str, Regex reg)
    {
        return reg.IsMatch(str);
    }

    /// <summary>
    /// 正则表达式替换
    /// </summary>
    /// <param name="str">源字符串</param>
    /// <param name="reg">正则表达式对象</param>
    /// <param name="replace">替换表达式</param>
    /// <returns>替换后的字符串</returns>
    public static string RegReplace(this string str, Regex reg, string replace)
    {
        return reg.Replace(str, replace, 1);
    }

    /// <summary>
    /// 格式化输出字符串
    /// </summary>
    /// <param name="str">源字符串</param>
    /// <param name="args">参数列表</param>
    /// <returns>格式化输出结果</returns>
    public static string WithFormat(this string str, params object[] args)
    {
        return string.Format(str, args);
    }

    public static string GetAppSetting(this string str)
    {
        return ConfigurationManager.AppSettings[str] ?? "";
    }

    /// <summary>
    /// 创建目录
    /// </summary>
    /// <param name="path"></param>
    public static void CreateDirectory(this string path)
    {
        Directory.CreateDirectory(path);
    }
}
