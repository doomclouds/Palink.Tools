using System;
using System.Diagnostics;

namespace Palink.Tools.Logging;

public class TraceFreebusLogger : FreebusLogger
{
    private const int LevelColumnSize = 15;

    private static readonly string BlankHeader =
        Environment.NewLine + new string(' ', LevelColumnSize);

    public TraceFreebusLogger(LoggingLevel minimumLoggingLevel = LoggingLevel.Debug)
        : base(minimumLoggingLevel)
    {
    }

    protected override void LogCore(LoggingLevel level, string message)
    {
        message = message.Replace(Environment.NewLine, BlankHeader);

        Trace.WriteLine($"[{level}]".PadRight(LevelColumnSize) + message);
    }
}