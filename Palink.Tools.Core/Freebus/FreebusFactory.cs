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
        var transport = new YzAimTransport(streamResource, logger);
        return new YzAimMaster(transport);
    }
}