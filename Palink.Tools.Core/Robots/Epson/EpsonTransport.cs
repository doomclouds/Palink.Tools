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
    public EpsonTransport(IStreamResource streamResource, IFreebusLogger logger) : base(
        streamResource, logger)
    {
    }

    public override bool ValidateResponse(IFreebusMessage message)
    {
        var dru = Encoding.UTF8.GetString(message.Dru);
        return dru.StartsWith("#");
    }

    public override List<string> IgnoreList { get; set; } = new()
    {
        //这里是测试需要忽略的数据帧
        "aa bb cc dd ee ff"
    };

    public override bool ShouldRetryResponse(IFreebusMessage message)
    {
        var hex = BitConverter.ToString(message.Dru).Replace('-', ' ');

        return IgnoreList.Contains(hex);
    }
}