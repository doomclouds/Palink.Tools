using System;
using Palink.Tools.IO;
using Palink.Tools.Logging;
using Palink.Tools.NModbus;
using Palink.Tools.NModbus.Extensions;
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

    public static YzAimMaster CreateYzAimMaster(IStreamResource streamResource,
        IFreebusLogger logger)
    {
        var factory = new ModbusFactory(logger: logger);

        if (streamResource.GetType().Name.ToLower().Contains("udp"))
            throw new ArgumentException("不支持UDP模式");
        var master = factory.CreateRtuMaster(streamResource);
        return new YzAimMaster(master, factory.Logger);
    }
}