using System;
using Newtonsoft.Json;

namespace Palink.Tools.PanShi.Monitor.Ecm;

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
    /// 消息类型，用于区别如何处理该消息
    /// </summary>
    [JsonProperty("message_type")]
    public MessageType MessageType { get; set; }


    /// <summary>
    /// 服务器URL
    /// </summary>
    [JsonProperty("url")]
    public string Url { get; set; }

    /// <summary>
    /// 类唯一编码
    /// </summary>
    [JsonProperty("id")]
    public string Id { get; set; }

    /// <summary>
    /// 是否发送成功
    /// </summary>
    [JsonProperty("send_succeed")]
    public bool SendSucceed { get; set; }

    private EcmMessage()
    {
    }

    /// <summary>
    /// EcmMessage构造器
    /// </summary>
    /// <param name="name"></param>
    /// <param name="infoType"></param>
    /// <param name="infoCode"></param>
    /// <param name="infoContent"></param>
    /// <param name="cmdType"></param>
    /// <param name="messageType"></param>
    public EcmMessage(string name, string infoType, string infoCode,
        string infoContent, CmdType cmdType, MessageType messageType)
    {
        Name = name;
        InfoType = infoType;
        InfoCode = infoCode;
        InfoContent = infoContent;
        CmdType = cmdType;
        MessageType = messageType;
        Id = Guid.NewGuid().ToString("N");
    }
}

/// <summary>
/// ECM消息工厂
/// </summary>
public static class EcmMessageFactory
{
    /// <summary>
    /// 心跳
    /// </summary>
    /// <param name="service"></param>
    /// <param name="type"></param>
    /// <returns></returns>
    public static EcmMessage BeatsInstance(this EcmService service, MessageType type)
    {
        var em = new EcmMessage(service.ExhibitNo, "M", "001", "心跳", CmdType.Beats, type)
        {
            Url = service.Url
        };
        return em;
    }

    /// <summary>
    /// 互动
    /// </summary>
    /// <param name="service"></param>
    /// <param name="type"></param>
    /// <returns></returns>
    public static EcmMessage InteractionInstance(this EcmService service,
        MessageType type)
    {
        var em = new EcmMessage(service.ExhibitNo, "M", "101", "互动次数",
            CmdType.Interaction, type)
        {
            Url = service.Url
        };
        return em;
    }

    /// <summary>
    /// 监控
    /// </summary>
    /// <param name="service"></param>
    /// <param name="infoType"></param>
    /// <param name="infoCode"></param>
    /// <param name="infoContent"></param>
    /// <param name="type"></param>
    /// <returns></returns>
    /// <exception cref="OperationCanceledException"></exception>
    public static EcmMessage MonitorInstance(this EcmService service, string infoType,
        string infoCode, string infoContent, MessageType type)
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

        var em = new EcmMessage(service.ExhibitNo, infoType, infoCode, infoContent,
            CmdType.Monitor, type)
        {
            Url = service.Url
        };
        return em;
    }
}