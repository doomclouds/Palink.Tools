using System.Threading.Tasks;
using Palink.Tools.PanShi.CloudMonitor;
using Xunit;

namespace Palink.Tools.Test.PanShi.CloudMonitor;

public class EcmTransferStationTests
{
    [Fact]
    public async void BeatsTest()
    {
        const string exhibitNo = "001001001BH0027-38";
        const string url = "http://15b4487o14.iok.la:13655/api/exhibit/";
        var service = new EcmService(5, exhibitNo, url);

        // var ret = service.BeatsInstance(MessageType.Normal)
        //     .SendDataToEcm();
        service.AddMessage(service.BeatsInstance(MessageType.Normal));

        var count = 10;
        while (service.MessageCount != 0 && count > 0)
        {
            await Task.Delay(3000);
            count--;
        }

        Assert.Equal(count > 0, true);
    }

    [Fact]
    public async void InteractionTest()
    {
        const string exhibitNo = "001001001BH0027-38";
        const string url = "http://15b4487o14.iok.la:13655/api/exhibit/";
        var service = new EcmService(5, exhibitNo, url);

        service.AddMessage(service.InteractionInstance(MessageType.Needed));

        var count = 10;
        while (service.MessageCount != 0 && count > 0)
        {
            await Task.Delay(3000);
            count--;
        }

        Assert.Equal(count > 0, true);
    }

    [Fact]
    public async void ErrorTest()
    {
        const string exhibitNo = "001001001BH0027-38";
        const string url = "http://15b4487o14.iok.la:13655/api/exhibit/";
        var service = new EcmService(5, exhibitNo, url);

        service.AddMessage(service.MonitorInstance("E", "100", "打败守卫测试异常输出",
            MessageType.ForeverOnce));

        var count = 10;
        while (service.MessageCount != 0 && count > 0)
        {
            await Task.Delay(3000);
            count--;
        }

        Assert.Equal(count > 0, true);
    }
}