using System;
using System.Net.NetworkInformation;
using System.Text;
using Palink.Tools.Extensions.StringExt;

namespace Palink.Tools.Extensions.NetExt;

/// <summary>
/// NetExtensions
/// </summary>
public static class NetExtensions
{
    /// <summary>
    /// Ping
    /// </summary>
    /// <returns></returns>
    public static bool Ping(this string ip, bool checkIp = true)
    {
        if (checkIp && !ip.IsIp())
        {
            throw new Exception("The IP address is invalid");
        }

        try
        {
            var pingSender = new Ping();
            var options = new PingOptions
            {
                DontFragment = true
            };
            const string data = "Palink123@";
            var buffer = Encoding.ASCII.GetBytes(data);
            const int timeout = 120;
            //测试网络连接：目标计算机为www.baidu.com(可以换成你所需要的目标地址）
            //如果网络连接成功，PING就应该有返回；否则，网络连接有问题
            var reply = pingSender.Send(ip, timeout, buffer, options);
            return reply?.Status == IPStatus.Success;
        }
        catch (Exception)
        {
            return false;
        }
    }
}