using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace Palink.Tools.Extensions.PLSecurity;

/// <summary>
/// 常用的加密解密算法
/// </summary>
[SuppressMessage("ReSharper", "UnusedVariable")]
public static class Encrypt
{
    #region DES对称加密解密

    /// <summary>
    /// 加密密钥，默认取“palink”的MD5值
    /// </summary>
    public static string DefaultEncryptKey = "palink".Md5String2();

    /// <summary>
    /// 使用默认加密
    /// </summary>
    /// <param name="strText">被加密的字符串</param>
    /// <returns>加密后的数据</returns>
    public static string DesEncrypt(this string strText)
    {
        try
        {
            return DesEncrypt(strText, DefaultEncryptKey);
        }
        catch
        {
            return "";
        }
    }

    /// <summary>
    /// 使用默认解密
    /// </summary>
    /// <param name="strText">需要解密的 字符串</param>
    /// <returns>解密后的数据</returns>
    public static string DesDecrypt(this string strText)
    {
        try
        {
            return DesDecrypt(strText, DefaultEncryptKey);
        }
        catch
        {
            return "";
        }
    }

    /// <summary> 
    /// 加密字符串
    /// 加密密钥必须为8位
    /// </summary> 
    /// <param name="strText">被加密的字符串</param> 
    /// <param name="strEncryptKey">8位长度密钥</param> 
    /// <returns>加密后的数据</returns> 
    public static string DesEncrypt(this string strText, string strEncryptKey)
    {
        if (strEncryptKey.Length < 8)
        {
            throw new Exception("密钥长度无效，密钥必须是8位！");
        }

        var ret = new StringBuilder();
        using var des = new DESCryptoServiceProvider();
        var inputByteArray = Encoding.Default.GetBytes(strText);
        des.Key = Encoding.ASCII.GetBytes(strEncryptKey.Substring(0, 8));
        des.IV = Encoding.ASCII.GetBytes(strEncryptKey.Substring(0, 8));
        var ms = new MemoryStream();
        using var cs =
            new CryptoStream(ms, des.CreateEncryptor(), CryptoStreamMode.Write);
        cs.Write(inputByteArray, 0, inputByteArray.Length);
        cs.FlushFinalBlock();
        foreach (var b in ms.ToArray())
        {
            ret.AppendFormat($"{b:X2}");
        }

        return ret.ToString();
    }

    /// <summary> 
    /// 加密字符串
    /// 加密密钥必须为8位
    /// </summary> 
    /// <param name="strText">被加密的字符串</param>
    /// <param name="desKey"></param>
    /// <param name="desIv"></param>
    /// <returns>加密后的数据</returns> 
    public static string DesEncrypt(this string strText, byte[] desKey, byte[] desIv)
    {
        var ret = new StringBuilder();
        using var des = new DESCryptoServiceProvider();
        var inputByteArray = Encoding.Default.GetBytes(strText);
        des.Key = desKey;
        des.IV = desIv;
        var ms = new MemoryStream();
        using var cs =
            new CryptoStream(ms, des.CreateEncryptor(), CryptoStreamMode.Write);
        cs.Write(inputByteArray, 0, inputByteArray.Length);
        cs.FlushFinalBlock();
        foreach (var b in ms.ToArray())
        {
            ret.AppendFormat($"{b:X2}");
        }

        return ret.ToString();
    }

    /// <summary>
    /// DES加密文件
    /// </summary>
    /// <param name="fin">文件输入流</param>
    /// <param name="outFilePath">文件输出路径</param>
    /// <param name="strEncryptKey">加密密钥</param>
    public static void DesEncrypt(this FileStream fin, string outFilePath,
        string strEncryptKey)
    {
        byte[] iv =
        {
            0x12,
            0x34,
            0x56,
            0x78,
            0x90,
            0xAB,
            0xCD,
            0xEF
        };
        var byKey = Encoding.UTF8.GetBytes(strEncryptKey.Substring(0, 8));
        using var fOut =
            new FileStream(outFilePath, FileMode.OpenOrCreate, FileAccess.Write);
        fOut.SetLength(0);
        var bin = new byte[100];
        long rdLen = 0;
        var totLen = fin.Length;
        DES des = new DESCryptoServiceProvider();
        var encStream = new CryptoStream(fOut, des.CreateEncryptor(byKey, iv),
            CryptoStreamMode.Write);
        while (rdLen < totLen)
        {
            var len = fin.Read(bin, 0, 100);
            encStream.Write(bin, 0, len);
            rdLen += len;
        }
    }

