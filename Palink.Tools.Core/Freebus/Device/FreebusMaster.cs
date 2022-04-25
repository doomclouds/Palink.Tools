﻿using Palink.Tools.Freebus.Interface;

namespace Palink.Tools.Freebus.Device;

public abstract class FreebusMaster : FreebusDevice, IFreebusMaster
{
    protected FreebusMaster(IFreebusTransport transport) : base(transport)
    {
    }

    public virtual IFreebusMessage ExecuteCustomMessage(IFreebusMessage request)
    {
        return Transport.UnicastMessage(request);
    }
}