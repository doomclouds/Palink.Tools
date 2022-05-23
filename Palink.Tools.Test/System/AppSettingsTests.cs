using Palink.Tools.Extensions.ConvertExt;
using Palink.Tools.System.Configuration;
using Xunit;

namespace Palink.Tools.Test.System;

public class AppSettingsTests
{
    [Fact]
    public void AppSettingValueTest()
    {
        var com = AppSettings.RfidCom;
        var time = AppSettings.GameTime;
        //assert
        Assert.Equal("COM3", com);
        Assert.Equal(120, time);
    }

    [Fact]
    public void YAppSettingValueTest()
    {
        var version = YAppSettings.Version;
        var redisIsEnabled = YAppSettings.RedisIsEnabled.ToBool();
        //assert
        Assert.Equal("v4.0.0", version);
        Assert.True(redisIsEnabled);
    }
}

public class AppSettings : JsonAppSettings
{
    public static string RfidCom => Config["RfidCom"];

    public static int GameTime => Config["GameTime"].ToInt();
}

public class YAppSettings : YamlAppSettings
{
    public static string Version => Config["swagger:version"];
    public static string RedisIsEnabled => Config["storage:redisIsEnabled"];
}