    /// <summary>
    /// DES加密文件
    /// </summary>
    /// <param name="fin">文件输入流</param>
    /// <param name="outFilePath">文件输出路径</param>
    /// <param name="desKey"></param>
    /// <param name="desIv"></param>
    public static void DesEncrypt(this FileStream fin, string outFilePath, byte[] desKey,
        byte[] desIv)
    {
        using var fOut =
            new FileStream(outFilePath, FileMode.OpenOrCreate, FileAccess.Write);
        fOut.SetLength(0);
        var bin = new byte[100];
        long rdLen = 0;
        var totLen = fin.Length;
        DES des = new DESCryptoServiceProvider();
        var encStream = new CryptoStream(fOut, des.CreateEncryptor(desKey, desIv),
            CryptoStreamMode.Write);
        while (rdLen < totLen)
        {
            var len = fin.Read(bin, 0, 100);
            encStream.Write(bin, 0, len);
            rdLen += len;
        }
    }

    /// <summary>
    /// DES解密文件
    /// </summary>
    /// <param name="fin">输入文件流</param>
    /// <param name="outFilePath">文件输出路径</param>
    /// <param name="decryptKey">解密密钥</param>
    public static void DesDecrypt(this FileStream fin, string outFilePath,
        string decryptKey)
    {
        byte[] iv =
        {
            0x12,
            0x34,
            0x56,
            0x78,
            0x90,
            0xAB,
            0xCD,
            0xEF
        };
        var byKey = Encoding.UTF8.GetBytes(decryptKey.Substring(0, 8));
        using var fOut =
            new FileStream(outFilePath, FileMode.OpenOrCreate, FileAccess.Write);
        fOut.SetLength(0);
        var bin = new byte[100];
        long rdLen = 0;
        var totLen = fin.Length;
        using DES des = new DESCryptoServiceProvider();
        var encStream = new CryptoStream(fOut, des.CreateDecryptor(byKey, iv),
            CryptoStreamMode.Write);
        while (rdLen < totLen)
        {
            var len = fin.Read(bin, 0, 100);
            encStream.Write(bin, 0, len);
            rdLen += len;
        }
    }

    /// <summary>
    /// DES解密文件
    /// </summary>
    /// <param name="fin">输入文件流</param>
    /// <param name="outFilePath">文件输出路径</param>
    /// <param name="desKey"></param>
    /// <param name="desIv"></param>
    public static void DesDecrypt(this FileStream fin, string outFilePath, byte[] desKey,
        byte[] desIv)
    {
        using var fOut =
            new FileStream(outFilePath, FileMode.OpenOrCreate, FileAccess.Write);
        fOut.SetLength(0);
        var bin = new byte[100];
        long rdLen = 0;
        var totLen = fin.Length;
        using DES des = new DESCryptoServiceProvider();
        var encStream = new CryptoStream(fOut, des.CreateDecryptor(desKey, desIv),
            CryptoStreamMode.Write);
        while (rdLen < totLen)
        {
            var len = fin.Read(bin, 0, 100);
            encStream.Write(bin, 0, len);
            rdLen += len;
        }
    }

    /// <summary>
    /// DES解密算法
    ///     密钥为8位
    /// </summary>
    /// <param name="pToDecrypt">需要解密的字符串</param>
    /// <param name="sKey">密钥</param>
    /// <returns>解密后的数据</returns>
    public static string DesDecrypt(this string pToDecrypt, string sKey)
    {
        if (sKey.Length < 8)
        {
            throw new Exception("密钥长度无效，密钥必须是8位！");
        }

        var ms = new MemoryStream();
        using var des = new DESCryptoServiceProvider();
        var inputByteArray = new byte[pToDecrypt.Length / 2];
        for (var x = 0; x < pToDecrypt.Length / 2; x++)
        {
            var i = Convert.ToInt32(pToDecrypt.Substring(x * 2, 2), 16);
            inputByteArray[x] = (byte)i;
        }

        des.Key = Encoding.ASCII.GetBytes(sKey.Substring(0, 8));
        des.IV = Encoding.ASCII.GetBytes(sKey.Substring(0, 8));
        using var cs =
            new CryptoStream(ms, des.CreateDecryptor(), CryptoStreamMode.Write);
        cs.Write(inputByteArray, 0, inputByteArray.Length);
        cs.FlushFinalBlock();
        return Encoding.Default.GetString(ms.ToArray());
    }

