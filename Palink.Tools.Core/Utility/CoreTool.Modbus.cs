using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace Palink.Tools.Utility;

/// <summary>
/// CoreTool
/// </summary>
public partial class CoreTool
{
    /// <summary>
    ///     Converts four UInt16 values into a IEEE 64 floating point format.
    /// </summary>
    /// <param name="b3">Highest-order ushort value.</param>
    /// <param name="b2">Second-to-highest-order ushort value.</param>
    /// <param name="b1">Second-to-lowest-order ushort value.</param>
    /// <param name="b0">Lowest-order ushort value.</param>
    /// <returns>IEEE 64 floating point value.</returns>
    public static double GetDouble(ushort b3, ushort b2, ushort b1, ushort b0)
    {
        var value = BitConverter.GetBytes(b0)
            .Concat(BitConverter.GetBytes(b1))
            .Concat(BitConverter.GetBytes(b2))
            .Concat(BitConverter.GetBytes(b3))
            .ToArray();

        return BitConverter.ToDouble(value, 0);
    }

    /// <summary>
    ///     Converts two UInt16 values into a IEEE 32 floating point format.
    /// </summary>
    /// <param name="highOrderValue">High order ushort value.</param>
    /// <param name="lowOrderValue">Low order ushort value.</param>
    /// <returns>IEEE 32 floating point value.</returns>
    public static float GetSingle(ushort highOrderValue, ushort lowOrderValue)
    {
        var value = BitConverter.GetBytes(lowOrderValue)
            .Concat(BitConverter.GetBytes(highOrderValue))
            .ToArray();

        return BitConverter.ToSingle(value, 0);
    }

    /// <summary>
    ///     Converts two UInt16 values into a UInt32.
    /// </summary>
    public static uint GetUInt32(ushort highOrderValue, ushort lowOrderValue)
    {
        var value = BitConverter.GetBytes(lowOrderValue)
            .Concat(BitConverter.GetBytes(highOrderValue))
            .ToArray();

        return BitConverter.ToUInt32(value, 0);
    }

    /// <summary>
    ///     Converts an array of bytes to an ASCII byte array.
    /// </summary>
    /// <param name="numbers">The byte array.</param>
    /// <returns>An array of ASCII byte values.</returns>
    public static byte[] GetAsciiBytes(params byte[] numbers)
    {
        return Encoding.UTF8.GetBytes(numbers.SelectMany(n => n.ToString("X2"))
            .ToArray());
    }

    /// <summary>
    ///     Converts an array of UInt16 to an ASCII byte array.
    /// </summary>
    /// <param name="numbers">The ushort array.</param>
    /// <returns>An array of ASCII byte values.</returns>
    public static byte[] GetAsciiBytes(params ushort[] numbers)
    {
        return Encoding.UTF8.GetBytes(numbers.SelectMany(n => n.ToString("X4"))
            .ToArray());
    }

    /// <summary>
    ///     Converts a network order byte array to an array of UInt16 values in host order.
    /// </summary>
    /// <param name="networkBytes">The network order byte array.</param>
    /// <returns>The host order ushort array.</returns>
    public static ushort[] NetworkBytesToHostUInt16(byte[] networkBytes)
    {
        if (networkBytes == null)
        {
            throw new ArgumentNullException(nameof(networkBytes));
        }

        if (networkBytes.Length % 2 != 0)
        {
            throw new FormatException(NetworkBytesNotEven);
        }

        var result = new ushort[networkBytes.Length / 2];

        for (var i = 0; i < result.Length; i++)
        {
            result[i] =
                (ushort)IPAddress.NetworkToHostOrder(
                    BitConverter.ToInt16(networkBytes, i * 2));
        }

        return result;
    }

    /// <summary>
    ///     Converts a hex string to a byte array.
    /// </summary>
    /// <param name="hex">The hex string.</param>
    /// <returns>Array of bytes.</returns>
    public static byte[] HexToBytes(string hex)
    {
        if (hex == null)
        {
            throw new ArgumentNullException(nameof(hex));
        }

        if (hex.Length % 2 != 0)
        {
            throw new FormatException(HexCharacterCountNotEven);
        }

        var bytes = new byte[hex.Length / 2];

        for (var i = 0; i < bytes.Length; i++)
        {
            bytes[i] = Convert.ToByte(hex.Substring(i * 2, 2), 16);
        }

        return bytes;
    }

    /// <summary>
    ///     Calculate Longitudinal Redundancy Check.
    /// </summary>
    /// <param name="data">The data used in LRC.</param>
    /// <returns>LRC value.</returns>
    public static byte CalculateLrc(byte[] data)
    {
        if (data == null)
        {
            throw new ArgumentNullException(nameof(data));
        }

        var lrc = data.Aggregate<byte, byte>(0, (current, b) => (byte)(current + b));

        lrc = (byte)((lrc ^ 0xFF) + 1);

        return lrc;
    }

    /// <summary>
    ///     Calculate Cyclical Redundancy Check.
    /// </summary>
    /// <param name="data">The data used in CRC.</param>
    /// <returns>CRC value.</returns>
    public static byte[] CalculateCrc(byte[] data)
    {
        if (data == null)
        {
            throw new ArgumentNullException(nameof(data));
        }

        var crc = ushort.MaxValue;

        foreach (var b in data)
        {
            var tableIndex = (byte)(crc ^ b);
            crc >>= 8;
            crc ^= CrcTable[tableIndex];
        }

        return BitConverter.GetBytes(crc);
    }

    /// <summary>
    ///     Convert the 32 bit registers to two 16 bit values.
    /// </summary>
    public static IEnumerable<ushort> Convert32To16(uint[] registers)
    {
        foreach (var register in registers)
        {
            // low order value
            yield return BitConverter.ToUInt16(BitConverter.GetBytes(register), 0);

            // high order value
            yield return BitConverter.ToUInt16(BitConverter.GetBytes(register), 2);
        }
    }

    /// <summary>
    ///     Convert the 16 bit registers to 32 bit registers.
    /// </summary>
    public static IEnumerable<uint> Convert16To32(IReadOnlyList<ushort> registers)
    {
        for (var i = 0; i < registers.Count; i++)
        {
            yield return GetUInt32(registers[i + 1], registers[i]);
            i++;
        }
    }
}