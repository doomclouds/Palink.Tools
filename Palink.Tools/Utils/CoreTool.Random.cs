using System;

namespace Palink.Tools.Utils;

/// <summary>
/// CoreTool
/// </summary>
public partial class CoreTool
{
    /// <summary>
    /// 随机字符串，包含在0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ
    /// </summary>
    /// <param name="length"></param>
    /// <returns></returns>
    public static string GenerateString(int length)
    {
        const string sourceStr =
            "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
        var str = string.Empty;
        var random = new Random();
        for (var i = 0; i < length; ++i)
        {
            str += sourceStr.Substring(random.Next(sourceStr.Length), 1);
        }

        return str;
    }
}