using System;
using System.Collections.Generic;
using System.Linq;

namespace Palink.Tools.Logging
{
    /// <summary>
    /// 日志扩展
    /// </summary>
    public static class LoggingExtensions
    {
        #region Standard level-based logging

        public static void Trace(this IFreebusLogger logger, string message)
        {
            logger.Log(LoggingLevel.Trace, message);
        }

        public static void Debug(this IFreebusLogger logger, string message)
        {
            logger.Log(LoggingLevel.Debug, message);
        }

        public static void Information(this IFreebusLogger logger, string message)
        {
            logger.Log(LoggingLevel.Information, message);
        }

        public static void Warning(this IFreebusLogger logger, string message)
        {
            logger.Log(LoggingLevel.Warning, message);
        }

        public static void Error(this IFreebusLogger logger, string message)
        {
            logger.Log(LoggingLevel.Error, message);
        }

        public static void Critical(this IFreebusLogger logger, string message)
        {
            logger.Log(LoggingLevel.Critical, message);
        }

        #endregion

        #region Func Logging

        public static void Log(this IFreebusLogger logger, LoggingLevel level,
            Func<string> messageFactory)
        {
            if (!logger.ShouldLog(level))
            {
                return;
            }

            var message = messageFactory();

            logger.Log(level, message);
        }

        public static void Trace(this IFreebusLogger logger, Func<string> messageFactory)
        {
            logger.Log(LoggingLevel.Trace, messageFactory);
        }

        public static void Debug(this IFreebusLogger logger, Func<string> messageFactory)
        {
            logger.Log(LoggingLevel.Debug, messageFactory);
        }

        public static void Information(this IFreebusLogger logger,
            Func<string> messageFactory)
        {
            logger.Log(LoggingLevel.Information, messageFactory);
        }

        public static void Warning(this IFreebusLogger logger,
            Func<string> messageFactory)
        {
            logger.Log(LoggingLevel.Warning, messageFactory);
        }

        public static void Error(this IFreebusLogger logger, Func<string> messageFactory)
        {
            logger.Log(LoggingLevel.Error, messageFactory);
        }

        public static void Critical(this IFreebusLogger logger,
            Func<string> messageFactory)
        {
            logger.Log(LoggingLevel.Critical, messageFactory);
        }

        #endregion

        #region Frame logging

        private static void LogFrame(this IFreebusLogger logger, string prefix,
            IEnumerable<byte> frame)
        {
            if (!logger.ShouldLog(LoggingLevel.Trace))
            {
                return;
            }

            logger.Trace(
                $"{prefix}: {string.Join(" ", frame.Select(b => b.ToString("X2")))}");
        }

        private static void LogFrame(this IFreebusLogger logger, string prefix,
            string msg)
        {
            if (!logger.ShouldLog(LoggingLevel.Trace))
            {
                return;
            }

            logger.Trace($"{prefix}: {msg}");
        }

        public static void LogFrameTx(this IFreebusLogger logger, byte[] frame)
        {
            logger.LogFrame("TX", frame);
        }

        public static void LogFrameRx(this IFreebusLogger logger, byte[] frame)
        {
            logger.LogFrame("RX", frame);
        }

        public static void LogFrameIgnoreRx(this IFreebusLogger logger, byte[] frame)
        {
            logger.LogFrame("IR", frame);
        }

        public static void LogFrameTx(this IFreebusLogger logger, string msg)
        {
            logger.LogFrame("TX", msg);
        }

        public static void LogFrameRx(this IFreebusLogger logger, string msg)
        {
            logger.LogFrame("RX", msg);
        }

        public static void LogFrameIgnoreRx(this IFreebusLogger logger, string msg)
        {
            logger.LogFrame("IR", msg);
        }

        #endregion
    }
}