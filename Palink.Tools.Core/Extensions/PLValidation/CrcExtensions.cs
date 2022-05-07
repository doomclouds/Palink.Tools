using System;
using System.Linq;
using Palink.Tools.Utility;

namespace Palink.Tools.Extensions.PLValidation;

/// <summary>
/// CrcExtensions   
/// </summary>
public static class CrcExtensions
{
    /// <summary>
    /// DoesCrcMatch
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    public static bool DoesCrcMatch(this byte[] message)
    {
        var messageFrame = message.Take(message.Length - 2).ToArray();

        //Calculate the CRC with the given set of bytes
        var calculatedCrc =
            BitConverter.ToUInt16(CoreTool.CalculateCrc(messageFrame), 0);

        //Get the crc that is stored in the message
        var messageCrc = message.GetCrc();

        //Determine if they match
        return calculatedCrc == messageCrc;
    }

    private static ushort GetCrc(this byte[] message)
    {
        if (message == null)
            throw new ArgumentNullException(nameof(message));

        if (message.Length < 4)
            throw new ArgumentException("message must be at least four bytes long");

        return BitConverter.ToUInt16(message, message.Length - 2);
    }
}