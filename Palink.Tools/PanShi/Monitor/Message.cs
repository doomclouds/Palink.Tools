using Newtonsoft.Json;

namespace Palink.Tools.PanShi.Monitor;

/// <summary>
/// 消息类别
/// </summary>
public enum MessageType
{
    /// <summary>
    /// 普通消息收到就发送给服务器，不判断是否成功
    /// </summary>
    Normal,

    /// <summary>
    /// 该消息一经收到必须发送成功，失败后缓存
    /// </summary>
    Needed,

    /// <summary>
    /// 该消息5min内重复发送只发一次，必须成功
    /// </summary>
    FiveMinOnce = 5,

    /// <summary>
    /// 该消息10min内重复发送只发一次，必须成功
    /// </summary>
    TenMinOnce = 10,

    /// <summary>
    /// 该消息半小时重复发送只发送一次，必须成功
    /// </summary>
    HalfHourOnce = 30,

    /// <summary>
    /// 该消息1小时重复发送只发送一次，必须成功
    /// </summary>
    OneHourOnce = 60,

    /// <summary>
    /// 永久只发一次，必须成功
    /// </summary>
    ForeverOnce
}

/// <summary>
/// Message
/// </summary>
public abstract class Message
{
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
    /// 消息类型，用于区别如何处理该消息
    /// </summary>
    [JsonProperty("message_type")]
    public MessageType MessageType { get; set; }

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
}

/// <summary>
/// ECM消息工厂
/// </summary>
public static class MessageFactory
{
}