    /// <summary>
    /// DES解密算法
    /// 密钥为8位
    /// </summary>
    /// <param name="pToDecrypt">需要解密的字符串</param>
    /// <param name="desKey"></param>
    /// <param name="desIv"></param>
    /// <returns>解密后的数据</returns>
    public static string DesDecrypt(this string pToDecrypt, byte[] desKey, byte[] desIv)
    {
        var ms = new MemoryStream();
        using var des = new DESCryptoServiceProvider();
        var inputByteArray = new byte[pToDecrypt.Length / 2];
        for (var x = 0; x < pToDecrypt.Length / 2; x++)
        {
            var i = Convert.ToInt32(pToDecrypt.Substring(x * 2, 2), 16);
            inputByteArray[x] = (byte)i;
        }

        des.Key = desKey;
        des.IV = desIv;
        using var cs =
            new CryptoStream(ms, des.CreateDecryptor(), CryptoStreamMode.Write);
        cs.Write(inputByteArray, 0, inputByteArray.Length);
        cs.FlushFinalBlock();
        return Encoding.Default.GetString(ms.ToArray());
    }

    #endregion

    #region MD5加密算法

    #region 对字符串进行MD5加密

    /// <summary>
    /// 对字符串进行MD5加密
    /// </summary>
    /// <param name="message">需要加密的字符串</param>
    /// <returns>加密后的结果</returns>
    public static string Md5String(this string message)
    {
        using var md5 = MD5.Create();
        var buffer = Encoding.Default.GetBytes(message);
        var bytes = md5.ComputeHash(buffer);
        return bytes.Aggregate("", (current, b) => current + b.ToString("x2"));
    }

    /// <summary>
    /// 对字符串进行MD5二次加密
    /// </summary>
    /// <param name="message">需要加密的字符串</param>
    /// <returns>加密后的结果</returns>
    public static string Md5String2(this string message) => Md5String(Md5String(message));

    /// <summary>
    /// MD5 三次加密算法
    /// </summary>
    /// <param name="s">需要加密的字符串</param>
    /// <returns>MD5字符串</returns>
    public static string Md5String3(this string s)
    {
        using var md5 = MD5.Create();
        var bytes = Encoding.ASCII.GetBytes(s);
        var bytes1 = md5.ComputeHash(bytes);
        var bytes2 = md5.ComputeHash(bytes1);
        var bytes3 = md5.ComputeHash(bytes2);
        return bytes3.Aggregate("", (current, b) => current + b.ToString("x2"));
    }

    /// <summary>
    /// 对字符串进行MD5加盐加密
    /// </summary>
    /// <param name="message">需要加密的字符串</param>
    /// <param name="salt">盐</param>
    /// <returns>加密后的结果</returns>
    public static string Md5String(this string message, string salt) =>
        Md5String(message + salt);

    /// <summary>
    /// 对字符串进行MD5二次加盐加密
    /// </summary>
    /// <param name="message">需要加密的字符串</param>
    /// <param name="salt">盐</param>
    /// <returns>加密后的结果</returns>
    public static string Md5String2(this string message, string salt) =>
        Md5String(Md5String(message + salt), salt);

    /// <summary>
    /// MD5三次加密算法
    /// </summary>
    /// <param name="s">需要加密的字符串</param>
    /// <param name="salt">盐</param>
    /// <returns>MD5字符串</returns>
    public static string Md5String3(this string s, string salt)
    {
        using var md5 = MD5.Create();
        var bytes = Encoding.ASCII.GetBytes(s + salt);
        var bytes1 = md5.ComputeHash(bytes);
        var bytes2 = md5.ComputeHash(bytes1);
        var bytes3 = md5.ComputeHash(bytes2);
        return bytes3.Aggregate("", (current, b) => current + b.ToString("x2"));
    }

