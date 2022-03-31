using System;

namespace Palink.Tools.PanShi.Monitor.Ecm;

/// <summary>
/// EcmMessage
/// </summary>
public class EcmMessage : Message
{
    /// <summary>
    /// 心跳实例
    /// </summary>
    /// <param name="no">展品编号</param>
    /// <returns></returns>
    public static EcmMessage BeatsInstance(string no)
    {
        var em = new EcmMessage(no, "M", "001", "心跳", CmdType.Beats,
            TimeSpan.FromSeconds(5),
            MessageTag.Once, Guid.NewGuid().ToString("N"));
        return em;
    }

    /// <summary>
    /// 互动实例
    /// </summary>
    /// <param name="no">展品编号</param>
    /// <param name="eTime">消息有效时长</param>
    /// <returns></returns>
    public static EcmMessage InteractionInstance(string no, TimeSpan eTime)
    {
        var em = new EcmMessage(no, "M", "101", "互动次数", CmdType.Interaction, eTime,
            MessageTag.Needed, Guid.NewGuid().ToString("N"));
        return em;
    }

    /// <summary>
    /// 监控实例
    /// </summary>
    /// <param name="no">展品编号</param>
    /// <param name="content">消息内容</param>
    /// <param name="type">消息类型M或E</param>
    /// <param name="eTime">消息有效时长</param>
    /// <param name="tag">消息类型，不同类型对应不同策略</param>
    /// <returns></returns>
    public static EcmMessage MonitorInstance(string no, string content, string type,
        TimeSpan eTime,
        MessageTag tag)
    {
        var em = new EcmMessage(no, type, "101", content, CmdType.Monitor, eTime, tag,
            Guid.NewGuid().ToString("N"));
        return em;
    }

    /// <summary>
    /// 展品编号
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// 命令类型
    /// </summary>
    public CmdType CmdType { get; set; }

    private EcmMessage(string no, string infoType, string infoCode,
        string infoContent, CmdType cmdType, TimeSpan eTime, MessageTag tag, string guid)
    {
        Name = no;
        ETime = eTime;
        Tag = tag;
        InfoType = infoType;
        InfoCode = infoCode;
        InfoContent = infoContent;
        CmdType = cmdType;
        Id = guid;
    }
}