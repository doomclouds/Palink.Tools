using Palink.Tools.Freebus.Interface;

namespace Palink.Tools.Freebus.Device;

public abstract class FreebusMaster : FreebusDevice, IFreebusMaster
{
    protected FreebusMaster(IFreebusTransport transport) : base(transport)
    {
    }

    public virtual IFreebusContext ExecuteCustomMessage(IFreebusContext request)
    {
        return Transport.UnicastMessage(request);
    }

    public virtual void BroadcastMessage(IFreebusContext request, bool shouldLog)
    {
        Transport.BroadcastMessage(request, shouldLog);
    }
}