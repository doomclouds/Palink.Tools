using System;
using Palink.Tools.Freebus.Interface;
using Palink.Tools.Utility;

namespace Palink.Tools.Freebus.Device;

public abstract class FreebusDevice : IDisposable
{
    internal FreebusDevice(IFreebusTransport transport)
    {
        Transport = transport;
    }

    /// <summary>
    ///     Gets the Freebus Transport.
    /// </summary>
    public IFreebusTransport Transport { get; }

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