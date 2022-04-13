﻿using Palink.Tools.Extensions.PLConvert;
using Palink.Tools.System.PLConfiguration;
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
        Assert.Equal(com, "COM3");
        Assert.Equal(time, 120);
    }
}

public class AppSettings : BaseAppSettings
{
    public static string RfidCom => Config["RfidCom"];

    public static int GameTime => Config["GameTime"].TryToInt();
}