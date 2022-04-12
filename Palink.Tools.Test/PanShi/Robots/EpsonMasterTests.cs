using System.Net.Sockets;
using System.Threading.Tasks;
using Palink.Tools.Communication.Adapter;
using Palink.Tools.Extensions.PLLogging;
using Palink.Tools.PanShi.Robots;
using Xunit;

namespace Palink.Tools.Test.PanShi.Robots;

public class EpsonMasterTests
{
    [Fact]
    public void LoginTest()
    {
        var tcp = new TcpClient("192.168.13.8", 5000);
        var adapter = new TcpClientAdapter(tcp);
        var epson = new EpsonMaster(adapter, new ConsoleLogger(LoggingLevel.Debug));
        var ret = epson.Login("");
        Assert.True(ret);
    }

    [Fact]
    public async void LogoutTest()
    {
        var tcp = new TcpClient("192.168.13.8", 5000);
        var adapter = new TcpClientAdapter(tcp);
        var epson = new EpsonMaster(adapter, new ConsoleLogger(LoggingLevel.Debug));
        epson.Login("");
        await Task.Delay(1000);
        var ret = epson.Logout();
        Assert.True(ret);
    }

    [Fact]
    public async void StartTest()
    {
        var tcp = new TcpClient("192.168.13.8", 5000);
        var adapter = new TcpClientAdapter(tcp);
        var epson = new EpsonMaster(adapter, new ConsoleLogger(LoggingLevel.Debug));
        epson.Login("");
        await Task.Delay(1000);
        var ret = epson.Start(1, true, 3000);
        Assert.True(ret);
    }

    [Fact]
    public async void MotorsOnAndOffAndHomeTests()
    {
        var tcp = new TcpClient("192.168.13.8", 5000);
        var adapter = new TcpClientAdapter(tcp);
        var epson = new EpsonMaster(adapter, new ConsoleLogger(LoggingLevel.Debug));
        epson.Login("");
        await Task.Delay(1000);
        epson.SetMotorsOff();
        await Task.Delay(1000);
        epson.Reset();
        await Task.Delay(1000);
        var ret = epson.SetMotorsOn();
        Assert.True(ret);
        await Task.Delay(1000);
        ret = epson.Home();
        Assert.True(ret);
    }

    [Fact]
    public async void IoTests()
    {
        var tcp = new TcpClient("192.168.13.8", 5000);
        var adapter = new TcpClientAdapter(tcp);
        var epson = new EpsonMaster(adapter, new ConsoleLogger(LoggingLevel.Debug));
        epson.Login("");
        await Task.Delay(1000);
        var ret = epson.GetIO(0);
        Assert.True(!ret);
        await Task.Delay(1000);
        ret = epson.SetIO(2, true);
        Assert.True(ret);
    }
}