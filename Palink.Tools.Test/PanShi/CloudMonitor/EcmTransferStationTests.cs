using System;
using System.Threading.Tasks;
using Palink.Tools.Monitor;
using Palink.Tools.Monitor.ECM;
using Xunit;

namespace Palink.Tools.Test.PanShi.CloudMonitor;

public class EcmTransferStationTests
{
    private const string Uri = "http://15b4487o14.iok.la:13655/api/exhibit/";

    [Fact]
    public async void BeatsTest()
    {
        const string exhibitNo = "001001001BH0027-38";
        var service = new ECMService(5, exhibitNo, Uri, "EcmCache");

        // var ret = service.BeatsInstance(MessageType.Normal)
        //     .SendDataToECM();
        service.AddMessage(service.CreateBeats());

        await Task.Delay(5000);
    }

    [Fact]
    public async void InteractionTest()
    {
        const string exhibitNo = "001001001BH0027-38";
        var service = new ECMService(5, exhibitNo, Uri, "EcmCache");

        service.AddMessage(service.CreateInteraction(TimeSpan.FromHours(1)));

        await Task.Delay(10000);
    }

    [Fact]
    public async void ErrorTest()
    {
        const string exhibitNo = "001001001BH0027-38";
        var service = new ECMService(5, exhibitNo, Uri, "EcmCache");

        service.AddMessage(service.CreateMonitor("打败守卫测试Error2", "E",
            TimeSpan.FromMinutes(3), MessageTag.AutoExpire));

        await Task.Delay(5000);
    }
}