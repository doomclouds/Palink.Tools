using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using Palink.Tools.Extensions.Enumeration;

namespace Palink.Tools.Extensions
{
    /// <summary>
    /// 转换器扩展类
    /// </summary>
    public static class ConvertExtensions
    {
        /// <summary>
        /// string转int
        /// </summary>
        /// <param name="input">输入</param>
        /// <param name="defaultNum">转换失败默认</param>
        /// <returns></returns>
        public static int TryToInt(this object input, int defaultNum = 0)
        {
            if (input == null)
                return defaultNum;

            return int.TryParse(input.ToString(), out var num) ? num : defaultNum;
        }

        /// <summary>
        /// string转long
        /// </summary>
        /// <param name="input">输入</param>
        /// <param name="defaultNum">转换失败默认</param>
        /// <returns></returns>
        public static long TryToLong(this object input, long defaultNum = 0)
        {
            if (input == null)
                return defaultNum;

            return long.TryParse(input.ToString(), out var num) ? num : defaultNum;
        }

        /// <summary>
        /// string转double
        /// </summary>
        /// <param name="input">输入</param>
        /// <param name="defaultNum">转换失败默认值</param>
        /// <returns></returns>
        public static double TryToDouble(this object input, double defaultNum = 0)
        {
            if (input == null)
                return defaultNum;

            return double.TryParse(input.ToString(), out var num) ? num : defaultNum;
        }

        /// <summary>
        /// string转decimal
        /// </summary>
        /// <param name="input">输入</param>
        /// <param name="defaultNum">转换失败默认值</param>
        /// <returns></returns>
        public static decimal TryToDecimal(this object input, decimal defaultNum = 0)
        {
            if (input == null)
                return defaultNum;

            return decimal.TryParse(input.ToString(), out var num) ? num : defaultNum;
        }

        /// <summary>
        /// string转decimal
        /// </summary>
        /// <param name="input">输入</param>
        /// <param name="defaultNum">转换失败默认值</param>
        /// <returns></returns>
        public static float TryToFloat(this object input, float defaultNum = 0)
        {
            if (input == null)
                return defaultNum;

            return float.TryParse(input.ToString(), out var num) ? num : defaultNum;
        }

        /// <summary>
        /// string转bool
        /// </summary>
        /// <param name="input">输入</param>
        /// <param name="falseVal"></param>
        /// <param name="defaultBool">转换失败默认值</param>
        /// <param name="trueVal"></param>
        /// <returns></returns>
        public static bool TryToBool(this object input, bool defaultBool = false,
            string trueVal = "1", string falseVal = "0")
        {
            if (input == null)
                return defaultBool;

            var str = input.ToString();
            if (bool.TryParse(str, out var outBool))
                return outBool;

            outBool = defaultBool;

            if (trueVal == str)
                return true;
            if (falseVal == str)
                return false;

            return outBool;
        }

        /// <summary>
        /// 值类型转string
        /// </summary>
        /// <param name="inputObj">输入</param>
        /// <param name="defaultStr">转换失败默认值</param>
        /// <returns></returns>
        public static string TryToString(this ValueType inputObj, string defaultStr = "")
        {
            var output = inputObj.IsNull() ? defaultStr : inputObj.ToString();
            return output;
        }

        /// <summary>
        /// 字符串转时间
        /// </summary>
        /// <param name="inputStr">输入</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns></returns>
        public static DateTime TryToDateTime(this string inputStr, DateTime defaultValue)
        {
            if (inputStr.IsNullOrEmpty())
                return defaultValue;

            return DateTime.TryParse(inputStr, out var outPutDateTime) ? outPutDateTime
                : defaultValue;
        }

        /// <summary>
        /// 字符串转时间
        /// </summary>
        /// <param name="inputStr">输入</param>
        /// <param name="formatter"></param>
        /// <param name="defaultValue">默认值</param>
        /// <returns></returns>
        public static DateTime TryToDateTime(this string inputStr, string formatter,
            DateTime defaultValue = default)
        {
            if (inputStr.IsNullOrEmpty())
                return defaultValue;

            return DateTime.TryParseExact(inputStr, formatter,
                CultureInfo.InvariantCulture, DateTimeStyles.None, out var outPutDateTime)
                ? outPutDateTime : defaultValue;
        }

        /// <summary>
        /// 时间戳转时间
        /// </summary>
        /// <param name="timestamp"></param>
        /// <returns></returns>
        public static DateTime TryToDateTime(this string timestamp)
        {
            var ticks = 621355968000000000 + long.Parse(timestamp) * 10000;
            return new DateTime(ticks);
        }

        /// <summary>
        /// 时间格式转换为字符串
        /// </summary>
        /// <param name="date"></param>
        /// <param name="formatter"></param>
        /// <param name="cultureInfo"></param>
        /// <returns></returns>
        public static string TryToDateTime(this DateTime date,
            string formatter = "MMMM dd, yyyy HH:mm:ss", string cultureInfo = "en-us")
        {
            return date.ToString(formatter, new CultureInfo(cultureInfo));
        }

