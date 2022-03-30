using System;
using Newtonsoft.Json;
using Palink.Tools.Extensions;

namespace Palink.Tools.PanShi.Monitor;

/// <summary>
/// 消息标签
/// </summary>
public enum MessageTag
{
    /// <summary>
    /// 只发送一次
    /// </summary>
    Once,

    /// <summary>
    /// 直到发送成功为止，无论之前是否发送过
    /// </summary>
    Needed,

    /// <summary>
    /// 只发送一次，过期时间内不重复发送
    /// </summary>
    AutoExpire,

    /// <summary>
    /// 直到发送成功为止，过期时间内不重复发送
    /// </summary>
    AutoExpireNeeded
}

/// <summary>
/// Message
/// </summary>
public abstract class Message
{
    private const string TagKey = "{0}:{1}";

    /// <summary>
    /// 消息唯一标识
    /// </summary>
    public string Id { get; set; }

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
    /// 消息有效时长
    /// </summary>
    [JsonProperty("eTime")]
    public TimeSpan ETime { get; set; }

    /// <summary>
    /// 消息标签
    /// </summary>
    [JsonProperty("tang")]
    public MessageTag Tag { get; set; }

    /// <summary>
    /// 是否发送成功
    /// </summary>
    public bool SendSucceed { get; set; }

    /// <summary>
    /// 获取当前消息缓存的标签
    /// </summary>
    /// <returns></returns>
    public string GetTag()
    {
        InfoContent.TryToEnum<MessageTag>();
        return TagKey.FormatWith(Tag.ToString(), InfoContent);
    }
}