    #endregion

    #region 获取文件的MD5值

    /// <summary>
    /// 获取文件的MD5值
    /// </summary>
    /// <param name="fileName">需要求MD5值的文件的文件名及路径</param>
    /// <returns>MD5字符串</returns>
    public static string Md5File(this string fileName)
    {
        using var fs =
            new BufferedStream(File.Open(fileName, FileMode.Open, FileAccess.Read),
                1048576);
        using var md5 = MD5.Create();
        var bytes = md5.ComputeHash(fs);
        return bytes.Aggregate("", (current, b) => current + b.ToString("x2"));
    }

    /// <summary>
    /// 获取数据流的MD5值
    /// </summary>
    /// <param name="stream"></param>
    /// <returns>MD5字符串</returns>
    public static string Md5String(this Stream stream)
    {
        using var fs = new BufferedStream(stream, 1048576);
        using var md5 = MD5.Create();
        var bytes = md5.ComputeHash(fs);
        var md5Str = bytes.Aggregate("", (current, b) => current + b.ToString("x2"));
        stream.Position = 0;
        return md5Str;
    }

    #endregion

    #endregion MD5加密算法

    #region 对称加密算法AES RijndaelManaged加密解密

    private const string DefaultAesKey = "@#Palink2022";

    private static readonly byte[] Keys =
    {
        0x41,
        0x72,
        0x65,
        0x79,
        0x6F,
        0x75,
        0x6D,
        0x79,
        0x53,
        0x6E,
        0x6F,
        0x77,
        0x6D,
        0x61,
        0x6E,
        0x3F
    };

    /// <summary>
    /// 生成符合AES加密规则的密钥
    /// </summary>
    /// <param name="length"></param>
    /// <returns></returns>
    public static string GenerateAesKey(int length)
    {
        var crypto = new AesCryptoServiceProvider
        {
            KeySize = length,
            BlockSize = 128
        };
        crypto.GenerateKey();
        return Convert.ToBase64String(crypto.Key);
    }

    /// <summary>
    /// 对称加密算法AES RijndaelManaged加密(RijndaelManaged（AES）算法是块式加密算法)
    /// </summary>
    /// <param name="encryptString">待加密字符串</param>
    /// <param name="mode">加密模式</param>
    /// <returns>加密结果字符串</returns>
    public static string AesEncrypt(this string encryptString,
        CipherMode mode = CipherMode.CBC)
    {
        return AesEncrypt(encryptString, DefaultAesKey, mode);
    }

    /// <summary>
    /// 对称加密算法AES RijndaelManaged加密(RijndaelManaged（AES）算法是块式加密算法)
    /// </summary>
    /// <param name="encryptString">待加密字符串</param>
    /// <param name="encryptKey">加密密钥，须半角字符</param>
    /// <param name="mode">加密模式</param>
    /// <returns>加密结果字符串</returns>
    public static string AesEncrypt(this string encryptString, string encryptKey,
        CipherMode mode = CipherMode.CBC)
    {
        encryptKey = GetSubString(encryptKey, 32, "");
        encryptKey = encryptKey.PadRight(32, ' ');
        using var rijndaelProvider = new RijndaelManaged
        {
            Key = Encoding.UTF8.GetBytes(encryptKey.Substring(0, 32)),
            IV = Keys,
            Mode = mode
        };
        using var rijndaelEncrypt = rijndaelProvider.CreateEncryptor();
        var inputData = Encoding.UTF8.GetBytes(encryptString);
        var encryptedData =
            rijndaelEncrypt.TransformFinalBlock(inputData, 0, inputData.Length);
        return Convert.ToBase64String(encryptedData);
    }

    /// <summary>
    /// 对称加密算法AES RijndaelManaged加密(RijndaelManaged（AES）算法是块式加密算法)
    /// </summary>
    /// <param name="encryptString">待加密字符串</param>
    /// <param name="options">加密选项</param>
    /// <returns>加密结果字符串</returns>
    public static string AesEncrypt(this string encryptString, RijndaelManaged options)
    {
        using var rijndaelEncrypt = options.CreateEncryptor();
        var inputData = Encoding.UTF8.GetBytes(encryptString);
        var encryptedData =
            rijndaelEncrypt.TransformFinalBlock(inputData, 0, inputData.Length);
        return Convert.ToBase64String(encryptedData);
    }

