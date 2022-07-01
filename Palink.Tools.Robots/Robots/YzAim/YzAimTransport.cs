using System;
using System.Collections.Generic;
using Palink.Tools.Extensions.ValidationExt;
using Palink.Tools.Freebus.Interface;
using Palink.Tools.Freebus.IO;
using Palink.Tools.IO;
using Palink.Tools.Logging;

namespace Palink.Tools.Robots.YzAim;

public class YzAimTransport : FreebusTransport
{
    internal YzAimTransport(IStreamResource streamResource, IFreebusLogger logger) : base(
        streamResource, logger)
    {
    }

    public override List<string> IgnoreList { get; set; } = new();
    public override string? ErrorMessage { get; set; }

    public override bool ValidateResponse(IFreebusContext context)
    {
        if (context.Dru.Length < 2) return false;

        if (context.Pdu[1] == 0x7a && context.Dru[1] == context.Pdu[1])
        {
            return context.Pdu[context.Pdu.Length - 3] == context.Dru[0];
        }

        if (context.Dru[0] == context.Pdu[0] && context.Dru[1] == context.Pdu[1])
        {
            return context.Dru.DoesCrcMatch();
        }

        return false;
    }

    public override bool ShouldRetryResponse(IFreebusContext context)
    {
        var hex = BitConverter.ToString(context.Dru).Replace('-', ' ');

        return IgnoreList.Contains(hex);
    }
}