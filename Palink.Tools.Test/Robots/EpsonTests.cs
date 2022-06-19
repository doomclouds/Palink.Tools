using System.Net.Sockets;
using System.Threading.Tasks;
using Palink.Tools.IO;
using Palink.Tools.Logging;
using Xunit;

namespace Palink.Tools.Test.Robots;

public class EpsonTests
{
    [Fact]
    public void LoginTest()
    {
        var tcp = new TcpClient("192.168.13.8", 5000);
        var adapter = new TcpClientAdapter(tcp)
        {
            ReadTimeout = 500,
            WriteTimeout = 500
        };
        var epson = RobotsFactory.CreateEpsonMaster(adapter, new DebugFreebusLogger());
        var ret = epson.Login("");
        Assert.True(ret);
    }

    [Fact]
    public async void LogoutTest()
    {
        var tcp = new TcpClient("192.168.13.8", 5000);
        var adapter = new TcpClientAdapter(tcp)
        {
            ReadTimeout = 500,
            WriteTimeout = 500
        };
        var epson = RobotsFactory.CreateEpsonMaster(adapter, new DebugFreebusLogger());
        await epson.LoginAsync("");
        await Task.Delay(1000);
        var ret = await epson.LogoutAsync();
        Assert.True(ret);
    }

    [Fact]
    public async void StartTest()
    {
        var tcp = new TcpClient("192.168.13.8", 5000);
        var adapter = new TcpClientAdapter(tcp)
        {
            ReadTimeout = 500,
            WriteTimeout = 500
        };
        var epson = RobotsFactory.CreateEpsonMaster(adapter, new DebugFreebusLogger());
        await epson.LoginAsync("");
        await Task.Delay(1000);
        var ret = await epson.StartAsync(1, 3000);
        Assert.True(ret);
    }

    [Fact]
    public async void MotorsOnAndOffAndHomeTests()
    {
        var tcp = new TcpClient("192.168.13.8", 5000);
        var adapter = new TcpClientAdapter(tcp)
        {
            ReadTimeout = 500,
            WriteTimeout = 500
        };
        var epson = RobotsFactory.CreateEpsonMaster(adapter, new DebugFreebusLogger());
        await epson.LoginAsync("");
        await Task.Delay(1000);
        await epson.SetMotorsOffAsync();
        await Task.Delay(1000);
        await epson.ResetAsync();
        await Task.Delay(1000);
        var ret = await epson.SetMotorsOnAsync();
        Assert.True(ret);
        await Task.Delay(1000);
        ret = await epson.HomeAsync();
        Assert.True(ret);
    }

    [Fact]
    public async void IoTests()
    {
        var tcp = new TcpClient("192.168.13.8", 5000);
        var adapter = new TcpClientAdapter(tcp)
        {
            ReadTimeout = 500,
            WriteTimeout = 500
        };
        var epson = RobotsFactory.CreateEpsonMaster(adapter, new DebugFreebusLogger());
        await epson.LoginAsync("");
        await Task.Delay(1000);
        var ret = await epson.GetIOAsync(0);
        Assert.True(!ret);
        await Task.Delay(1000);
        ret = await epson.SetIOAsync(2, true);
        Assert.True(ret);
    }

    [Fact]
    public async void ExecuteTest()
    {
        var tcp = new TcpClient("192.168.13.8", 5000);
        var adapter = new TcpClientAdapter(tcp)
        {
            ReadTimeout = 500,
            WriteTimeout = 500
        };
        var epson = RobotsFactory.CreateEpsonMaster(adapter, new DebugFreebusLogger());
        await epson.LoginAsync("");
        await Task.Delay(1000);
        await epson.ResetAsync();
        await Task.Delay(1000);
        var ret = await epson.SetMotorsOnAsync();
        Assert.True(ret);
        ret = await epson.ExecuteAsync("\"Go XY(400,50,0,0)\"", 6000);
        Assert.True(ret);
    }
}