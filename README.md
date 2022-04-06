# Palink.Tools

#### 通讯

位于`Palink.Tools.Communication`名称空间下的`Master.cs`类是一个可以进行单播命令和广播命令的封装。通讯可以使用TCP、UDP及串口。

```c#
using System;
using System.Net.Sockets;
using Palink.Tools.Communication;
using Palink.Tools.Communication.Adapter;
using Palink.Tools.Communication.Interface;
using Palink.Tools.Communication.Message;
using Palink.Tools.Extensions.Logging;
using Xunit;

namespace Palink.Tools.Test.Communication;

public class MasterTest
{
    [Fact]
    public void MyMasterTest()
    {
        //tcp发送数据
        var tcp = new TcpClient();
        tcp.Connect("127.0.0.1", 9000);
        var adapter = new TcpClientAdapter(tcp);
        var master = new MyMaster(adapter, new ConsoleLogger(LoggingLevel.Debug));
        master.TestCmd();

        //udp发送数据
        var udp = new UdpClient();
        udp.Connect("127.0.0.1", 9000);
        var udpAdapter = new UdpClientAdapter(udp);
        var udpMaster = new MyMaster(udpAdapter, new ConsoleLogger(LoggingLevel.Debug));
        udpMaster.TestCmd();
    }
}

public class MyMaster : Master
{
    /// <summary>
    /// Master
    /// </summary>
    /// <param name="streamResource"></param>
    /// <param name="logger"></param>
    public MyMaster(IStreamResource streamResource, IPlLogger logger) : base(
        streamResource, logger)
    {
    }

    /// <summary>
    /// 数据校验
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    public override bool CheckData(IMessage message)
    {
        return message.Buffer.Length == 9;
    }

    /// <summary>
    /// 创建发送数据体
    /// </summary>
    /// <param name="noCheckFrame"></param>
    /// <returns></returns>
    public override IMessage CreateFrame((byte id, byte cmd, byte[] frame) noCheckFrame)
    {
        return null;
    }

    /// <summary>
    /// 创建字符串发送数据体
    /// </summary>
    /// <param name="frame"></param>
    /// <returns></returns>
    public override IMessage CreateStringFrame(string frame)
    {
        return null;
    }

    /// <summary>
    /// 测试命令
    /// </summary>
    public void TestCmd()
    {
        var message = new BaseMessage()
        {
            Data = new byte[]
            {
                0xa5,
                0x5a,
                01,
                02,
                03
            },
            SendBytes = 5,
            ReadBytes = 9
        };

        try
        {
            //单播命令，并等待返回9个字节的数据
            Unicast(message);

            //广播命令，无需等待返回
            // SendData(message);
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }
    }
}
```

#### 数组扩展

- ForEach提供遍历同时可以对数组进行操作、支持多维数组

#### 特性扩展

- GetDescription：获取枚举描述信息

#### Convert扩展

- TryToInt&TryToLong&TryToDouble&TryToDecimal&TryToFloat&TryToBool：string转int/long/double/decimal/float/bool
- TryToString：值类型转string
- TryToDateTime：字符串转时间
- TryToDateTime：时间戳转时间
- TryToDateTime：时间格式转换为字符串
- TryToEnum：字符串去空格
- TryToEnum：字符串转枚举
- TryToList：将枚举类型转换为List
- ConvertType：根据类型名返回一个Type类型

#### Encrypt扩展

包含Base64、MD5、DES、AES、哈希加密与SHA的相关加密解密方法

#### 网络功能扩展

- Ping：判断是否可以ping通该ip地址

#### 对象扩展

- IsPrimitive：判断对象是否是原始对象
- DeepClone：深度Clone
- IsNullOrEmpty&IsNotNull&ThrowIfNull:非空与不非空判断
- ReferenceEquals：引用是否相同判断
- IsDefaultValue：是否是默认值判断

#### 序列化扩展

- ToJson：实体转JSON
- FromJson：JSON转实体
- SerializeUtf8&DeserializeUtf8：字符串序列化成字节序列&字节序列序列化成字符串
- FromJsonFile：根据key将json文件内容转换为指定对象

#### 字符串扩展

- IsNullOrEmpty&IsNullOrWhiteSpace&IsNotNullOrEmpty&IsNotNullOrWhiteSpace：空判断
- 常用正则表达式判断：IsChinese&IsEmail&IsMobile&IsPhone&IsIp&IsIdCard&IsDate&IsNumeric&IsZipCode&IsImgFileName
- 字符串截取：TryReplace&Sub&RegexReplace
- Format&FormatWith：字符串格式化

#### 随机数扩展

- StrictNext：生成真正的随机数
- NextGauss：产生正态分布的随机数

#### 反射扩展

- InvokeMethod：执行方法
- SetField：设置字段
- GetField：获取字段
- GetFields：获取所有的字段信息
- SetProperty：设置属性
- GetProperty：获取属性
- GetProperties：获取所有的属性信息
- GetInstance：获取默认实例

#### 系统类

- HiPerfTimer：纳秒级计时器，仅支持Windows系统
- BaseAppSettings：appsettings.json配置文件数据读取类，抽象类需要继承去实现

#### 工具类

使用CoreTool去调用静态方法

- 文件及文件目录操作
- Win32 Api相关
- 全局鼠标键盘钩子(Hook)
- Modbus：CRC、LRC、HEX转Byte[]、Byte[]转ushort等
- Random：随机字符生成
- 注册表操作
- Windows任务计划操作
- 时间格式转化
- 软件相关：开机自启、防止多开、置顶、禁止触摸屏边缘侧滑等
- Shutdown相关：延时开启、延时重启等

#### Barrel

位于Palink.Tools.PLSystems.Caching.MonkeyCache.SQLite和Palink.Tools.PLSystems.Caching.MonkeyCache.FileStore分别是Sqlite缓存与文件缓存

```c#
//使用默认对象
var barrel = Barrel.Current;
//自定义对象
var _barrel = Barrel.Create($"{AppDomain.CurrentDomain.BaseDirectory}{Barrel.ApplicationId}");

//增加缓存
_barrel.Add(message.Id, message, message.ETime, message.GetTag());
//删除缓存
_barrel.Empty(msg.Id);
//获取缓存
var msg = _barrel.Get<EcmMessage>(key);
```

#### 磐石内部

##### 云监控

ECM云监控系统：心跳

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

ECM云监控：互动

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

ECM云监控：监控信息

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

##### 其他通用系统数据发送

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
