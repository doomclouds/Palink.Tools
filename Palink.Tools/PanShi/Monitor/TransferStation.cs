using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace Palink.Tools.PanShi.Monitor;

/// <summary>
/// TransferStation
/// </summary>
public static class TransferStation
{
    /// <summary>
    /// 发送数据
    /// </summary>
    /// <param name="msg">消息体</param>
    /// <param name="url"></param>
    /// <param name="wait">是否等待服务器返回值</param>
    public static async Task<bool> PostJsonToServer<T>(this T msg, string url,
        bool wait = false) where T : Message
    {
        try
        {
            using var client = new HttpClient();
            var response = await client.PostAsJsonAsync(url, msg);

            if (!wait)
            {
                return true;
            }

            return response.StatusCode == HttpStatusCode.OK;
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            return false;
        }
    }
}