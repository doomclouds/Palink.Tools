using System;
using System.Collections.Generic;
using System.Text;
using Palink.Tools.Freebus.Interface;
using Palink.Tools.Freebus.IO;
using Palink.Tools.IO;
using Palink.Tools.Logging;

namespace Palink.Tools.Robots.Epson;

public class EpsonTransport : FreebusTransport
{
    internal EpsonTransport(IStreamResource streamResource, IFreebusLogger logger) : base(
        streamResource, logger)
    {
    }

    public override bool ValidateResponse(IFreebusContext context)
    {
        var dru = Encoding.UTF8.GetString(context.Dru);
        return dru.StartsWith("#");
    }

    public override List<string> IgnoreList { get; set; } = new()
    {
        //这里是测试需要忽略的数据帧
        "aa bb cc dd ee ff"
    };

    public override string? ErrorMessage { get; set; }

    public override bool ShouldRetryResponse(IFreebusContext context)
    {
        var hex = BitConverter.ToString(context.Dru).Replace('-', ' ');

        return IgnoreList.Contains(hex);
    }
}