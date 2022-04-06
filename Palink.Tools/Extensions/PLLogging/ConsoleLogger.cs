using System;

namespace Palink.Tools.Extensions.PLLogging
{
    /// <summary>
    /// ConsoleLogger
    /// </summary>
    public class ConsoleLogger : BaseLogger
    {
        private const int LevelColumnSize = 15;

        private static readonly string BlankHeader =
            Environment.NewLine + new string(' ', LevelColumnSize);

        /// <summary>
        /// ConsoleLogger
        /// </summary>
        /// <param name="minimumLoggingLevel"></param>
        public ConsoleLogger(LoggingLevel minimumLoggingLevel) : base(minimumLoggingLevel)
        {
        }

        /// <summary>
        /// 打印日志核心方法
        /// </summary>
        /// <param name="level"></param>
        /// <param name="message"></param>
        protected override void LogCore(LoggingLevel level, string message)
        {
            message = message?.Replace(Environment.NewLine, BlankHeader);

            Console.WriteLine($"[{level}]".PadRight(LevelColumnSize) + message);
        }
    }
}