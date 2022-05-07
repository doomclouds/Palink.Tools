using System;
using System.Security.Cryptography;
using System.Text;
using Palink.Tools.Extensions.PLObject;
using Palink.Tools.Extensions.PLString;

namespace Palink.Tools.Extensions.PLSecurity;

/// <summary>
/// EncryptExtensions
/// </summary>
public static class EncryptExtensions
{
    #region Base64

    public static string? EncodeBase64String(this string? inputStr,
        string encodingName = "UTF-8")
    {
        if (inputStr.IsNull())
            return default;

        var bytes = Encoding.GetEncoding(encodingName).GetBytes(inputStr);
        return Convert.ToBase64String(bytes);
    }

    public static string? DecodeBase64String(this string? base64String,
        string encodingName = "UTF-8")
    {
        if (base64String.IsNull())
            return default;

        var bytes = Convert.FromBase64String(base64String);
        return Encoding.GetEncoding(encodingName).GetString(bytes);
    }

    #endregion

    #region Md5

    public static string? EncodeMd5String(this string? inputStr)
    {
        if (inputStr.IsNullOrEmpty())
            return default;

        using var md5 = MD5.Create();
        var result = md5.ComputeHash(Encoding.Default.GetBytes(inputStr));
        return BitConverter.ToString(result).Replace("-", "");
    }

    #endregion

    #region SHA

    public static string? EncodeSha1String(this string? inputStr)
    {
        if (inputStr.IsNull())
            return default;

        using var sha1 = SHA1.Create();
        var buffer = Encoding.UTF8.GetBytes(inputStr);
        var byteArr = sha1.ComputeHash(buffer);
        return BitConverter.ToString(byteArr).Replace("-", "").ToLower();
    }

    public static string? EncodeSha256String(this string? inputStr)
    {
        if (inputStr.IsNull())
            return default;

        using var sha256 = SHA256.Create();
        var buffer = Encoding.UTF8.GetBytes(inputStr);
        var byteArr = sha256.ComputeHash(buffer);
        return BitConverter.ToString(byteArr).Replace("-", "").ToLower();
    }

    public static string? EncodeSha384String(this string? inputStr)
    {
        if (inputStr.IsNull())
            return default;

        using var sha384 = SHA384.Create();
        var buffer = Encoding.UTF8.GetBytes(inputStr);
        var byteArr = sha384.ComputeHash(buffer);
        return BitConverter.ToString(byteArr).Replace("-", "").ToLower();
    }

    public static string? EncodeSha512String(this string? inputStr)
    {
        if (inputStr.IsNull())
            return default;

        using var sha512 = SHA512.Create();
        var buffer = Encoding.UTF8.GetBytes(inputStr);
        var byteArr = sha512.ComputeHash(buffer);
        return BitConverter.ToString(byteArr).Replace("-", "").ToLower();
    }

    #endregion
}