using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Palink.Tools.Extensions.NetExt;

namespace Palink.Tools.Monitor;

/// <summary>
/// MessageTransport
/// </summary>
public static class MessageTransport
{
    /// <summary>
    /// 发送数据
    /// </summary>
    /// <param name="msg">消息体</param>
    /// <param name="wait">是否等待服务器返回值</param>
    public static async Task<bool> PostJsonToServer<T>(this T msg, bool wait = false)
        where T : Message
    {
        try
        {
            using var client = new HttpClient();
            var response = await client.PostAsJsonAsync(msg.Url, msg);

            if (!wait)
            {
                return true;
            }
            // var rer = response.Content.ReadAsStringAsync().Result;
            return response.StatusCode == HttpStatusCode.OK;
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            return false;
        }
    }
}