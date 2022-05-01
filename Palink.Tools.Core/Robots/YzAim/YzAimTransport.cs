using System;
using System.Collections.Generic;
using Palink.Tools.Extensions.PLValidation;
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

    public override bool ValidateResponse(IFreebusMessage message)
    {
        if (message.Dru.Length < 2) return false;

        if (message.Pdu[1] == 0x7a && message.Dru[1] == message.Pdu[1])
        {
            return message.Pdu[message.Pdu.Length - 3] == message.Dru[0];
        }

        if (message.Dru[0] == message.Pdu[0] && message.Dru[1] == message.Pdu[1])
        {
            return message.Dru.DoesCrcMatch();
        }

        return false;
    }

    public override bool ShouldRetryResponse(IFreebusMessage message)
    {
        var hex = BitConverter.ToString(message.Dru).Replace('-', ' ');

        return IgnoreList.Contains(hex);
    }
}