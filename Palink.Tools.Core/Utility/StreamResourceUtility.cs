using System.Linq;
using System.Text;
using Palink.Tools.IO;

namespace Palink.Tools.Utility;

public partial class CoreTool
{
    public static string ReadLine(IStreamResource stream, string newLine)
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

        return result.ToString().Substring(0, result.Length - newLine.Length);
    }
}