using Palink.Tools.Extensions;
using Palink.Tools.Systems;
using Xunit;

namespace Palink.Tools.Test.Systems;

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