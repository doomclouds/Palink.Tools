namespace Palink.Tools.Extensions.PLLogging
{
    /// <summary>
    /// BaseLogger
    /// </summary>
    public abstract class BaseLogger : IPlLogger
    {
        /// <summary>
        /// BaseLogger
        /// </summary>
        /// <param name="minimumLoggingLevel"></param>
        protected BaseLogger(LoggingLevel minimumLoggingLevel)
        {
            MinimumLoggingLevel = minimumLoggingLevel;
        }

        /// <summary>
        /// 最小日志级别
        /// </summary>
        protected LoggingLevel MinimumLoggingLevel { get; }

        /// <summary>
        /// 是否需要打印该级别日志
        /// </summary>
        /// <param name="level"></param>
        /// <returns></returns>
        public bool ShouldLog(LoggingLevel level)
        {
            return level >= MinimumLoggingLevel;
        }

        /// <summary>
        /// 输出日志
        /// </summary>
        /// <param name="level"></param>
        /// <param name="message"></param>
        public void Log(LoggingLevel level, string message)
        {
            if (ShouldLog(level))
            {
                LogCore(level, message);
            }
        }

        /// <summary>
        /// 打印日志核心方法
        /// </summary>
        /// <param name="level"></param>
        /// <param name="message"></param>
        protected abstract void LogCore(LoggingLevel level, string message);
    }
}