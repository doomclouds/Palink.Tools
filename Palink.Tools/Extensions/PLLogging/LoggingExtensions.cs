namespace Palink.Tools.Extensions.PLLogging
{
    /// <summary>
    /// 日志扩展
    /// </summary>
    public static class LoggingExtensions
    {
        #region 级别日志

        /// <summary>
        /// Trace
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="message"></param>
        public static void Trace(this IPlLogger logger, string message)
        {
            logger.Log(LoggingLevel.Trace, message);
        }

        /// <summary>
        /// Debug
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="message"></param>
        public static void Debug(this IPlLogger logger, string message)
        {
            logger.Log(LoggingLevel.Debug, message);
        }

        /// <summary>
        /// Information
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="message"></param>
        public static void Information(this IPlLogger logger, string message)
        {
            logger.Log(LoggingLevel.Information, message);
        }

        /// <summary>
        /// Warning
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="message"></param>
        public static void Warning(this IPlLogger logger, string message)
        {
            logger.Log(LoggingLevel.Warning, message);
        }

        /// <summary>
        /// Error
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="message"></param>
        public static void Error(this IPlLogger logger, string message)
        {
            logger.Log(LoggingLevel.Error, message);
        }

        /// <summary>
        /// Critical
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="message"></param>
        public static void Critical(this IPlLogger logger, string message)
        {
            logger.Log(LoggingLevel.Critical, message);
        }

        #endregion
    }
}