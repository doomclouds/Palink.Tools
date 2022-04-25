using System;

namespace Palink.Tools.Logging;

/// <summary>
/// ConsoleLogger
/// </summary>
public class ConsoleFreebusLogger : FreebusLogger
{
    private const int LevelColumnSize = 15;

    private static readonly string BlankHeader =
        Environment.NewLine + new string(' ', LevelColumnSize);

    public ConsoleFreebusLogger(LoggingLevel minimumLoggingLevel = LoggingLevel.Debug)
        : base(minimumLoggingLevel)
    {
    }

    protected override void LogCore(LoggingLevel level, string message)
    {
        message = message.Replace(Environment.NewLine, BlankHeader);

        Console.WriteLine($"[{level}]".PadRight(LevelColumnSize) + message);
    }
}