using System;

namespace Palink.Tools.Monitor.ECM;

/// <summary>
/// ECMMessage
/// </summary>
public abstract class ECMMessage : Message
{
    /// <summary>
    /// 展品编号
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// 命令类型
    /// </summary>
    public CmdType CmdType { get; set; }

    protected ECMMessage(string no, string url, string infoType, string infoCode,
        string infoContent, CmdType cmdType, TimeSpan eTime, MessageTag tag, string guid)
    {
        Url = url;
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