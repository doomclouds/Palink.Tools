using System;
using Newtonsoft.Json;

namespace Palink.Tools.PanShi.CloudMonitor;

/// <summary>
/// EcmMessage
/// </summary>
public class EcmMessage
{
    /// <summary>
    /// 展品编号
    /// </summary>
    [JsonProperty("name")]
    public string Name { get; set; }

    /// <summary>
    /// 信息类型 E、M
    /// </summary>
    [JsonProperty("info_type")]
    public string InfoType { get; set; }

    /// <summary>
    /// 信息代码000-999
    /// </summary>
    [JsonProperty("info_code")]
    public string InfoCode { get; set; }

    /// <summary>
    /// 信息内容
    /// </summary>
    [JsonProperty("info_content")]
    public string InfoContent { get; set; }

    /// <summary>
    /// 命令类型
    /// </summary>
    [JsonProperty("cmd_type")]
    public CmdType CmdType { get; set; }

    /// <summary>
    /// EcmMessage
    /// </summary>
    public EcmMessage()
    {
    }

    private EcmMessage(string name, string infoType, string infoCode,
        string infoContent, CmdType cmdType)
    {
        Name = name;
        InfoType = infoType;
        InfoCode = infoCode;
        InfoContent = infoContent;
        CmdType = cmdType;
    }

    /// <summary>
    /// 心跳
    /// </summary>
    /// <param name="exhibitNo"></param>
    /// <returns></returns>
    public static EcmMessage BeatsInstance(string exhibitNo)
    {
        return new EcmMessage(exhibitNo, "M", "001", "心跳", CmdType.Beats);
    }

    /// <summary>
    /// 互动
    /// </summary>
    /// <param name="exhibitNo"></param>
    /// <returns></returns>
    public static EcmMessage InteractionInstance(string exhibitNo)
    {
        return new EcmMessage(exhibitNo, "M", "101", "互动次数",
            CmdType.Interaction);
    }

    /// <summary>
    /// 监控
    /// </summary>
    /// <param name="exhibitNo"></param>
    /// <param name="infoType"></param>
    /// <param name="infoCode"></param>
    /// <param name="infoContent"></param>
    /// <returns></returns>
    /// <exception cref="OperationCanceledException"></exception>
    public static EcmMessage MonitorInstance(string exhibitNo, string infoType,
        string infoCode, string infoContent)
    {
        if (infoType != "M" && infoType != "E")
        {
            throw new OperationCanceledException("信息类型是M或E类型");
        }

        int.TryParse(infoCode, out var code);
        if (code <= 101 && infoType == "M")
        {
            throw new OperationCanceledException("M类型的信息代码000-101已被系统使用");
        }

        return new EcmMessage(exhibitNo, infoType, infoCode, infoContent,
            CmdType.Monitor);
    }
}