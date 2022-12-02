using System.Text;

namespace Palink.Tools.Extensions.SerializeExt;

/// <summary>
/// SerializeExtensions
/// </summary>
public static class SerializeExtensions
{
    public static byte[]? SerializeUtf8(this string? str)
    {
        return str == null ? null : Encoding.UTF8.GetBytes(str);
    }

    public static string? DeserializeUtf8(this byte[]? stream)
    {
        return stream == null ? null : Encoding.UTF8.GetString(stream);
    }
}