    /// <summary>
    /// 对称加密算法AES RijndaelManaged加密(RijndaelManaged（AES）算法是块式加密算法)
    /// </summary>
    /// <param name="encryptString">待加密字符串</param>
    /// <param name="encryptKey">加密密钥，须半角字符</param>
    /// <param name="mode">加密模式</param>
    /// <returns>加密结果字符串</returns>
    public static string AesEncrypt(this string encryptString, byte[] encryptKey,
        CipherMode mode = CipherMode.CBC)
    {
        using var rijndaelProvider = new RijndaelManaged
        {
            Key = encryptKey,
            IV = Keys,
            Mode = mode
        };
        using var rijndaelEncrypt = rijndaelProvider.CreateEncryptor();
        var inputData = Encoding.UTF8.GetBytes(encryptString);
        var encryptedData =
            rijndaelEncrypt.TransformFinalBlock(inputData, 0, inputData.Length);
        return Convert.ToBase64String(encryptedData);
    }

    /// <summary>
    /// 对称加密算法AES RijndaelManaged解密字符串
    /// </summary>
    /// <param name="decryptString">待解密的字符串</param>
    /// <param name="mode">加密模式</param>
    /// <returns>解密成功返回解密后的字符串,失败返源串</returns>
    public static string AesDecrypt(this string decryptString,
        CipherMode mode = CipherMode.CBC)
    {
        return AesDecrypt(decryptString, DefaultAesKey, mode);
    }

    /// <summary>
    /// 对称加密算法AES RijndaelManaged解密字符串
    /// </summary>
    /// <param name="decryptString">待解密的字符串</param>
    /// <param name="decryptKey">解密密钥,和加密密钥相同</param>
    /// <param name="mode">加密模式</param>
    /// <returns>解密成功返回解密后的字符串,失败返回空</returns>
    public static string AesDecrypt(this string decryptString, string decryptKey,
        CipherMode mode = CipherMode.CBC)
    {
        try
        {
            decryptKey = GetSubString(decryptKey, 32, "");
            decryptKey = decryptKey.PadRight(32, ' ');
            using var rijndaelProvider = new RijndaelManaged()
            {
                Key = Encoding.UTF8.GetBytes(decryptKey),
                IV = Keys,
                Mode = mode
            };
            using var rijndaelDecrypt = rijndaelProvider.CreateDecryptor();
            var inputData = Convert.FromBase64String(decryptString);
            var decryptedData =
                rijndaelDecrypt.TransformFinalBlock(inputData, 0, inputData.Length);
            return Encoding.UTF8.GetString(decryptedData);
        }
        catch
        {
            return string.Empty;
        }
    }

    /// <summary>
    /// 对称加密算法AES RijndaelManaged解密字符串
    /// </summary>
    /// <param name="decryptString">待解密的字符串</param>
    /// <param name="options">加密选项</param>
    /// <returns>解密成功返回解密后的字符串,失败返回空</returns>
    public static string AesDecrypt(this string decryptString, RijndaelManaged options)
    {
        try
        {
            using var rijndaelDecrypt = options.CreateDecryptor();
            var inputData = Convert.FromBase64String(decryptString);
            var decryptedData =
                rijndaelDecrypt.TransformFinalBlock(inputData, 0, inputData.Length);
            return Encoding.UTF8.GetString(decryptedData);
        }
        catch
        {
            return string.Empty;
        }
    }

    /// <summary>
    /// 对称加密算法AES RijndaelManaged解密字符串
    /// </summary>
    /// <param name="decryptString">待解密的字符串</param>
    /// <param name="decryptKey">解密密钥,和加密密钥相同</param>
    /// <param name="mode">加密模式</param>
    /// <returns>解密成功返回解密后的字符串,失败返回空</returns>
    public static string AesDecrypt(this string decryptString, byte[] decryptKey,
        CipherMode mode = CipherMode.CBC)
    {
        try
        {
            using var rijndaelProvider = new RijndaelManaged()
            {
                Key = decryptKey,
                IV = Keys,
                Mode = mode
            };
            using var rijndaelDecrypt = rijndaelProvider.CreateDecryptor();
            var inputData = Convert.FromBase64String(decryptString);
            var decryptedData =
                rijndaelDecrypt.TransformFinalBlock(inputData, 0, inputData.Length);
            return Encoding.UTF8.GetString(decryptedData);
        }
        catch
        {
            return string.Empty;
        }
    }

