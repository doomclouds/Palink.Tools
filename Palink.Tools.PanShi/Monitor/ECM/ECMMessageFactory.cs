using System;

namespace Palink.Tools.Monitor.ECM;

public static class ECMMessageFactory
{
    public static ECMBeats CreateBeats(this ECMService service)
    {
        return new ECMBeats(service.ExhibitNo, service.Url);
    }

    public static ECMInteraction CreateInteraction(this ECMService service,
        TimeSpan eTime)
    {
        return new ECMInteraction(service.ExhibitNo, service.Url, eTime);
    }

    public static ECMMonitor CreateMonitor(this ECMService service,
        string content, string type,
        TimeSpan eTime, MessageTag tag)
    {
        return new ECMMonitor(service.ExhibitNo, service.Url, content, type, eTime, tag);
    }
}