# Palink.Tools

详细教程已迁移到[Palink.Tools案例教程](https://palinkv.com/category/Palink.Tools/)

## 磐石内部

### 云监控

#### ECM云监控系统：心跳

```c#
public async void BeatsTest()
{
    const string exhibitNo = "001001001BH0027-38";
    const string url = "http://127.0.0.1/";
    var service = new EcmService(5, exhibitNo, url, "EcmCache");

    // var ret = service.BeatsInstance(MessageType.Normal)
    //     .SendDataToEcm();
    service.AddMessage(EcmMessage.BeatsInstance(exhibitNo));

    await Task.Delay(3000);
}
```

#### ECM云监控：互动

```c#
public async void InteractionTest()
{
    const string exhibitNo = "001001001BH0027-38";
    const string url = "http://127.0.0.1/";
    var service = new EcmService(5, exhibitNo, url, "EcmCache");

    service.AddMessage(
        EcmMessage.InteractionInstance(exhibitNo, TimeSpan.FromHours(1)));

    await Task.Delay(3000);
}
```

#### ECM云监控：监控信息

```c#
public async void ErrorTest()
{
    const string exhibitNo = "001001001BH0027-38";
    const string url = "http://127.0.0.1/";
    var service = new EcmService(5, exhibitNo, url, "EcmCache");

    service.AddMessage(EcmMessage.MonitorInstance(exhibitNo, "打败守卫测试Error", "E",
                                                  TimeSpan.FromMinutes(3), MessageTag.AutoExpire));

    await Task.Delay(3000);
}
```

云监控服务内部自动发送消息，对于消息也有自动缓存功能。分为以下几种类型

- Once：发送后不判断是否成功，不缓存
- Needed：发送必须成功，如果失败会自动缓存等待下次执行
- AutoExpire：发送后不判断是否成功，自动缓存，缓存存在期间不接受相同消息
- AutoExpireNeeded：发送必须成功，如果失败会自动缓存等待下次执行，缓存存在期间不接受相同消息

### 其他通用系统数据发送

支持与ECM一样的缓存与过期判断策略

```c#
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
        Url = "https://127.0.0.1",
        Tag = MessageTag.Needed,
        InfoContent = "流水数据",
        ETime = TimeSpan.FromMinutes(2),
        Id = Guid.NewGuid().ToString("N"),
    };
    service.AddMessage(msg);
    await Task.Delay(3000);
}

public class MyMessage : Message
{
    [JsonProperty("hid")]
    public string Hid { get; set; }

    [JsonProperty("payway")]
    public string Payway { get; set; }

    [JsonProperty("capitalAction")]
    public string CapitalAction { get; set; }

    [JsonProperty("amount")]
    public string Amount { get; set; }

    [JsonProperty("dataSource")]
    public string DataSource { get; set; }

    [JsonProperty("time")]
    public string Time { get; set; }

    [JsonProperty("tradeNo")]
    public string TradeNo { get; set; }
}
```
