using System;
using System.Net.NetworkInformation;
using System.Text;

namespace Palink.Tools.Extensions
{
    /// <summary>
    /// 网络功能扩展
    /// </summary>
    public static class NetExtensions
    {
        /// <summary>
        /// Ping
        /// </summary>
        /// <returns></returns>
        public static bool Ping(this string ip)
        {
            if (!ip.IsIp())
            {
                throw new Exception("Net:IP地址不合法");
            }

            try
            {
                var pingSender = new Ping();
                var options = new PingOptions
                {
                    DontFragment = true
                };
                const string data = "doom1993";
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
}