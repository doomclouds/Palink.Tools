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
    #region BASE64

    public static string? Base64Encrypt(this string? inputStr,
        string encodingName = "UTF-8")
    {
        if (inputStr.IsNull())
            return default;

        var bytes = Encoding.GetEncoding(encodingName).GetBytes(inputStr);
        return Convert.ToBase64String(bytes);
    }

    public static string? Base64Decrypt(this string? base64String,
        string encodingName = "UTF-8")
    {
        if (base64String.IsNull())
            return default;

        var bytes = Convert.FromBase64String(base64String);
        return Encoding.GetEncoding(encodingName).GetString(bytes);
    }

    #endregion

    #region MD5

    public static string? MD5Encrypt(this string? inputStr)
    {
        if (inputStr.IsNullOrEmpty())
            return default;

        using var md5 = MD5.Create();
        var result = md5.ComputeHash(Encoding.Default.GetBytes(inputStr));
        return BitConverter.ToString(result).Replace("-", "");
    }

    #endregion

    #region SHA

    public static string? SHA1Encrypt(this string? inputStr)
    {
        if (inputStr.IsNull())
            return default;

        using var sha1 = SHA1.Create();
        var buffer = Encoding.UTF8.GetBytes(inputStr);
        var byteArr = sha1.ComputeHash(buffer);
        return BitConverter.ToString(byteArr).Replace("-", "").ToLower();
    }

    public static string? SHA256SEncrypt(this string? inputStr)
    {
        if (inputStr.IsNull())
            return default;

        using var sha256 = SHA256.Create();
        var buffer = Encoding.UTF8.GetBytes(inputStr);
        var byteArr = sha256.ComputeHash(buffer);
        return BitConverter.ToString(byteArr).Replace("-", "").ToLower();
    }

    public static string? SHA384Encrypt(this string? inputStr)
    {
        if (inputStr.IsNull())
            return default;

        using var sha384 = SHA384.Create();
        var buffer = Encoding.UTF8.GetBytes(inputStr);
        var byteArr = sha384.ComputeHash(buffer);
        return BitConverter.ToString(byteArr).Replace("-", "").ToLower();
    }

    public static string? SHA512Encrypt(this string? inputStr)
    {
        if (inputStr.IsNull())
            return default;

        using var sha512 = SHA512.Create();
        var buffer = Encoding.UTF8.GetBytes(inputStr);
        var byteArr = sha512.ComputeHash(buffer);
        return BitConverter.ToString(byteArr).Replace("-", "").ToLower();
    }

    #endregion

    #region AES

    /// <summary>
    /// AES加密
    /// </summary>
    /// <param name="inputStr">加密字符</param>
    /// <param name="password">加密的密码</param>
    /// <param name="iv">密钥</param>
    /// <returns></returns>
    public static string? AESEncrypt(this string? inputStr, string password, string iv)
    {
        if (inputStr.IsNullOrEmpty()) return default;

        using var aesAgl = Aes.Create();
        aesAgl.Mode = CipherMode.CBC;
        aesAgl.Padding = PaddingMode.PKCS7;
        aesAgl.KeySize = 128;
        aesAgl.BlockSize = 128;

        var pwdBytes = Encoding.UTF8.GetBytes(password);
        var keyBytes = new byte[16];
        var len = pwdBytes.Length;
        if (len > keyBytes.Length) len = keyBytes.Length;
        Array.Copy(pwdBytes, keyBytes, len);
        aesAgl.Key = keyBytes;

        var ivBytes = Encoding.UTF8.GetBytes(iv);
        var tIVBytes = new byte[16];
        len = ivBytes.Length;
        if (len > tIVBytes.Length) len = tIVBytes.Length;
        Array.Copy(ivBytes, tIVBytes, len);
        aesAgl.IV = tIVBytes;

        var transform = aesAgl.CreateEncryptor();
        var plainText = Encoding.UTF8.GetBytes(inputStr);
        var cipherBytes =
            transform.TransformFinalBlock(plainText, 0, plainText.Length);
        return Convert.ToBase64String(cipherBytes);
    }

    /// <summary>
    /// AES解密
    /// </summary>
    /// <param name="inputStr">解密字符</param>
    /// <param name="password">解密的密码</param>
    /// <param name="iv">密钥</param>
    /// <returns></returns>
    public static string? AESDecrypt(this string? inputStr, string password, string iv)
    {
        if (inputStr.IsNullOrEmpty()) return default;

        using var aesAgl = Aes.Create();
        aesAgl.Mode = CipherMode.CBC;
        aesAgl.Padding = PaddingMode.PKCS7;
        aesAgl.KeySize = 128;
        aesAgl.BlockSize = 128;

        var encryptedData = Convert.FromBase64String(inputStr);
        var pwdBytes = Encoding.UTF8.GetBytes(password);
        var keyBytes = new byte[16];
        var len = pwdBytes.Length;
        if (len > keyBytes.Length) len = keyBytes.Length;
        Array.Copy(pwdBytes, keyBytes, len);
        aesAgl.Key = keyBytes;

        var ivBytes = Encoding.UTF8.GetBytes(iv);
        var tIVBytes = new byte[16];
        len = ivBytes.Length;
        if (len > tIVBytes.Length) len = tIVBytes.Length;
        Array.Copy(ivBytes, tIVBytes, len);
        aesAgl.IV = tIVBytes;

        var transform = aesAgl.CreateDecryptor();
        var plainText =
            transform.TransformFinalBlock(encryptedData, 0, encryptedData.Length);
        return Encoding.UTF8.GetString(plainText);
    }
}

#endregion