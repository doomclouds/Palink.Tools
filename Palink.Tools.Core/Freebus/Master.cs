﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Palink.Tools.Freebus.Device;
using Palink.Tools.Freebus.Interface;
using Palink.Tools.IO;
using Palink.Tools.Logging;

namespace Palink.Tools.Freebus;

/// <summary>
/// Master
/// </summary>
[Obsolete("Master与Message不再使用，请使用Freebus相关方法")]
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
    protected Master(IStreamResource streamResource, IFreebusLogger logger) : base(
        streamResource, logger)
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
    /// <param name="ignoreReadBytes">忽略读取字节的长度，读取到数据就返回，主要用于TCP</param>
    /// <param name="useHexLog">显示hex日志还是UTF8解码后的字符</param>
    /// <returns></returns>
    public override IMessage Unicast(IMessage message, bool ignoreReadBytes = false,
        bool useHexLog = true)
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
                        while (numBytesRead != message.ReadBytes &&
                               watch.ElapsedMilliseconds < StreamResource.ReadTimeout)
                        {
                            numBytesRead += StreamResource.Read(buffer, numBytesRead,
                                message.ReadBytes - numBytesRead);
                            if (ignoreReadBytes && numBytesRead > 0)
                                break;
                        }

                        if (watch.ElapsedMilliseconds >= StreamResource.ReadTimeout)
                        {
                            throw new Exception("未读取到任何数据");
                        }

                        watch.Stop();
                        result = BitConverter.ToString(buffer).Replace("-", " ");
                    } while (IgnoreStringList.Contains(result));

                    message.Buffer = buffer;

                    if (!useHexLog)
                        FreebusLogger.Debug("RX:" +
                                            Encoding.UTF8.GetString(buffer).Trim('\0'));
                    else
                    {
                        FreebusLogger.Debug("RX:" +
                                            BitConverter.ToString(buffer).Replace("-", " "));
                    }
                }
                catch (Exception exception)
                {
                    FreebusLogger.Error($"{exception.Message};{exception.StackTrace}");
                    times--;
                    continue;
                }

                if (!CheckData(message))
                {
                    FreebusLogger.Error("数据校验错误," +
                                        BitConverter.ToString(buffer).Replace("-", " "));
                    times--;
                    continue;
                }

                break;
            }

            if (times <= 0) throw new Exception("通讯超时");
            return message;
        }
    }

    /// <summary>
    /// 异步单播命令
    /// </summary>
    /// <param name="message"></param>
    /// <param name="token"></param>
    /// <param name="ignoreReadBytes">忽略读取字节的长度，读取到数据就返回，主要用于TCP</param>
    /// <param name="useHexLog">显示hex日志还是UTF8解码后的字符</param>
    /// <returns></returns>
    public override Task<IMessage> UnicastAsync(IMessage message,
        CancellationToken token = default,
        bool ignoreReadBytes = false, bool useHexLog = true)
    {
        return Task.Run(() => Unicast(message, ignoreReadBytes, useHexLog), token);
    }
}