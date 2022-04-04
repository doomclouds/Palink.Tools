using System.Globalization;
using System.Threading.Tasks;
using Palink.Tools.PLSystems;
using Xunit;
using Xunit.Abstractions;

namespace Palink.Tools.Test.PLSystems;

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