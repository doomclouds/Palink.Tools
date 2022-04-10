﻿using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace Palink.Tools.Extensions.PLString;

/// <summary>
/// 字符判断扩展类
/// </summary>
public static class StringExtensions
{
    #region 空判断

    /// <summary>
    /// 字符串为空或空字符
    /// </summary>
    /// <param name="inputStr"></param>
    /// <returns></returns>
    public static bool IsNullOrEmpty(this string? inputStr)
    {
        return string.IsNullOrEmpty(inputStr);
    }

    /// <summary>
    /// 字符串为空或只包含空格
    /// </summary>
    /// <param name="inputStr"></param>
    /// <returns></returns>
    public static bool IsNullOrWhiteSpace(this string inputStr)
    {
        return string.IsNullOrWhiteSpace(inputStr);
    }

    /// <summary>
    /// 字符串非空且非空字符
    /// </summary>
    /// <param name="inputStr"></param>
    /// <returns></returns>
    public static bool IsNotNullOrEmpty(this string inputStr)
    {
        return !string.IsNullOrEmpty(inputStr);
    }

    /// <summary>
    /// 字符串非空且不包含空格
    /// </summary>
    /// <param name="inputStr"></param>
    /// <returns></returns>
    public static bool IsNotNullOrWhiteSpace(this string inputStr)
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
    /// <returns></returns>
    public static bool IsImgFileName(this string fileName)
    {
        var suffix = new List<string>()
        {
            ".jpg",
            ".jpeg",
            ".png",
            ".bmp"
        };

        var fileSuffix = Path.GetExtension(fileName).ToLower();

        return suffix.Contains(fileSuffix);
    }

    #endregion

    #region 字符串截取

    /// <summary>
    /// Sub
    /// </summary>
    /// <param name="inputStr"></param>
    /// <param name="length"></param>
    /// <returns></returns>
    public static string? Sub(this string? inputStr, int length)
    {
        if (inputStr.IsNullOrEmpty())
            return null;

        return inputStr.Length >= length ? inputStr.Substring(0, length) : inputStr;
    }

    /// <summary>
    /// TryReplace
    /// </summary>
    /// <param name="inputStr"></param>
    /// <param name="oldStr"></param>
    /// <param name="newStr"></param>
    /// <returns></returns>
    public static string TryReplace(this string inputStr, string oldStr,
        string newStr)
    {
        return inputStr.IsNullOrEmpty() ? inputStr : inputStr.Replace(oldStr, newStr);
    }

    /// <summary>
    /// RegexReplace
    /// </summary>
    /// <param name="inputStr"></param>
    /// <param name="pattern"></param>
    /// <param name="replacement"></param>
    /// <returns></returns>
    public static string RegexReplace(this string inputStr, string pattern,
        string replacement)
    {
        return inputStr.IsNullOrEmpty() ? inputStr
            : Regex.Replace(inputStr, pattern, replacement);
    }

    #endregion

    #region Format

    /// <summary>
    /// Format
    /// </summary>
    /// <param name="format"></param>
    /// <param name="arg0"></param>
    /// <param name="arg1"></param>
    /// <returns></returns>
    public static string Format(this string format, object arg0, object arg1)
    {
        return string.Format(format, arg0, arg1);
    }

    /// <summary>
    /// Format
    /// </summary>
    /// <param name="format"></param>
    /// <param name="args"></param>
    /// <returns></returns>
    public static string Format(this string format, object[] args)
    {
        return string.Format(format, args);
    }

    /// <summary>
    /// Format
    /// </summary>
    /// <param name="format"></param>
    /// <param name="arg0"></param>
    /// <param name="arg1"></param>
    /// <param name="arg2"></param>
    /// <returns></returns>
    public static string Format(this string format, object arg0, object arg1,
        object arg2)
    {
        return string.Format(format, arg0, arg1, arg2);
    }

    /// <summary>
    /// Format
    /// </summary>
    /// <param name="format"></param>
    /// <param name="arg0"></param>
    /// <returns></returns>
    public static string Format(this string format, object arg0)
    {
        return string.Format(format, arg0);
    }

    /// <summary>
    /// FormatWith
    /// </summary>
    /// <param name="this"></param>
    /// <param name="values"></param>
    /// <returns></returns>
    public static string FormatWith(this string @this, params object[] values)
    {
        return string.Format(@this, values);
    }

    /// <summary>
    /// FormatWith
    /// </summary>
    /// <param name="this"></param>
    /// <param name="arg0"></param>
    /// <param name="arg1"></param>
    /// <param name="arg2"></param>
    /// <returns></returns>
    public static string FormatWith(this string @this, object arg0, object arg1,
        object arg2)
    {
        return string.Format(@this, arg0, arg1, arg2);
    }

    /// <summary>
    /// FormatWith
    /// </summary>
    /// <param name="this"></param>
    /// <param name="arg0"></param>
    /// <param name="arg1"></param>
    /// <returns></returns>
    public static string FormatWith(this string @this, object arg0, object arg1)
    {
        return string.Format(@this, arg0, arg1);
    }

    /// <summary>
    /// FormatWith
    /// </summary>
    /// <param name="this"></param>
    /// <param name="arg0"></param>
    /// <returns></returns>
    public static string FormatWith(this string @this, object arg0)
    {
        return string.Format(@this, arg0);
    }

    #endregion
}