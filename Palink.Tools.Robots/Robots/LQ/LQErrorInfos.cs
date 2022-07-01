using System.Collections.Generic;
using Newtonsoft.Json;

namespace Palink.Tools.Robots.LQ;

public class LQErrorInfos
{
    [JsonProperty("-Version")] public string? Version { get; set; }

    [JsonProperty("Info")] public List<LQCodeInfo>? LQCodeInfos { get; set; }
}


public class LQCodeInfo
{
    [JsonProperty("-code")] public string? Code { get; set; }

    [JsonProperty("-level")] public string? Level { get; set; }

    [JsonProperty("-message")] public string? Message { get; set; }
}