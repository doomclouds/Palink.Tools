using System;
using System.Net.Sockets;
using Palink.Tools.IO;
using Palink.Tools.Logging;
using Palink.Tools.NModbus;
using Palink.Tools.NModbus.Interfaces;
using Palink.Tools.NModbus.IO;
using Palink.Tools.Robots.Epson;
using Palink.Tools.Robots.YzAim;

namespace Palink.Tools.Freebus;

public class FreebusFactory
{
    public static EpsonMaster CreateEpsonMaster(IStreamResource streamResource,
        IFreebusLogger logger)
    {
        var transport = new EpsonTransport(streamResource, logger);
        return new EpsonMaster(transport);
    }

    public static YzAimMaster CreateYzAimMaster(ModbusFactory factory,
        Socket socket)
    {
        if (socket.ProtocolType != ProtocolType.Tcp)
            throw new ArgumentException("仅支持TCP模式");
        var master = factory.CreateMaster(socket);
        return new YzAimMaster(master, factory.Logger);
    }
}