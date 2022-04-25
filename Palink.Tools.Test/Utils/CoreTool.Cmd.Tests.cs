using System;
using Palink.Tools.Utility;
using Xunit;

namespace Palink.Tools.Test.Utils;

public partial class CoreToolTest
{
    [Fact]
    public void ShutdownTest()
    {
        CoreTool.TimedShutDown(DateTime.Parse("20:00"));
        // CoreTool.DelayShutdown(1000);
    }

    [Fact]
    public void RestartTest()
    {
        CoreTool.TimedRestart(DateTime.Parse("20:00"));
        // CoreTool.DelayRestart(1000);
    }

    [Fact]
    public void CancelTest()
    {
        CoreTool.CancelShutDown();
    }
}