    /// <summary>
    /// 按字节长度(按字节,一个汉字为2个字节)取得某字符串的一部分
    /// </summary>
    /// <param name="sourceString">源字符串</param>
    /// <param name="length">所取字符串字节长度</param>
    /// <param name="tailString">附加字符串(当字符串不够长时，尾部所添加的字符串，一般为"...")</param>
    /// <returns>某字符串的一部分</returns>
    private static string GetSubString(this string sourceString, int length,
        string tailString)
    {
        return GetSubString(sourceString, 0, length, tailString);
    }

    /// <summary>
    /// 按字节长度(按字节,一个汉字为2个字节)取得某字符串的一部分
    /// </summary>
    /// <param name="sourceString">源字符串</param>
    /// <param name="startIndex">索引位置，以0开始</param>
    /// <param name="length">所取字符串字节长度</param>
    /// <param name="tailString">附加字符串(当字符串不够长时，尾部所添加的字符串，一般为"...")</param>
    /// <returns>某字符串的一部分</returns>
    private static string GetSubString(this string sourceString, int startIndex,
        int length, string tailString)
    {
        //当是日文或韩文时(注:中文的范围:\u4e00 - \u9fa5, 日文在\u0800 - \u4e00, 韩文为\xAC00-\xD7A3)
        if (Regex.IsMatch(sourceString, "[\u0800-\u4e00]+") ||
            Regex.IsMatch(sourceString, "[\xAC00-\xD7A3]+"))
        {
            //当截取的起始位置超出字段串长度时
            if (startIndex >= sourceString.Length)
            {
                return string.Empty;
            }

            return sourceString.Substring(startIndex,
                length + startIndex > sourceString.Length
                    ? (sourceString.Length - startIndex) : length);
        }

        //中文字符，如"中国人民abcd123"
        if (length <= 0)
        {
            return string.Empty;
        }

        var bytesSource = Encoding.Default.GetBytes(sourceString);

        //当字符串长度大于起始位置
        if (bytesSource.Length <= startIndex)
        {
            return string.Empty;
        }

        var endIndex = bytesSource.Length;

        //当要截取的长度在字符串的有效长度范围内
        if (bytesSource.Length > (startIndex + length))
        {
            endIndex = length + startIndex;
        }
        else
        {
            //当不在有效范围内时,只取到字符串的结尾
            length = bytesSource.Length - startIndex;
            tailString = "";
        }

        var anResultFlag = new int[length];
        var nFlag = 0;
        //字节大于127为双字节字符
        for (var i = startIndex; i < endIndex; i++)
        {
            if (bytesSource[i] > 127)
            {
                nFlag++;
                if (nFlag == 3)
                {
                    nFlag = 1;
                }
            }
            else
            {
                nFlag = 0;
            }

            anResultFlag[i] = nFlag;
        }

        //最后一个字节为双字节字符的一半
        if ((bytesSource[endIndex - 1] > 127) && (anResultFlag[length - 1] == 1))
        {
            length++;
        }

        var bsResult = new byte[length];
        Array.Copy(bytesSource, startIndex, bsResult, 0, length);
        var myResult = Encoding.Default.GetString(bsResult);
        myResult += tailString;
        return myResult;
    }

    /// <summary>
    /// 加密文件流
    /// </summary>
    /// <param name="fs">需要加密的文件流</param>
    /// <param name="decryptKey">加密密钥</param>
    /// <param name="mode">加密模式</param>
    /// <returns>加密流</returns>
    public static CryptoStream AesEncryptStream(this FileStream fs, string decryptKey,
        CipherMode mode = CipherMode.CBC)
    {
        decryptKey = GetSubString(decryptKey, 32, "");
        decryptKey = decryptKey.PadRight(32, ' ');
        using var rijndaelProvider = new RijndaelManaged()
        {
            Key = Encoding.UTF8.GetBytes(decryptKey),
            IV = Keys,
            Mode = mode
        };
        using var encrypt = rijndaelProvider.CreateEncryptor();
        return new CryptoStream(fs, encrypt, CryptoStreamMode.Write);
    }

