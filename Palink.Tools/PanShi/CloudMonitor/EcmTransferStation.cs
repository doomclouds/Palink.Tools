using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net;
using System.Net.Http;
using Palink.Tools.Extensions;

namespace Palink.Tools.PanShi.CloudMonitor;

/// <summary>
/// 命令类型
/// </summary>
public enum CmdType
{
    /// <summary>
    /// 心跳
    /// </summary>
    [Description("getExhibitHeart")]Beats,

    /// <summary>
    /// 监控
    /// </summary>
    [Description("getExhibitMonitor")]Monitor,

    /// <summary>
    /// 互动
    /// </summary>
    [Description("getExhibitInteract")]Interaction,
}

/// <summary>
/// EcmTransferStation
/// </summary>
public static class EcmTransferStation
{
    /// <summary>
    /// 发送数据
    /// </summary>
    /// <param name="msg">消息体</param>
    /// <param name="wait">是否等待服务器返回值</param>
    public static bool SendDataToEcm(this EcmMessage msg, bool wait = false)
    {
        try
        {
            var authHost =
                $"{msg.Url}{msg.CmdType.GetDescription()}";
            var client = new HttpClient();
            var paraList = new List<KeyValuePair<string, string>>
            {
                new("no", msg.Name),
                new("m_type", msg.InfoType),
                new("m_code", msg.InfoCode),
                new("m_content", msg.InfoContent),
            };

            var response = client
                .PostAsync(authHost, new FormUrlEncodedContent(paraList))
                .Result;

            if (!wait)
            {
                return false;
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