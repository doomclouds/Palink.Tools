using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Palink.Tools.Extensions.ObjectExt;
using Palink.Tools.Freebus.Interface;
using Palink.Tools.Freebus.IO;
using Palink.Tools.IO;
using Palink.Tools.Logging;
using Palink.Tools.Robots.Resources;

namespace Palink.Tools.Robots.LQ;

public class LQTransport : FreebusTransport
{
    internal LQTransport(IStreamResource streamResource, IFreebusLogger logger) : base(
        streamResource, logger)
    {
        LQErrorInfos =
            JsonConvert.DeserializeObject<LQErrorInfos>(ErrorCode.LQCode);
    }

    public override List<string> IgnoreList { get; set; } = new();
    public override string? ErrorMessage { get; set; }

    public LQErrorInfos LQErrorInfos { get; set; }

    public override bool ValidateResponse(IFreebusContext context)
    {
        var res = context.GetDruString().Split('#');

        if (res.Contains(" "))
        {
            var code = res[0].Split(' ')[0];
            var codeInfo = LQErrorInfos.LQCodeInfos?.FirstOrDefault(x => x.Code == code);
            if (codeInfo.NotNull())
            {
                ErrorMessage = $"{codeInfo.Level}:{codeInfo.Message}";
            }
        }

        return res.Length > 1 && res[1].StartsWith("0");
    }
}