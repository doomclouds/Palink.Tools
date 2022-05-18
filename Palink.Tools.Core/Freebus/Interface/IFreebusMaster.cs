using System;

namespace Palink.Tools.Freebus.Interface;

public interface IFreebusMaster : IDisposable
{
    IFreebusTransport Transport { get; }

    /// <summary>
    ///    Executes the custom message.
    /// </summary>
    /// <param name="request">The request.</param>
    IFreebusContext ExecuteCustomMessage(IFreebusContext request);

    /// <summary>
    /// Broadcast the custom message.
    /// </summary>
    /// <param name="request"></param>
    /// <param name="shouldLog"></param>
    void BroadcastMessage(IFreebusContext request, bool shouldLog);
}