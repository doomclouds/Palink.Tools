using System;
using Palink.Tools.Utility;

namespace Palink.Tools.Extensions.ByteExt;

public static class ByteExtensions
{
#if NET6_0_OR_GREATER
    public static string ToHex(this byte[] data)
    {
        return Convert.ToHexString(data);
    }
#endif

    public static byte[] ToBytes(this string hex)
    {
        return CoreTool.HexToBytes(hex);
    }
}