[源码地址](https://github.com/doomclouds/Palink.Tools)，觉得本库可以帮助到你的小伙伴，麻烦点个小星星。

## version1.4.1

## Palink.Tools

### Palink.Tools.Extensions.ArrayExt

#### 多维数组遍历

```c#
//二维
var array = new[,]
{
    { 1, 2, 3 },
    { 4, 5, 6 },
    { 7, 8, 9 },
};
array.ForEach((arr, indices) =>
              {
                  var value = arr.GetValue(indices[0], indices[1]);
                  Console.WriteLine($"[{indices[0]},{indices[1]}] = {value}");
              });
```

#### 数组切片

```c#
var array = new[]
{
    1, 2, 3, 4, 5, 6
};
var res = array.Slice(1, 3);
```

#### 判断元素是否在数组内部

```c#
var array = new[]
{
    1, 2, 3, 4, 5, 6
};

var judge = 3;
var isIn = judge.IsIn(array);
```

#### 数组内容合并为字符串

```c#
var array = new[]
{
    1, 2, 3, 4, 5, 6
};

var res = array.JoinAsString(",");
//res = "1,2,3,4,5,6"
```

#### WhereIf筛选

```c#
var array = new[]
{
    1, 2, 3, 4, 5, 6
};

var condition = array.Length >= 2;
var res = array.WhereIf(condition, i => i > 2).ToList();
```



### Palink.Tools.Extensions.AttributeExt

#### 获取枚举Description

```c#
public enum MyEnum
{
    [Description("none")]
    None,
    [Description("text")]
    Test
}

var myEnum = MyEnum.None.EnumDescription();
```



### Palink.Tools.Extensions.ConvertExt

#### 类型转换

```c#
//类型转换
var test = "1.22";
var noByte = test.ToByte();
var noInt = test.ToInt();
var noLong = test.ToLong();
var noDouble = test.ToDouble();
var noDecimal = test.ToDecimal();
var noFloat = test.ToFloat();
var noBool = test.ToBool();

//To与As
noInt = test.To<int>();
test = noInt.As<string>();

//时间类型转换
var dateString = "2022-10-22 12:00:00";
var date = dateString.ToDateTime("yyyy-MM-dd HH:mm:ss");
dateString = date.ToDateString("yyyy/MM/dd");

//枚举转List
var enumList = typeof(MyEnum).TryToList();

//枚举转键值对(枚举为Key，描述为Value)
MyEnum.None.ToDic();
```



### Palink.Tools.Extensions.NetExt

#### Ping

```c#
var resBool = "192.168.7.33".Ping();
```

#### HttpClient发送与接收Json

```c#
//post
var http = new HttpClient();
http.PostAsJsonAsync("http://127.0.0.1", "json类型", default);

//put
var http = new HttpClient();
httpPutAsJsonAsync("http://127.0.0.1", "json类型", default);

//get
http.GetFromJsonAsync<string>("http://127.0.0.1", default);
```



### Palink.Tools.Extensions.ObjectExt

#### 对象判断

```c#
var app = new App();
var res = app.IsNull();
res = app.NotNull();

//条件判断
app.If(app.NotNull(), app1 =>
       {
           app.Run();
       });

//获取属性的DisplayName与Description
app.Name.PropertyDescription();
app.Name.PropertyDisplayName();
```



### Palink.Tools.Extensions.RandomExt

```c#
var r = new Random();
r.StrictNext();//真正随机数
r.NextGauss();//高斯随机数
```



### Palink.Tools.Extensions.ReflectionExt

```c#
var app = new App();

//克隆
var appClone = app.DeepClone();
//获取字段
var f = appClone.GetField<string>("fieldName");
//设置字段
appClone.SetField("fieldName", "value")
 //获取属性
var p = appClone.GetProperty<string>("propertyName");
//设置属性
appClone.SetProperty("propertyName", "value");
//实例化对象
var newApp = typeof(App).GetInstance<App>();
```



### Palink.Tools.Extensions.SecurityExt

#### 加密算法与解密算法

```c#
var palink = "palink";
//Base64
var encrypt = palink.Base64Encrypt();
var decrypt = palink.Base64Decrypt();
//MD5
encrypt = palink.MD5Encrypt();
//SHA
encrypt = palink.SHA1Encrypt();
encrypt = palink.SHA256SEncrypt();
encrypt = palink.SHA384Encrypt();
encrypt = palink.SHA512Encrypt();
//AES
encrypt = palink.AESEncrypt(...);
decrypt = palink.AESDecrypt(...);
//Hash
encrypt = palink.HashEncoding();
```



### Palink.Tools.Extensions.SerializeExt

```c#
//JSON
var json = (new App()).ToJson();
var app = json.FromJson<App>(json);

//字符串序列化
var str = "12121";
var data = str.SerializeUtf8();
str = data.DeserializeUtf8();
```



### Palink.Tools.Extensions.StringExt

#### 空判断

```c#
var str = "";
str.IsNullOrEmpty();
str.IsNullOrWhiteSpace();
str.IsNotNullOrEmpty();
str.IsNotNullOrWhiteSpace();
```

#### 正则表达式

```c#
var regex = "???";
regex.IsChinese();
regex.IsEmail();
regex.IsMobile();
regex.IsPhone();
regex.IsIp();
regex.IsIdCard();
regex.IsDate();
regex.IsNumeric();
regex.IsZipCode();
regex.IsImgFileName();
```

#### 字符串切片与替换

```c#
var str = "abcdef";
str.Slice(1,3);
str.TryReplace("a", "m");
str.RegexReplace("^[0-9]*$", "");
```

#### 格式化

```c#
var format = "a_{0}_{1}"
format.FormatWith("b", "c")
```



### Palink.Tools.Extensions.TimeExt

#### 常用

```c#
var date = DateTime.Now;
date.GetWeekAmount(); //获取某一年有多少周
date.WeekOfYear(); //返回年度第几个星期   默认星期日是第一天
date.WeekOfYear(DayOfWeek.Monday); //返回年度第几个星期, 可以设置周一是哪天
date.GetWeekTime(2022, 13, out var start, out var end); //得到一年中的某周的起始日和截止日
date.GetWeekWorkTime(2022, 13, out var start, out var end); //得到一年中的某周的起始日和截止日    周一到周五  工作日
date.SetLocalTime(); //设置本地计算机系统时间，仅支持Windows系统
```

#### 格林威治时间(1970)、北京时间

```c#
var date = DateTime.Now;
var long1 = date.BeijingTimeToUnixTimeStamp10(); //北京时间转换成unix时间戳(10位/秒)
var long2 = date.UtcTimeToUnixTimeStamp10(); //格林威治时间转换成unix时间戳(10位/秒)
var date1 = long1.UnixTimeStamp10ToBeijingTime(); //10位unix时间戳转换成北京时间
var date2 = long2.UnixTimeStamp10ToUtcTime(); // 10位unix时间戳转换成格林威治
...
```



### Palink.Tools.Extensions.ValidationExt

#### CRC

```c#
var bytes = new byte[]{1,2,3,4,5,6,crc_h,crc_l};
var boolRes = bytes.DoesCrcMatch();
var crc = bytes.Slice(0, 6).ToAarray().GetCrc();
```



### Freebus通讯

Freebus是一个自由通讯的封装，可以进行单播命令和广播命令。通讯可以使用TCP、UDP及串口。Freebus主要包含抽象类`FreebusMaster`与抽象类`FreebusTransport`。`FreebusMaster`用于协议数据的封装，负责数据整个命令的收发过程控制。`FreebusTransport`负责数据帧的读写。类库中实现了一个爱普生机械远程以太网控制的实现类，现在以`EpsonMaster`为例介绍如何实现自定义通讯类。

#### 实现FreebusTransport

```c#
public class EpsonTransport : FreebusTransport
{
    internal EpsonTransport(IStreamResource streamResource, IFreebusLogger logger) : base(
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
```

**要点：**

- `ValidateResponse`是接收数据进行校验的方法。`IFreebusMessage`接口提供接收数据缓存数组`Dru`。
- `ShouldRetryResponse`是判断是否出现不需要的数据，如果需要忽略该数据帧可以将其添加到`IgnoreList`集合内。

#### 实现FreebusMaster

```c#
public class EpsonMaster : FreebusMaster
{
  private const string NewLine = "\r\n";
  internal EpsonMaster(IFreebusTransport transport) : base(transport)
  {
  }
  private static string CreateCmd(string? cmd, string parameters)
  {
      return $"${cmd},{parameters}{NewLine}";
  }
  /// <summary>
  /// 机器人登录
  /// </summary>
  /// <returns></returns>
  public bool Login(string pwd)
  {
      try
      {
          var cmd = CreateCmd(MethodBase.GetCurrentMethod()?.Name, pwd);
          var message = new FreebusMessage();
          message.SetPdu(cmd);
          message.NewLine = NewLine;
          var ret = ExecuteCustomMessage(message);
          return ret.Succeed;
      }
      catch (Exception e)
      {
          Transport.Logger.Error(
              $"{MethodBase.GetCurrentMethod()?.Name}命令异常,{e.Message}");
          return false;
      }
  }
  public (bool running, bool safeguard, bool eStop, bool error, bool ready, bool auto)
      GetStatus()
  {
      try
      {
          var cmd = CreateCmd(MethodBase.GetCurrentMethod()?.Name, "");
          var message = new FreebusMessage();
          message.SetPdu(cmd);
          message.NewLine = NewLine;
          var ret = ExecuteCustomMessage(message).GetDruString();
          if (ret == "" || ret.Split(',')[1].Length != 11)
              return (false, false, false, false, false, false);
          var running = ret.Split(',')[1].Substring(9, 1) == "1";
          var safeguard = ret.Split(',')[1].Substring(5, 1) == "1";
          var eStop = ret.Split(',')[1].Substring(6, 1) == "1";
          var error = ret.Split(',')[1].Substring(7, 1) == "1";
          var ready = ret.Split(',')[1].Substring(10, 1) == "1";
          var auto = ret.Split(',')[1].Substring(2, 1) == "1";
          return (running, safeguard, eStop, error, ready, auto);
      }
      catch (Exception e)
      {
          Transport.Logger.Error(
              $"{MethodBase.GetCurrentMethod()?.Name}命令异常,{e.Message}");
          return (false, false, false, false, false, false);
      }
  }
}
```

- 机械臂登录的命令为`$Login,123456`。由于是字符串，需要将其转化为字节数组所有我们调用`message.SetPdu(cmd)`方法设置发送字节数组缓存`Pdu`。如果发送的直接就是字节数组，就可以对`Pdu`进行赋值。
- 在使用`ExecuteCustomMessage`方法执行收发流程之前还需要确认是按接收字节数去接收数据还是按数据帧结尾字符去接收数据。如果知道自己将要收到n个字节，可以设置`message.DruLength`为n后直接调用`ExecuteCustomMessage`执行收发流程。如果数据帧以某个字符结尾，如`\r\n`。我们设置`message.NewLine`为`\r\n`后就可以直接调用`ExecuteCustomMessage`。
- 最后是对返回结果的处理,可以参考上面的`GetStatus`方法



### Palink.Tools.Messaging

#### 消息订阅与发送

```c#
Messenger.Default.Register<string>(this, Console.WriteLine, token: "palink");
Messenger.Default.Send("bob", token: "palink");
```



### Palink.Tools.System

#### 中国日历

```c#
public void ChineseCalendarTest()
{
    var calendar = new ChineseCalendar(new DateTime(1993, 5, 9));
    var animal = calendar.AnimalString;
    var date = calendar.ChineseDateString;
    var constellation = calendar.ChineseConstellation;
    var tg = calendar.GanZhiYearString;
    Assert.Equal(animal, "鸡");
    Assert.Equal(date, "一九九三年闰三月十八");
    Assert.Equal(constellation, "柳土獐");
    Assert.Equal(tg, "癸酉年");
}
```

#### INI文件读写

```c#
var ini = new IniFile("path");
ini.IniWriteValue("sectionName", "key", "value"); //写ini文件
var ret = ini.IniReadValue("sectionName", "key") //读ini文件
ini.ClearAllSection(); //删除ini文件下所有段落
ini.ClearSection(); //删除ini文件下指定段落下的所有键
```

#### HighTimer(MS级高速定时器)

```c#
var timer = new HighTimer();
timer.Timer += Timer_Timer;
timer.Start(1, true);

private void Timer_Timer(object? sender, EventArgs e)
{
    Console.WriteLine("timer");
}
```



### Palink.Tools.Utility

#### Win32Api

```c#
CoreTool.SetForegroundWindow(IntPtr.Zero); //设置窗口为前景窗口
CoreTool.TopmostWin("winName"); //置顶窗口   
CoreTool.UnTopmostWin("winName"); //取消置顶窗口   
CoreTool.ShowWin("winName"); //显示窗口   
CoreTool.HideWin("winName"); //隐藏窗口   
CoreTool.SetWinPos("winName", 0, 0, true); //设置窗口位置   
```

#### StringExt

```c#
//创建OpenFileDialog的过滤器，因为总是忘记过滤器规则
var filters = new List<string>()
{
    "bmp","png"
};
CoreTool.BuilderFileFilter(filters, "图片");
```

#### Cmd

```c#
CoreTool.DelayShutdown(1);//延时1s关机
CoreTool.DelayRestart(1);//延时1s关机重启
CoreTool.TimedShutDown(...);//定时关机
CoreTool.TimedRestart(...);//定时关机并重启
CoreTool.CancelShutDown();//取消操作
```

#### Modbus

```c#
//CRC LRC
CoreTool.CalculateCrc(new byte[]{...});
CoreTool.CalculateLrc(new byte[]{...});
//Hex字符串转byte[]
var bytes = CoreTool.HexToBytes("a55aff");
//Converts four UInt16 values into a IEEE 64 floating point format.
CoreTool.GetDouble(...);
//Converts two UInt16 values into a IEEE 32 floating point format.
CoreTool.GetSingle(...);
//Converts two UInt16 values into a UInt32.
CoreTool.GetUInt32(...);
//Converts an array of bytes to an ASCII byte array.
var bytes = CoreTool.GetAsciiBytes(...);
//Converts an array of UInt16 to an ASCII byte array.
var bytes = CoreTool.GetAsciiBytes(...);
//Converts a network order byte array to an array of UInt16 values in host order.
var ushorts = CoreTool.NetworkBytesToHostUInt16(...);
```

#### Random

```c#
//随机字符串，包含在0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ
CoreTool.GenerateString(length : 32);
```

#### Software

```c#
//防止软件多开
CoreTool.SoftwareMutex(out var appMutex);
```

#### StreamResource

```c#
//以结束符\r\n读取一行数据
var udp = new UdpClient();
var adapter = new UdpClientAdapter(udp);
var data = CoreTool.ReadLine(adapter, "\r\n"); //不支持中文
var data2 = CoreTool.ReadLineByUTF8(adapter, "\r\n"); //支持中文
```



## Palink.Tools.Caching

`Palink.Tools.Caching`包可以在Nuget中搜索`Palink.Tools.Caching`获取最新版本

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



## Palink.Tools.Configuration

`Palink.Tools.Configuration`包可以在Nuget中搜索`Palink.Tools.Configuration`获取最新版本

### 配置文件

#### JSON配置

主要是一个`JsonAppSettings`的抽象类，如果需要读取`appsettings.json`配置文件数据，需要实现该类。

```c#
public class Appsettings : JsonAppSettings
{
    public string Mysql => Config["Mysql"];
}
//appsetting.json
{
    "Mysql":"Mysql ConnectionStrings"
}
```

#### YAML配置

主要是一个`YamlAppSettings`的抽象类，如果需要读取`appsettings.yml`配置文件数据，需要实现该类。

```c#
public class YAppSettings : YamlAppSettings
{
    public static string Version => Config["swagger:version"];
    public static string RedisIsEnabled => Config["storage:redisIsEnabled"];
}
//appsetting.yml
---
swagger:
  version: 'v4.0.0'
  name: 'palink.blog'
  title: 'palink.blog api'
  description: 'Powered by .NET 5 on Linux'
  routePrefix: ''
  documentTitle: '🤣Api'
sckey: 'SCU60393T5a94df1d5a9274125293f34a6acf928f5d78f551cf6d6'
storage:
  #数据库连接字符串
  mongodb: 'mongodb://localhost/palink_blog'
  redisIsEnabled: true
  redis: '127.0.0.1'
```



## Palink.Tools.Modbus

如要使用Modbus，请安装包`Palink.Tools.Modbus`

#### Modbus RTU

使用串口需要安装`Palink.Tools.Serial`或`Palink.Tools.SerialPortStream`

```c#
public void ModbusRtuReadWriteCoilsOverSerialPortTest()
{
    var serialPort = new SerialPort("COM2");
    serialPort.Open();
    var adapter = new SerialPortAdapter(serialPort)
    {
        ReadTimeout = 500,
        WriteTimeout = 500
    };
    var master = (new ModbusFactory()).CreateRtuMaster(adapter);
    master.WriteMultipleCoils(1, 0, new[]
    {
        true, true, false, false, false,
        true, true, false, false, false,
    });
    var ret = master.ReadCoils(1, 0, 10);
    Assert.True(ret[0]);
    Assert.True(ret[1]);
    Assert.True(ret[5]);
    Assert.True(ret[6]);
}
```

#### Modbus RTU Over TCP

```c#
public void ModbusRtuReadWriteCoilsOverTcpTest()
{
    var tcp = new Socket(SocketType.Stream, ProtocolType.Tcp);
    tcp.Connect("127.0.0.1", 502);
    var adapter = new SocketAdapter(tcp)
    {
        ReadTimeout = 500,
        WriteTimeout = 500
    };
    var master = (new ModbusFactory()).CreateRtuMaster(adapter);
    master.WriteMultipleCoils(1, 0, new[]
    {
        true, true, false, false, false,
        true, true, false, false, false,
    });
    var ret = master.ReadCoils(1, 0, 10);
    Assert.True(ret[0]);
    Assert.True(ret[1]);
    Assert.True(ret[5]);
    Assert.True(ret[6]);
}
```

#### Modbus TCP/IP

```c#
public void ModbusRtuReadWriteCoilsOverTcpTest()
{
    var tcp = new TcpClient();
    tcp.Connect("127.0.0.1", 502);
    var master = (new ModbusFactory()).CreateMaster(tcp);
    master.WriteMultipleCoils(1, 0, new[]
    {
        true, true, false, false, false,
        true, true, false, false, false,
    });
    var ret = master.ReadCoils(1, 0, 10);
    Assert.True(ret[0]);
    Assert.True(ret[1]);
    Assert.True(ret[5]);
    Assert.True(ret[6]);
}
```

#### Modbus UDP/IP

```c#
public void ModbusRtuReadWriteCoilsOverTcpTest()
{
    var udp = new UdpClient();
    udp.Connect("127.0.0.1", 502);
    var master = (new ModbusFactory()).CreateMaster(tcp);
    master.WriteMultipleCoils(1, 0, new[]
    {
        true, true, false, false, false,
        true, true, false, false, false,
    });
    var ret = master.ReadCoils(1, 0, 10);
    Assert.True(ret[0]);
    Assert.True(ret[1]);
    Assert.True(ret[5]);
    Assert.True(ret[6]);
}
```



## Palink.Tools.Robots

### Epson机械臂

```c#
public void LoginTest()
{
    var tcp = new TcpClient("192.168.13.8", 5000);
    var adapter = new TcpClientAdapter(tcp)
    {
        ReadTimeout = 500,
        WriteTimeout = 500
    };
    var epson = FreebusFactory.CreateEpsonMaster(adapter, new DebugFreebusLogger());
    var ret = epson.Login("");
    Assert.True(ret);
}
```

### YzAim直流伺服

```c#
public void FunctionTests()
{
    const byte situation = 0x01;
    const ushort myAcc = 15000;
    const ushort mySpeed = 1500;
    var client = new TcpClient();
    client.Connect("192.168.0.8", 503);
    var adapter = new TcpClientAdapter(client);
    adapter.ReadTimeout = 500;
    adapter.WriteTimeout = 500;
    var yzAim = FreebusFactory.CreateYzAimMaster(adapter, NullFreebusLogger.Instance);
    var ret = yzAim.ModifyId(situation, 100);
    ret = yzAim.ModifyId(100, situation);
    var id = yzAim.GetYzAimStatusCmd(situation, YzAimCmd.Address);
    var speed = yzAim.GetYzAimStatusCmd(situation, YzAimCmd.TargetSpeed);
    var acc = yzAim.GetYzAimStatusCmd(situation, YzAimCmd.Acc);
    var electricity = yzAim.GetActualElectricity(situation);
    var electricity1 =
        yzAim.GetYzAimStatusCmd(situation, YzAimCmd.Electricity);
    var voltage = yzAim.GetActualVoltage(situation);
    var voltage1 = yzAim.GetYzAimStatusCmd(situation, YzAimCmd.Voltage);
    var dir = yzAim.GetYzAimStatusCmd(situation, YzAimCmd.Dir);
    var errCode = yzAim.GetYzAimStatusCmd(situation, YzAimCmd.ErrCode);
    var temperature =
        yzAim.GetYzAimStatusCmd(situation, YzAimCmd.Temperature);
    yzAim.SetYzAimStatusCmd(situation, YzAimCmd.ModbusEnable, 1);
    yzAim.SetYzAimStatusCmd(situation, YzAimCmd.MotorEnable, 1);
    // yzAmi.SetPosition(2, -10000);
    yzAim.WriteAllMotionParams(new List<(int position, ushort speed, ushort acc)>()
    {
        (-10000, mySpeed, myAcc)
    });
    var group = new List<(byte id, int pos)> { (situation, -10000) };
    Task.Delay(100).Wait();
    var cSpeed = 0.0;
    while (!yzAim.AllReady(group))
    {
        var e = yzAim.GetActualElectricity(situation);
        var v = yzAim.GetActualVoltage(situation);
        var s = yzAim.GetActualSpeed(situation);
        var e1 = yzAim.GetYzAimStatusCmd(situation, YzAimCmd.Electricity);
        var v1 = yzAim.GetYzAimStatusCmd(situation, YzAimCmd.Voltage);
        if (e > electricity)
            electricity = e;
        if (v > voltage)
            voltage = v;
        if (e1 > electricity1)
            electricity1 = e1;
        if (v1 > voltage1)
            voltage1 = v1;
        if (Math.Abs(s) > Math.Abs(cSpeed))
            cSpeed = s;
        Task.Delay(100).Wait();
    }
    var pos = yzAim.GetPosition(situation);
    // yzAmi.SetPosition(2, -100000);
    yzAim.WriteAllMotionParams(new List<(int position, ushort speed, ushort acc)>()
    {
        (-100000, mySpeed, myAcc)
    });
    Task.Delay(100).Wait();
    group = new List<(byte id, int pos)> { (situation, -100000) };
    while (!yzAim.AllReady(group))
    {
        var e = yzAim.GetActualElectricity(situation);
        var v = yzAim.GetActualVoltage(situation);
        var s = yzAim.GetActualSpeed(situation);
        var e1 = yzAim.GetYzAimStatusCmd(situation, YzAimCmd.Electricity);
        var v1 = yzAim.GetYzAimStatusCmd(situation, YzAimCmd.Voltage);
        if (e > electricity)
            electricity = e;
        if (v > voltage)
            voltage = v;
        if (e1 > electricity1)
            electricity1 = e1;
        if (v1 > voltage1)
            voltage1 = v1;
        if (Math.Abs(s) > Math.Abs(cSpeed))
            cSpeed = s;
    }
    pos = yzAim.GetPosition(situation);
}
```

