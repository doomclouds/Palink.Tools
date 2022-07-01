using System;
using System.Collections.Generic;
using Palink.Tools.Freebus.Interface;
using Palink.Tools.Freebus.IO;
using Palink.Tools.IO;
using Palink.Tools.Logging;

namespace Palink.Tools.Robots.DiskWall;

public class DiskWallTransport : FreebusTransport
{
    internal DiskWallTransport(IStreamResource streamResource, IFreebusLogger logger) :
        base(streamResource, logger)
    {
    }

    public override List<string> IgnoreList { get; set; } = new();
    public override string? ErrorMessage { get; set; }

    public override bool ShouldRetryResponse(IFreebusContext context)
    {
        if (context.Dru[0] != 0xa5 || context.Dru[1] != 0x5a) return true;

        var sumInt = 0;
        for (var i = 0; i < context.Dru.Length - 1; i++)
        {
            sumInt += context.Dru[i];
        }

        var sum = BitConverter.GetBytes(sumInt);
        return sum[0] != context.Dru[context.Dru.Length - 1];
    }
}