    /// <summary>
    /// 加密文件流
    /// </summary>
    /// <param name="fs">需要加密的文件流</param>
    /// <param name="decryptKey">加密密钥</param>
    /// <param name="mode">加密模式</param>
    /// <returns>加密流</returns>
    public static CryptoStream AesEncryptStream(this FileStream fs, byte[] decryptKey,
        CipherMode mode = CipherMode.CBC)
    {
        using var rijndaelProvider = new RijndaelManaged()
        {
            Key = decryptKey,
            IV = Keys,
            Mode = mode
        };
        using var encrypt = rijndaelProvider.CreateEncryptor();
        return new CryptoStream(fs, encrypt, CryptoStreamMode.Write);
    }

    /// <summary>
    /// 解密文件流
    /// </summary>
    /// <param name="fs">需要解密的文件流</param>
    /// <param name="decryptKey">解密密钥</param>
    /// <param name="mode">加密模式</param>
    /// <returns>加密流</returns>
    public static CryptoStream AesDecryptStream(this FileStream fs, string decryptKey,
        CipherMode mode = CipherMode.CBC)
    {
        decryptKey = GetSubString(decryptKey, 32, "");
        decryptKey = decryptKey.PadRight(32, ' ');
        using var rijndaelProvider = new RijndaelManaged()
        {
            Key = Encoding.UTF8.GetBytes(decryptKey),
            IV = Keys,
            Mode = mode
        };
        using var decrypt = rijndaelProvider.CreateDecryptor();
        return new CryptoStream(fs, decrypt, CryptoStreamMode.Read);
    }

    /// <summary>
    /// 解密文件流
    /// </summary>
    /// <param name="fs">需要解密的文件流</param>
    /// <param name="decryptKey">解密密钥</param>
    /// <param name="mode">加密模式</param>
    /// <returns>加密流</returns>
    public static CryptoStream AesDecryptStream(this FileStream fs, byte[] decryptKey,
        CipherMode mode = CipherMode.CBC)
    {
        using var rijndaelProvider = new RijndaelManaged()
        {
            Key = decryptKey,
            IV = Keys,
            Mode = mode
        };
        using var decrypt = rijndaelProvider.CreateDecryptor();
        return new CryptoStream(fs, decrypt, CryptoStreamMode.Read);
    }

    /// <summary>
    /// 对指定文件AES加密
    /// </summary>
    /// <param name="input">源文件流</param>
    /// <param name="mode">加密模式</param>
    /// <param name="outputPath">输出文件路径</param>
    public static void AesEncryptFile(this FileStream input, string outputPath,
        CipherMode mode = CipherMode.CBC)
    {
        using var fs = new FileStream(outputPath, FileMode.Create);
        using var encryptStream = AesEncryptStream(fs, DefaultAesKey, mode);
        var byteArrayInput = new byte[input.Length];
        var read = input.Read(byteArrayInput, 0, byteArrayInput.Length);
        encryptStream.Write(byteArrayInput, 0, byteArrayInput.Length);
    }

    /// <summary>
    /// 对指定的文件AES解密
    /// </summary>
    /// <param name="input">源文件流</param>
    /// <param name="mode">加密模式</param>
    /// <param name="outputPath">输出文件路径</param>
    public static void AesDecryptFile(this FileStream input, string outputPath,
        CipherMode mode = CipherMode.CBC)
    {
        using var fs = new FileStream(outputPath, FileMode.Create);
        using var decrypt = AesDecryptStream(input, DefaultAesKey, mode);
        var byteArrayOutput = new byte[1024];
        while (true)
        {
            var count = decrypt.Read(byteArrayOutput, 0, byteArrayOutput.Length);
            fs.Write(byteArrayOutput, 0, count);
            if (count < byteArrayOutput.Length)
            {
                break;
            }
        }
    }

    #endregion
}