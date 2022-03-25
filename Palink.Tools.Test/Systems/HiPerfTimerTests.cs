using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using Palink.Tools.Systems;
using Xunit;
using Xunit.Abstractions;
using Timer = System.Timers.Timer;

namespace Palink.Tools.Test.Systems;

public class HiPerfTimerTests
{
    private readonly ITestOutputHelper _testOutputHelper;

    public HiPerfTimerTests(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
    }

    [Fact]
    public async void UsedTimeTest()
    {
        var timer = new HiPerfTimer();
        timer.Start();
        var t = await HiPerfTimer.Execute(async () =>
        {
            for (var i = 0; i < 10; i++)
            {
                await Task.Delay(1);
            }
        });
        _testOutputHelper.WriteLine(t.ToString(CultureInfo.InvariantCulture));
    }
}