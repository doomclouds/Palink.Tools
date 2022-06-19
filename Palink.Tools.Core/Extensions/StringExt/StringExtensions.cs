using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Text.RegularExpressions;
using Palink.Tools.Extensions.ObjectExt;

namespace Palink.Tools.Extensions.StringExt;

/// <summary>
/// StringExtensions
/// </summary>
public static class StringExtensions
{
    #region 空判断

    /// <summary>
    /// 字符串为空或空字符
    /// </summary>
    /// <param name="inputStr"></param>
    /// <returns></returns>
    public static bool IsNullOrEmpty([NotNullWhen(false)] this string? inputStr)
    {
        return string.IsNullOrEmpty(inputStr);
    }

    /// <summary>
    /// 字符串为非空且非空字符
    /// </summary>
    /// <param name="inputStr"></param>
    /// <returns></returns>
    public static bool NotNullNotEmpty([NotNullWhen(true)] this string? inputStr)
    {
        return !string.IsNullOrEmpty(inputStr);
    }

    /// <summary>
    /// 字符串为空或只包含空格
    /// </summary>
    /// <param name="inputStr"></param>
    /// <returns></returns>
    public static bool IsNullOrWhiteSpace([NotNullWhen(false)] this string? inputStr)
    {
        return string.IsNullOrWhiteSpace(inputStr);
    }

    /// <summary>
    /// 字符串为非空且不包含空格
    /// </summary>
    /// <param name="inputStr"></param>
    /// <returns></returns>
    public static bool NotNullNotWhiteSpace([NotNullWhen(true)] this string? inputStr)
    {
        return !string.IsNullOrWhiteSpace(inputStr);
    }

    #endregion

    #region 常用正则表达式

    private static readonly Regex EmailRegex = new(
        @"\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*", RegexOptions.IgnoreCase);

    private static readonly Regex MobileRegex = new("^1[0-9]{10}$");

    private static readonly Regex PhoneRegex = new(@"^(\d{3,4}-?)?\d{7,8}$");

    private static readonly Regex IpRegex =
        new(
            @"^(\d{1,2}|1\d\d|2[0-4]\d|25[0-5])\.(\d{1,2}|1\d\d|2[0-4]\d|25[0-5])\.(\d{1,2}|1\d\d|2[0-4]\d|25[0-5])\.(\d{1,2}|1\d\d|2[0-4]\d|25[0-5])$");

    private static readonly Regex DateRegex = new(@"(\d{4})-(\d{1,2})-(\d{1,2})");

    private static readonly Regex NumericRegex = new(@"^[-]?[0-9]+(\.[0-9]+)?$");

    private static readonly Regex ZipcodeRegex = new(@"^\d{6}$");

    private static readonly Regex IdRegex = new(@"^[1-9]\d{16}[\dXx]$");

    /// <summary>
    /// 是否中文
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public static bool IsChinese(this string str)
    {
        return Regex.IsMatch(@"^[\u4e00-\u9fa5]+$", str);
    }

    /// <summary>
    /// 是否为邮箱名
    /// </summary>
    public static bool IsEmail(this string s)
    {
        return !string.IsNullOrEmpty(s) && EmailRegex.IsMatch(s);
    }

    /// <summary>
    /// 是否为手机号
    /// </summary>
    public static bool IsMobile(this string s)
    {
        return !string.IsNullOrEmpty(s) && MobileRegex.IsMatch(s);
    }

    /// <summary>
    /// 是否为固话号
    /// </summary>
    public static bool IsPhone(this string s)
    {
        return !string.IsNullOrEmpty(s) && PhoneRegex.IsMatch(s);
    }

    /// <summary>
    /// 是否为IP
    /// </summary>
    public static bool IsIp(this string s)
    {
        return IpRegex.IsMatch(s);
    }

    /// <summary>
    /// 是否是身份证号
    /// </summary>
    public static bool IsIdCard(this string idCard)
    {
        return !string.IsNullOrEmpty(idCard) && IdRegex.IsMatch(idCard);
    }

    /// <summary>
    /// 是否为日期
    /// </summary>
    public static bool IsDate(this string s)
    {
        return DateRegex.IsMatch(s);
    }

    /// <summary>
    /// 是否是数值(包括整数和小数)
    /// </summary>
    public static bool IsNumeric(this string numericStr)
    {
        return NumericRegex.IsMatch(numericStr);
    }

    /// <summary>
    /// 是否为邮政编码
    /// </summary>
    public static bool IsZipCode(this string s)
    {
        return string.IsNullOrEmpty(s) || ZipcodeRegex.IsMatch(s);
    }

    /// <summary>
    /// 是否是图片文件名
    /// </summary>
    /// <param name="fileName"></param>
    /// <param name="fileExtensions">图像文件扩展名</param>
    /// <returns></returns>
    public static bool IsImgFileName(this string fileName,
        List<string>? fileExtensions = default)
    {
        var suffix = new List<string>()
        {
            ".jpg",
            ".jpeg",
            ".png",
            ".bmp"
        };

        if (fileExtensions.NotNull())
        {
            suffix = fileExtensions;
        }

        var fileSuffix = Path.GetExtension(fileName).ToLower();

        return suffix.Contains(fileSuffix);
    }

    #endregion

    #region 字符串替换

    /// <summary>
    /// TryReplace
    /// </summary>
    /// <param name="inputStr"></param>
    /// <param name="oldStr"></param>
    /// <param name="newStr"></param>
    /// <returns></returns>
    public static string TryReplace(this string? inputStr, string oldStr,
        string newStr)
    {
        return inputStr.IsNullOrEmpty() ? "" : inputStr.Replace(oldStr, newStr);
    }

    /// <summary>
    /// RegexReplace
    /// </summary>
    /// <param name="inputStr"></param>
    /// <param name="pattern"></param>
    /// <param name="replacement"></param>
    /// <returns></returns>
    public static string RegexReplace(this string? inputStr, string pattern,
        string replacement)
    {
        return inputStr.IsNullOrEmpty()
            ? ""
            : Regex.Replace(inputStr, pattern, replacement);
    }

    #endregion

    #region Format
    
    /// <summary>
    /// FormatWith
    /// </summary>
    /// <param name="format"></param>
    /// <param name="values"></param>
    /// <returns></returns>
    public static string FormatWith(this string format, params object[] values)
    {
        return string.Format(format, values);
    }

    /// <summary>
    /// FormatWith
    /// </summary>
    /// <param name="format"></param>
    /// <param name="arg0"></param>
    /// <param name="arg1"></param>
    /// <param name="arg2"></param>
    /// <returns></returns>
    public static string FormatWith(this string format, object arg0, object arg1,
        object arg2)
    {
        return string.Format(format, arg0, arg1, arg2);
    }

    /// <summary>
    /// FormatWith
    /// </summary>
    /// <param name="format"></param>
    /// <param name="arg0"></param>
    /// <param name="arg1"></param>
    /// <returns></returns>
    public static string FormatWith(this string format, object arg0, object arg1)
    {
        return string.Format(format, arg0, arg1);
    }

    /// <summary>
    /// FormatWith
    /// </summary>
    /// <param name="format"></param>
    /// <param name="arg0"></param>
    /// <returns></returns>
    public static string FormatWith(this string format, object arg0)
    {
        return string.Format(format, arg0);
    }

    #endregion
}