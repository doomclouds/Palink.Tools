using System.Collections.Generic;
using Palink.Tools.Freebus.Interface;
using Palink.Tools.Freebus.IO;
using Palink.Tools.IO;
using Palink.Tools.Logging;

namespace Palink.Tools.Robots.LQ;

public class LQTransport : FreebusTransport
{
    internal LQTransport(IStreamResource streamResource, IFreebusLogger logger) : base(
        streamResource, logger)
    {
    }

    public override List<string> IgnoreList { get; set; } = new();

    public override bool ValidateResponse(IFreebusContext context)
    {
        var res = context.GetDruString().Split('#');
        return res.Length > 1 && res[1].StartsWith("0");
    }
}