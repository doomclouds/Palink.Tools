using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Palink.Tools.IO;

namespace Palink.Tools.Utility;

public partial class CoreTool
{
    /// <summary>
    /// 读取ASCII对应的字符，不支持中文
    /// </summary>
    /// <param name="stream"></param>
    /// <param name="newLine"></param>
    /// <param name="returnNewLine"></param>
    /// <returns></returns>
    public static string ReadLine(IStreamResource stream, string newLine,
        bool returnNewLine = false)
    {
        var result = new StringBuilder();
        var singleByteBuffer = new byte[1];

        do
        {
            if (stream.Read(singleByteBuffer, 0, 1) == 0)
            {
                continue;
            }

            result.Append(Encoding.UTF8.GetChars(singleByteBuffer).First());
        } while (!result.ToString().EndsWith(newLine));

        return returnNewLine ? result.ToString() : result.ToString().Substring(0, result.Length - newLine.Length);
    }

    /// <summary>
    /// 读取UTF8对应的字符,支持中文
    /// </summary>
    /// <param name="stream"></param>
    /// <param name="newLine"></param>
    /// <param name="returnNewLine"></param>
    /// <returns></returns>
    public static string ReadLineByUTF8(IStreamResource stream, string newLine,
        bool returnNewLine = false)
    {
        var result = new StringBuilder();
        var singleByteBuffer = new byte[1];
        var buffer = Array.Empty<byte>();

        do
        {
            if (stream.Read(singleByteBuffer, 0, 1) == 0)
            {
                continue;
            }

            buffer = buffer.Concat(singleByteBuffer).ToArray();
            result.Append(Encoding.UTF8.GetChars(singleByteBuffer).First());
        } while (!result.ToString().EndsWith(newLine));

        var res = Encoding.UTF8.GetString(buffer);

        return returnNewLine ? res : res.Substring(0, res.Length - newLine.Length);
    }

    public static byte[] ReadLine(IStreamResource stream, int length)
    {
        var result = Array.Empty<byte>();
        var numArray = new byte[1];
        do
        {
            if (stream.Read(numArray, 0, 1) != 0)
                result = result.Append(numArray[0]).ToArray();
        } while (result.Length != length);

        return result;
    }

    public static byte[] ReadLine(IStreamResource stream, IReadOnlyList<byte> tail)
    {
        var result = Array.Empty<byte>();
        int length;
        var numArray = new byte[1];
        do
        {
            if (stream.Read(numArray, 0, 1) != 0)
                result = result.Append(numArray[0]).ToArray();
            length = result.Length;
        } while (!(result.Length > 2 && result[length - 2] == tail[0] && result[length - 1]
                     == tail[1]));

        return result;
    }
}