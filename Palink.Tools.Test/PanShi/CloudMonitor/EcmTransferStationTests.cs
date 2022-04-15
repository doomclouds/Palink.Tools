using System;
using System.Threading.Tasks;
using Palink.Tools.PanShi.Monitor;
using Palink.Tools.PanShi.Monitor.Ecm;
using Xunit;

namespace Palink.Tools.Test.PanShi.CloudMonitor;

public class EcmTransferStationTests
{
    private const string Uri = "http://127.0.0.1/";
    [Fact]
    public async void BeatsTest()
    {
        const string exhibitNo = "001001001BH0027-38";
        var service = new EcmService(5, exhibitNo, Uri, "EcmCache");

        // var ret = service.BeatsInstance(MessageType.Normal)
        //     .SendDataToEcm();
        service.AddMessage(EcmMessage.BeatsInstance(exhibitNo));

        await Task.Delay(3000);
    }

    [Fact]
    public async void InteractionTest()
    {
        const string exhibitNo = "001001001BH0027-38";
        var service = new EcmService(5, exhibitNo, Uri, "EcmCache");

        service.AddMessage(
            EcmMessage.InteractionInstance(exhibitNo, TimeSpan.FromHours(1)));

        await Task.Delay(3000);
    }

    [Fact]
    public async void ErrorTest()
    {
        const string exhibitNo = "001001001BH0027-38";
        var service = new EcmService(5, exhibitNo, Uri, "EcmCache");

        service.AddMessage(EcmMessage.MonitorInstance(exhibitNo, "打败守卫测试Error", "E",
            TimeSpan.FromMinutes(3), MessageTag.AutoExpire));

        await Task.Delay(3000);
    }
}