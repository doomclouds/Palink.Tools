using System;
using Palink.Tools.NModbus.Interfaces;
using Palink.Tools.Utility;

namespace Palink.Tools.NModbus.Device;

/// <summary>
///     Modbus device.
/// </summary>
internal abstract class ModbusDevice : IDisposable
{
    internal ModbusDevice(IModbusTransport transport)
    {
        Transport = transport;
    }

    /// <summary>
    ///     Gets the Modbus Transport.
    /// </summary>
    public IModbusTransport Transport { get; }

    /// <summary>
    ///     Releases unmanaged and - optionally - managed resources.
    /// </summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    ///     Releases unmanaged and - optionally - managed resources.
    /// </summary>
    /// <param name="disposing">
    ///     <c>true</c> to release both managed and unmanaged resources;
    ///     <c>false</c> to release only unmanaged resources.
    /// </param>
    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            CoreTool.Dispose(Transport);
        }
    }
}