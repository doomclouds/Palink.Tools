using System.Net.Sockets;
using Palink.Tools.Freebus;
using Palink.Tools.IO;
using Palink.Tools.Logging;
using Palink.Tools.Robots.LQ;
using Xunit;

namespace Palink.Tools.Test.Robots;

public class LQTests
{
    [Fact]
    public void LQTest()
    {
        var udp = new UdpClient("192.168.10.120", 2090);
        var adapter = new UdpClientOverCOMAdapter(udp);
        var master = RobotsFactory.CreateLQMaster(adapter, new ConsoleFreebusLogger());

        var res = master.Logout();
        Assert.True(res);

        res = master.Login(0);
        Assert.True(res);

        var state = master.GetState();

        res = master.Auto(false);
        Assert.True(res);

        res = master.Auto(true);
        Assert.True(res);

        if (!state.power)
        {
            res = master.PowerEnable(true);
            Assert.True(res);
        }

        if (!state.homed)
        {
            res = master.Home();
            Assert.True(res);
        }

        var speed = master.GetSpeed();

        if (speed < 50)
        {
            res = master.SetSpeed(speed + 1);
            Assert.True(res);
        }

        var sysSpeed = master.GetSysSpeed();
        if (sysSpeed < 50)
        {
            res = master.SetSysSpeed(50);
            Assert.True(res);
        }

        if (master.GetIO(0))
        {
            res = master.SetIO(0, false);
            Assert.True(res);
        }
        else
        {
            res = master.SetIO(0, true);
            Assert.True(res);
        }

        var pos = master.GetPosition();

        // while (true)
        // {
        //     SetPosition(master, pos.x + 300, pos.y, pos.z, 0);
        //     SetPosition(master, pos.x + 300, pos.y - 600, pos.z, 0);
        //     SetPosition(master, pos.x, pos.y - 600, pos.z, 0);
        //     SetPosition(master, pos.x, pos.y, pos.z, 0);
        //     SetPosition(master, pos.x + 300, pos.y - 300, pos.z, 0);
        // }
    }

    private void SetPosition(LQMaster master, double x, double y, double z, double u)
    {
        master.DeletePosition("p1");

        master.RecordPosition("p1", x, y, z, u, 0);

        master.ExecutePosition("p1");
    }
}