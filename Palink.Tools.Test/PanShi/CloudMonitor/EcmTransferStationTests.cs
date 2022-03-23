using Palink.Tools.PanShi.CloudMonitor;
using Xunit;

namespace Palink.Tools.Test.PanShi.CloudMonitor;

public class EcmTransferStationTests
{
    [Fact]
    public void BeatsTest()
    {
        const string exhibitNo = "001001001BH0027-34";
        EcmMessage.BeatsInstance(exhibitNo)
            .SendDataToEcm("http://15b4487o14.iok.la:13655/api/exhibit/");
    }

    [Fact]
    public void InteractionTest()
    {
        const string exhibitNo = "001001001BH0027-34";
        EcmMessage.InteractionInstance(exhibitNo)
            .SendDataToEcm("http://15b4487o14.iok.la:13655/api/exhibit/");
    }

    [Fact]
    public void ErrorTest()
    {
        const string exhibitNo = "001001001BH0027-34";
        EcmMessage.MonitorInstance(exhibitNo, "E", "100", "天宫神臂测试异常输出")
            .SendDataToEcm("http://15b4487o14.iok.la:13655/api/exhibit/");
    }
}