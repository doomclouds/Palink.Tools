using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Palink.Tools.Extensions.ArrayExt;

namespace Palink.Tools.NModbus.Data;

internal class FileRecordCollection : IModbusMessageDataCollection
{
    private IReadOnlyList<byte>? _networkBytes;

    public FileRecordCollection(ushort fileNumber, ushort startingAddress, byte[] data)
    {
        Build(fileNumber, startingAddress, data);
        FileNumber = fileNumber;
        StartingAddress = startingAddress;
    }

    public FileRecordCollection(byte[] messageFrame)
    {
        var fileNumber =
            (ushort)IPAddress.NetworkToHostOrder(BitConverter.ToInt16(messageFrame, 4));
        var startingAddress =
            (ushort)IPAddress.NetworkToHostOrder(BitConverter.ToInt16(messageFrame, 6));
        var count =
            (ushort)IPAddress.NetworkToHostOrder(BitConverter.ToInt16(messageFrame, 8));
        var data = messageFrame.Slice(10, count * 2).ToArray();

        Build(fileNumber, startingAddress, data);
        FileNumber = fileNumber;
        StartingAddress = startingAddress;
    }

    private void Build(ushort fileNumber, ushort startingAddress, byte[] data)
    {
        if (data.Length % 2 != 0)
        {
            throw new FormatException("Number of bytes has to be even");
        }

        var values = new List<byte>
        {
            6, // Reference type, demanded by standard definition
        };

        void AddAsBytes(int value)
        {
            values.AddRange(
                BitConverter.GetBytes(
                    (ushort)IPAddress.HostToNetworkOrder((short)value)));
        }

        AddAsBytes(fileNumber);
        AddAsBytes(startingAddress);
        AddAsBytes(data.Length / 2);

        values.AddRange(data);

        DataBytes = data;
        _networkBytes = values;
    }

    /// <summary>
    /// The Extended Memory file number
    /// </summary>
    public ushort FileNumber { get; }

    /// <summary>
    /// The starting register address within the file.
    /// </summary>
    public ushort StartingAddress { get; }

    /// <summary>
    ///  The bytes written to the extended memory file.
    /// </summary>
    public IReadOnlyList<byte>? DataBytes { get; private set; }

    public byte[]? NetworkBytes => _networkBytes?.ToArray();

    /// <summary>
    ///     Gets the byte count.
    /// </summary>
    public byte ByteCount => (byte)(_networkBytes?.Count ?? 0);

    /// <summary>
    ///     Returns a <see cref="T:System.String" /> that represents the current <see cref="T:System.Object" />.
    /// </summary>
    /// <returns>
    ///     A <see cref="T:System.String" /> that represents the current <see cref="T:System.Object" />.
    /// </returns>
    public override string ToString()
    {
        return string.Concat("{",
            string.Join(", ", _networkBytes?.Select(v => v.ToString()).ToArray() ??
                              Array.Empty<string>()), "}");
    }
}