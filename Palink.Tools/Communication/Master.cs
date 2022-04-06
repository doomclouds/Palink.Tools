using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Palink.Tools.Communication.Device;
using Palink.Tools.Communication.Interface;
using Palink.Tools.Extensions.PLLogging;

namespace Palink.Tools.Communication
{
    /// <summary>
    /// Master
    /// </summary>
    public abstract class Master : BaseMaster
    {
        /// <summary>
        /// 线程锁
        /// </summary>
        public static readonly object Locker = new();

        /// <summary>
        /// 需要忽略的数据帧
        /// </summary>
        public List<string> IgnoreStringList { get; set; } = new();

        /// <summary>
        /// Master
        /// </summary>
        /// <param name="streamResource"></param>
        /// <param name="logger"></param>
        protected Master(IStreamResource streamResource, IPlLogger logger) : base(streamResource, logger)
        {
        }

        /// <summary>
        /// 虚方法，初始化读写超时
        /// </summary>
        public override void InitTimeout()
        {
            StreamResource.ReadTimeout = 50;
            StreamResource.WriteTimeout = 50;
        }

        /// <summary>
        /// 单播命令
        /// </summary>
        /// <param name="message"></param>
        /// <param name="useHexLog"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public override IMessage Unicast(IMessage message, bool useHexLog = true)
        {
            lock (Locker)
            {
                var times = ReadWriteTimes;
                var buffer = new byte[message.ReadBytes];
                while (times > 0)
                {
                    StreamResource.DiscardInBuffer();
                    SendData(message, useHexLog);

                    try
                    {
                        string result;
                        do
                        {
                            var numBytesRead = 0;
                            var watch = new Stopwatch();
                            watch.Start();
                            while (numBytesRead != message.ReadBytes && watch.ElapsedMilliseconds < StreamResource.ReadTimeout)
                            {
                                numBytesRead += StreamResource.Read(buffer, numBytesRead, message.ReadBytes - numBytesRead);
                            }

                            if (watch.ElapsedMilliseconds >= StreamResource.ReadTimeout)
                            {
                                throw new Exception("未读取到任何数据");
                            }
                            watch.Stop();
                            result = BitConverter.ToString(buffer).Replace("-", " ");
                        }
                        while (IgnoreStringList.Contains(result));

                        message.Buffer = buffer;

                        if (!useHexLog)
                            PlLogger.Debug("RX:" + Encoding.UTF8.GetString(buffer));
                        else
                        {
                            PlLogger.Debug("RX:" + BitConverter.ToString(buffer).Replace("-", " "));
                        }
                    }
                    catch (Exception exception)
                    {
                        PlLogger.Error($"{exception.Message};{exception.StackTrace}");
                        times--;
                        continue;
                    }

                    if (!CheckData(message))
                    {
                        PlLogger.Error("数据校验错误," + BitConverter.ToString(buffer).Replace("-", " "));
                        times--;
                        continue;
                    }
                    break;
                }

                if (times <= 0) throw new Exception("通讯超时");
                return message;
            }
        }
    }
}