        /// <summary>
        /// 字符串去空格
        /// </summary>
        /// <param name="inputStr">输入</param>
        /// <returns></returns>
        public static string TryToTrim(this string inputStr)
        {
            var output = inputStr.IsNullOrEmpty() ? inputStr : inputStr.Trim();
            return output;
        }

        /// <summary>
        /// 字符串转枚举
        /// </summary>
        /// <typeparam name="T">输入</typeparam>
        /// <param name="str"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        public static T TryToEnum<T>(this string str, T t = default) where T : struct
        {
            return Enum.TryParse<T>(str, out var result) ? result : t;
        }

        /// <summary>
        /// 将枚举类型转换为List
        /// </summary>
        /// <param name="enumType"></param>
        /// <returns></returns>
        public static IEnumerable<EnumResponse> TryToList(this Type enumType)
        {
            var result = new List<EnumResponse>();

            foreach (var item in Enum.GetValues(enumType))
            {
                var response = new EnumResponse
                {
                    Key = item.ToString(),
                    Value = Convert.ToInt32(item)
                };

                var objArray = item.GetType().GetField(item.ToString())
                    .GetCustomAttributes(typeof(DescriptionAttribute), true);
                if (objArray.Any())
                    response.Description = (objArray.First() as DescriptionAttribute)
                        ?.Description;

                result.Add(response);
            }

            return result;
        }

        /// <summary>
        /// 根据类型名返回一个Type类型
        /// </summary>
        /// <param name="typeName">类型的名称</param>
        /// <returns>Type对象</returns>
        public static Type ConvertType(this string typeName) =>
            typeName.ToLower().Replace("system.", "") switch
            {
                "boolean" => typeof(bool),
                "bool" => typeof(bool),
                "int16" => typeof(short),
                "short" => typeof(short),
                "int32" => typeof(int),
                "int" => typeof(int),
                "long" => typeof(long),
                "int64" => typeof(long),
                "uint16" => typeof(ushort),
                "ushort" => typeof(ushort),
                "uint32" => typeof(uint),
                "uint" => typeof(uint),
                "uint64" => typeof(ulong),
                "ulong" => typeof(ulong),
                "single" => typeof(float),
                "float" => typeof(float),
                "string" => typeof(string),
                "guid" => typeof(Guid),
                "decimal" => typeof(decimal),
                "double" => typeof(double),
                "datetime" => typeof(DateTime),
                "byte" => typeof(byte),
                "char" => typeof(char),
                _ => typeof(string)
            };

        /// <summary>
        /// 类型直转
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static T ConvertTo<T>(this IConvertible value) where T : IConvertible
        {
            return (T)ConvertTo(value, typeof(T));
        }

        /// <summary>
        /// 类型直转
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <param name="defaultValue">转换失败的默认值</param>
        /// <returns></returns>
        public static T TryConvertTo<T>(this IConvertible value, T defaultValue = default)
            where T : IConvertible
        {
            try
            {
                return (T)ConvertTo(value, typeof(T));
            }
            catch
            {
                return defaultValue;
            }
        }

        /// <summary>
        /// 类型直转
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <param name="result">转换失败的默认值</param>
        /// <returns></returns>
        public static bool TryConvertTo<T>(this IConvertible value, out T result)
            where T : IConvertible
        {
            try
            {
                result = (T)ConvertTo(value, typeof(T));
                return true;
            }
            catch
            {
                result = default;
                return false;
            }
        }

        /// <summary>
        /// 类型直转
        /// </summary>
        /// <param name="value"></param>
        /// <param name="type">目标类型</param>
        /// <param name="result">转换失败的默认值</param>
        /// <returns></returns>
        public static bool TryConvertTo(this IConvertible value, Type type,
            out object result)
        {
            try
            {
                result = ConvertTo(value, type);
                return true;
            }
            catch
            {
                result = default;
                return false;
            }
        }

        /// <summary>
        /// 类型直转
        /// </summary>
        /// <param name="value"></param>
        /// <param name="type">目标类型</param>
        /// <returns></returns>
        public static object ConvertTo(this IConvertible value, Type type)
        {
            if (null == value)
            {
                return default;
            }

            if (type.IsEnum)
            {
                return Enum.Parse(type, value.ToString(CultureInfo.InvariantCulture));
            }

            if (type.IsGenericType &&
                type.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                var underlyingType = Nullable.GetUnderlyingType(type);
                return underlyingType!.IsEnum
                    ? Enum.Parse(underlyingType,
                        value.ToString(CultureInfo.CurrentCulture))
                    : Convert.ChangeType(value, underlyingType);
            }

            return Convert.ChangeType(value, type);
        }
    }
}