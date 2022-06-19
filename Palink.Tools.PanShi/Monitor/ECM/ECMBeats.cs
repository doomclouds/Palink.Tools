using System;

namespace Palink.Tools.Monitor.ECM;

public class ECMBeats : ECMMessage
{
    public ECMBeats(string no, string url) : base(no, url, "M",
        "001", "心跳", CmdType.Beats, TimeSpan.FromSeconds(10), MessageTag.Once,
        Guid.NewGuid().ToString("N"))
    {
    }
}

public class ECMInteraction : ECMMessage
{
    public ECMInteraction(string no, string url, TimeSpan eTime) : base(no, url, "M",
        "101", "互动次数", CmdType.Interaction, eTime, MessageTag.Needed,
        Guid.NewGuid().ToString("N"))
    {
    }
}

public class ECMMonitor : ECMMessage
{
    public ECMMonitor(string no, string url, string content, string type,
        TimeSpan eTime, MessageTag tag) : base(no, url, type,
        "000", content, CmdType.Monitor, eTime, tag,
        Guid.NewGuid().ToString("N"))
    {
    }
}