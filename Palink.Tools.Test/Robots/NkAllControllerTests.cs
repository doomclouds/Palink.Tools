using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using Palink.Tools.Robots.MultiaxialPlatform;
using Xunit;

namespace Palink.Tools.Test.Robots;

public class NkAllControllerTests
{
    [Fact]
    public async void NkAllControllerTest()
    {
        var udp = new UdpClient();
        const int delay = 1000;
        var data = NkAllController.BuildFBFDFrame(0, 0, z:-100);
        var str = BitConverter.ToString(data).Replace("-", " ");
        await udp.SendAsync(data, data.Length,
            new IPEndPoint(IPAddress.Parse("192.168.1.88"), 20000));
        for (var i = 0; i < 100; i++)
        {
            var random = new Random();
            await Task.Delay(delay);
            data = NkAllController.BuildFBFDFrame(random.Next(-10, 11),
                random.Next(-10, 11));
            await udp.SendAsync(data, data.Length,
                new IPEndPoint(IPAddress.Parse("192.168.1.88"), 20000));
        }
    }
}