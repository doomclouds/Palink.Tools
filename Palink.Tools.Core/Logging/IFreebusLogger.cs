namespace Palink.Tools.Logging;

/// <summary>
/// LoggingLevel
/// </summary>
public enum LoggingLevel
{
    /// <summary>
    /// Trace = 0
    /// </summary>
    Trace = 0,

    /// <summary>
    /// Debug = 1
    /// </summary>
    Debug = 1,

    /// <summary>
    /// Information = 2
    /// </summary>
    Information = 2,

    /// <summary>
    /// Warning = 3
    /// </summary>
    Warning = 3,

    /// <summary>
    /// Error = 4
    /// </summary>
    Error = 4,

    /// <summary>
    /// Critical = 5
    /// </summary>
    Critical = 5
}

/// <summary>
/// IPlLogger
/// </summary>
public interface IFreebusLogger
{
    /// <summary>
    /// 输出日志
    /// </summary>
    /// <param name="level"></param>
    /// <param name="message"></param>
    void Log(LoggingLevel level, string message);

    /// <summary>
    /// 日志最低级别
    /// </summary>
    /// <param name="level"></param>
    /// <returns></returns>
    bool ShouldLog(LoggingLevel level);
}