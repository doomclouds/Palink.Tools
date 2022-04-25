﻿namespace Palink.Tools.Logging;

public class NullFreebusLogger : IFreebusLogger
{
    /// <summary>
    /// Singleton.
    /// </summary>
    public static NullFreebusLogger Instance = new();

    private NullFreebusLogger()
    {
    }

    /// <summary>
    /// This won't do anything.
    /// </summary>
    /// <param name="level"></param>
    /// <param name="message"></param>
    public void Log(LoggingLevel level, string message)
    {
    }

    /// <summary>
    /// Always return false
    /// </summary>
    /// <param name="level"></param>
    /// <returns></returns>
    public bool ShouldLog(LoggingLevel level)
    {
        return false;
    }
}