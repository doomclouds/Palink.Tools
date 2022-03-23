using System;
using Palink.Tools.Utils;
using Xunit;

namespace Palink.Tools.Test.Utils;

public partial class CoreToolTest
{
    [Fact]
    public void AutoStartTest()
    {
        CoreTool.AutoStart("HeavenlyRobot",
            @"C:\Resources\BaiduNetdiskWorkspace\磐石项目资料\2022\BH0027-34-天宫神臂\RobotProtocolTest\HeavenlyRobot\bin\Debug\net6.0-windows\HeavenlyRobot.exe",
            "HeavenlyRobot", TimeSpan.FromSeconds(5));
    }

    [Fact]
    public void AllowEdgeSwipeTest()
    {
        CoreTool.AllowEdgeSwipe(false);
    }
}