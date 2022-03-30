using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestPlatform.CommunicationUtilities.ObjectModel;
using Palink.Tools.PanShi.Monitor;
using Palink.Tools.PanShi.Monitor.Ecm;
using Xunit;

namespace Palink.Tools.Test.PanShi.CloudMonitor;

public class EcmTransferStationTests
{
    [Fact]
    public async void BeatsTest()
    {
        const string exhibitNo = "001001001BH0027-38";
        const string url = "http://15b4487o14.iok.la:13655/api/exhibit/";
        var service = new EcmService(5, exhibitNo, url, "EcmCache");

        // var ret = service.BeatsInstance(MessageType.Normal)
        //     .SendDataToEcm();
        service.AddMessage(EcmMessage.BeatsInstance(exhibitNo));

        await Task.Delay(3000);
    }

    [Fact]
    public async void InteractionTest()
    {
        const string exhibitNo = "001001001BH0027-38";
        const string url = "http://15b4487o14.iok.la:13655/api/exhibit/";
        var service = new EcmService(5, exhibitNo, url, "EcmCache");

        service.AddMessage(
            EcmMessage.InteractionInstance(exhibitNo, TimeSpan.FromHours(1)));

        await Task.Delay(3000);
    }

    [Fact]
    public async void ErrorTest()
    {
        const string exhibitNo = "001001001BH0027-38";
        const string url = "http://15b4487o14.iok.la:13655/api/exhibit/";
        var service = new EcmService(5, exhibitNo, url, "EcmCache");

        service.AddMessage(EcmMessage.MonitorInstance(exhibitNo, "打败守卫测试Error", "E",
            TimeSpan.FromMinutes(3), MessageTag.AutoExpire));

        await Task.Delay(3000);
    }
}