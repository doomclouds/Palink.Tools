using System.Collections.Generic;
using Palink.Tools.Freebus.IO;
using Palink.Tools.IO;
using Palink.Tools.Logging;

namespace Palink.Tools.Robots.UPS;

public class UPSTransport : FreebusTransport
{
    internal UPSTransport(IStreamResource streamResource, IFreebusLogger logger) : base(
        streamResource, logger)
    {
    }

    public override List<string> IgnoreList { get; set; } = new();
    public override string? ErrorMessage { get; set; }
}