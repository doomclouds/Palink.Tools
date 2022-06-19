using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Palink.Tools.Monitor;
using Xunit;

namespace Palink.Tools.Test.PanShi;

public class PostCacheTests
{
    private const string Uri = "https://127.0.0.1/";

    [Fact]
    public async void NormalTest()
    {
        var service = new PostCacheService<MyMessage>("PostCache");
        var msg = new MyMessage()
        {
            Hid = "6127-0FA5-5D69-2436-922E",
            Payway = "微信",
            CapitalAction = "收款",
            Amount = "18.6",
            DataSource = "PC",
            Time = "2022-03-31 10:31:14",
            TradeNo = "PC-??????????????????????",
            Url = Uri,
            Tag = MessageTag.AutoExpireNeeded,
            InfoContent = "流水数据",
            ETime = TimeSpan.FromMinutes(2),
            Id = Guid.NewGuid().ToString("N"),
        };
        service.AddMessage(msg);
        await Task.Delay(3000);
    }
}

public class MyMessage : Message
{
    [JsonProperty("hid")] public string Hid { get; set; }

    [JsonProperty("payway")] public string Payway { get; set; }

    [JsonProperty("capitalAction")] public string CapitalAction { get; set; }

    [JsonProperty("amount")] public string Amount { get; set; }

    [NotNull][JsonProperty("dataSource")] public string? DataSource { get; set; }

    [JsonProperty("time")] public string Time { get; set; }

    [JsonProperty("tradeNo")] public string TradeNo { get; set; }
}