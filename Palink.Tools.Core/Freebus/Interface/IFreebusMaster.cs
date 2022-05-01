using System;

namespace Palink.Tools.Freebus.Interface;

public interface IFreebusMaster : IDisposable
{
    IFreebusTransport Transport { get; }

    /// <summary>
    ///    Executes the custom message.
    /// </summary>
    /// <param name="request">The request.</param>
    IFreebusMessage ExecuteCustomMessage(IFreebusMessage request);

    void BroadcastMessage(IFreebusMessage request, bool shouldLog);
}