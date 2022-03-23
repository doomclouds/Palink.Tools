using System;
using System.Collections.Generic;
using System.ComponentModel;
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
    /// <param name="msg"></param>
    /// <param name="urlStart"></param>
    public static void SendDataToEcm(this EcmMessage msg, string urlStart)
    {
        try
        {
            var authHost =
                $"{urlStart}{msg.CmdType.GetDescription()}";
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
            //var result = response.Content.ReadAsStringAsync().Result;

            //return result == "ok";
        }
        catch (Exception)
        {
            // return false;
            return;
        }
    }
}