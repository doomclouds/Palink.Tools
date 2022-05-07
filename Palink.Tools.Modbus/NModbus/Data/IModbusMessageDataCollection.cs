namespace Palink.Tools.NModbus.Data;

internal interface IModbusMessageDataCollection
{
    /// <summary>
    ///     Gets the network bytes.
    /// </summary>
    byte[]? NetworkBytes { get; }

    /// <summary>
    ///     Gets the byte count.
    /// </summary>
    byte ByteCount { get; }
}