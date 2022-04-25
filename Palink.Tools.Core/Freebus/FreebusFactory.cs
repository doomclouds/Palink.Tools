using Palink.Tools.IO;
using Palink.Tools.Logging;
using Palink.Tools.Robots.Epson;

namespace Palink.Tools.Freebus;

public class FreebusFactory
{
    public static EpsonMaster CreateEpsonMaster(IStreamResource streamResource,
        IFreebusLogger logger)
    {
        var transport = new EpsonTransport(streamResource, logger);
        return new EpsonMaster(transport);
    }
}