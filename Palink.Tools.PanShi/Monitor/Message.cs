using System;
using System.Diagnostics.CodeAnalysis;
using Newtonsoft.Json;
using Palink.Tools.Extensions.ConvertExt;
using Palink.Tools.Extensions.StringExt;

namespace Palink.Tools.Monitor;

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
/// 继承此类子类使用CreateMessage方法构造，该方法4个参数必须存在
/// </summary>
public abstract class Message
{
    private const string TagKey = "{0}:{1}:{2}:{3}";

    /// <summary>
    /// 消息唯一标识
    /// </summary>
    [JsonProperty("id")]
    [NotNull]
    public string? Id { get; set; }

    /// <summary>
    /// 信息类型 E、M
    /// </summary>
    [JsonProperty("infoType")]
    [NotNull]
    public string? InfoType { get; set; }

    /// <summary>
    /// 信息代码000-999
    /// </summary>
    [JsonProperty("infoCode")]
    [NotNull]
    public string? InfoCode { get; set; }

    /// <summary>
    /// 信息内容
    /// </summary>
    [JsonProperty("infoContent")]
    [NotNull]
    public string? InfoContent { get; set; }

    /// <summary>
    /// 消息有效时长
    /// </summary>
    [JsonProperty("eTime")]
    public TimeSpan ETime { get; set; } = TimeSpan.FromSeconds(30);

    /// <summary>
    /// 消息标签
    /// </summary>
    [JsonProperty("tag")]
    public MessageTag Tag { get; set; }

    /// <summary>
    /// 是否发送成功
    /// </summary>
    [JsonProperty("sendSucceed")]
    public bool SendSucceed { get; set; }

    /// <summary>
    /// 将消息发送到服务器的URL地址
    /// </summary>
    [JsonProperty("url")]
    [NotNull]
    public string? Url { get; set; }

    /// <summary>
    /// 获取当前消息缓存的标签
    /// </summary>
    /// <returns></returns>
    public string GetTag()
    {
        if (Url.IsNullOrEmpty() || Id.IsNullOrEmpty() || InfoContent.IsNullOrEmpty())
        {
            throw new Exception("消息体的URL、Id、和InfoContent属性是必填的");
        }

        InfoContent.TryToEnum<MessageTag>();
        return TagKey.FormatWith(Tag.ToString(), InfoType, InfoCode, InfoContent);
    }

    /// <summary>
    /// 设置消息过期时间,不设置默认为30s
    /// </summary>
    public void SetupExpirationTime(TimeSpan time)
    {
        ETime = time;
    }
}