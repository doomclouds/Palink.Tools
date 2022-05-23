using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using Palink.Tools.Extensions.ConvertExt.Enumeration;
using Palink.Tools.Extensions.ObjectExt;
using Palink.Tools.Extensions.SerializeExt;
using Palink.Tools.Extensions.StringExt;

namespace Palink.Tools.Extensions.ConvertExt;

/// <summary>
/// ConvertExtensions
/// </summary>
public static class ConvertExtensions
{
    /// <summary>
    /// object convert to byte
    /// </summary>
    /// <param name="input">input</param>
    /// <param name="default">default value</param>
    /// <returns></returns>
    public static byte ToByte(this object? input, byte @default = 0)
    {
        if (input.IsNull())
            return @default;

        return byte.TryParse(input.ToString(), out var num) ? num : @default;
    }

    /// <summary>
    /// object convert to int
    /// </summary>
    /// <param name="input">input</param>
    /// <param name="default">default value</param>
    /// <returns></returns>
    public static int ToInt(this object? input, int @default = 0)
    {
        if (input.IsNull())
            return @default;

        return int.TryParse(input.ToString(), out var num) ? num : @default;
    }

    /// <summary>
    /// object convert to long
    /// </summary>
    /// <param name="input">input</param>
    /// <param name="default">default value</param>
    /// <returns></returns>
    public static long ToLong(this object? input, long @default = 0)
    {
        if (input.IsNull())
            return @default;

        return long.TryParse(input.ToString(), out var num) ? num : @default;
    }

    /// <summary>
    /// object convert to double
    /// </summary>
    /// <param name="input">input</param>
    /// <param name="default">default value</param>
    /// <returns></returns>
    public static double ToDouble(this object? input, double @default = 0)
    {
        if (input.IsNull())
            return @default;

        return double.TryParse(input.ToString(), out var num) ? num : @default;
    }

    /// <summary>
    /// object convert to decimal
    /// </summary>
    /// <param name="input">input</param>
    /// <param name="default">default value</param>
    /// <returns></returns>
    public static decimal ToDecimal(this object? input, decimal @default = 0)
    {
        if (input.IsNull())
            return @default;

        return decimal.TryParse(input.ToString(), out var num) ? num : @default;
    }

    /// <summary>
    /// object convert to float
    /// </summary>
    /// <param name="input">input</param>
    /// <param name="default">default value</param>
    /// <returns></returns>
    public static float ToFloat(this object? input, float @default = 0)
    {
        if (input.IsNull())
            return @default;

        return float.TryParse(input.ToString(), out var num) ? num : @default;
    }

    /// <summary>
    /// object convert to bool
    /// </summary>
    /// <param name="input">input</param>
    /// <param name="default">default value</param>
    /// <param name="trueVal"></param>
    /// <param name="falseVal"></param>
    /// <returns></returns>
    public static bool ToBool(this object? input, bool @default = false,
        string trueVal = "1",
        string falseVal = "0")
    {
        if (input.IsNull())
            return @default;

        var str = input.ToString();
        if (bool.TryParse(str, out var outBool))
            return outBool;

        outBool = @default;

        if (trueVal == str)
            return true;
        return falseVal != str && outBool;
    }

    /// <summary>
    /// string convert to datetime
    /// </summary>
    /// <param name="input">input</param>
    /// <param name="default">default value</param>
    /// <returns></returns>
    public static DateTime ToDateTime(this string? input, DateTime @default)
    {
        if (input.IsNullOrEmpty())
            return @default;

        return DateTime.TryParse(input, out var outPutDateTime)
            ? outPutDateTime
            : @default;
    }

    /// <summary>
    /// string convert to datetime
    /// </summary>
    /// <param name="input">input</param>
    /// <param name="formatter">formatter</param>
    /// <param name="default">default value</param>
    /// <returns></returns>
    public static DateTime ToDateTime(this string? input, string formatter,
        DateTime @default = default)
    {
        if (input.IsNullOrEmpty())
            return @default;

        return DateTime.TryParseExact(input, formatter,
            CultureInfo.InvariantCulture, DateTimeStyles.None, out var outPutDateTime)
            ? outPutDateTime
            : @default;
    }

    /// <summary>
    /// datetime convert to string
    /// </summary>
    /// <param name="input">input</param>
    /// <param name="formatter">formatter</param>
    /// <param name="cultureInfo">cultureInfo</param>
    /// <returns></returns>
    public static string ToDateString(this DateTime input,
        string formatter = "yyyy-MM-dd HH:mm:ss", string cultureInfo = "zh-cn")
    {
        return input.ToString(formatter, new CultureInfo(cultureInfo));
    }

    /// <summary>
    /// string convert to enum
    /// </summary>
    /// <param name="input">input</param>
    /// <param name="t">t</param>
    /// <returns></returns>
    public static T TryToEnum<T>(this string? input, T t = default) where T : struct
    {
        return Enum.TryParse<T>(input, out var result) ? result : t;
    }

    /// <summary>
    /// enum convert to list
    /// </summary>
    /// <param name="enumType"></param>
    /// <returns></returns>
    public static IEnumerable<EnumResponse> TryToList(this Type enumType)
    {
        if (!enumType.IsEnum)
            throw new ArgumentException("must be enum", nameof(enumType));
        var result = new List<EnumResponse>();

        foreach (var item in Enum.GetValues(enumType))
        {
            var response = new EnumResponse
            {
                Key = item.ToString(),
                Value = Convert.ToInt32(item),
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
    /// Used to simplify and beautify casting an object to a type.
    /// </summary>
    /// <typeparam name="T">Type to be casted</typeparam>
    /// <param name="obj">Object to cast</param>
    /// <returns>Casted object</returns>
    public static T As<T>(this object obj) where T : class => (T)obj;

    /// <summary>
    /// Converts given object to a value type using <see cref="M:System.Convert.ChangeType(System.Object,System.Type)" /> method.
    /// </summary>
    /// <param name="obj">Object to be converted</param>
    /// <typeparam name="T">Type of the target object</typeparam>
    /// <returns>Converted object</returns>
    public static T To<T>(this object obj) where T : struct => typeof(T) == typeof(Guid)
        ? (T)TypeDescriptor.GetConverter(typeof(T))
            .ConvertFromInvariantString(obj.ToString())
        : (T)Convert.ChangeType(obj, typeof(T),
            CultureInfo.InvariantCulture);

    /// <summary>
    /// json to value tuple
    /// </summary>
    /// <param name="valueTupleJson"></param>
    /// <returns></returns>
    public static T? TryToValueTuple<T>(this string valueTupleJson)
    {
        return valueTupleJson.FromJson<T>();
    }

    /// <summary>
    /// tuple to json
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static string TupleTryToString<T>(this T value) where T : struct
    {
        return value.ToJson();
    }
}