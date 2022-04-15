﻿using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Palink.Tools.Communication.Interface;
using Palink.Tools.Extensions.PLLogging;

namespace Palink.Tools.Communication.Device;

/// <summary>
/// 基础Master
/// </summary>
public abstract class BaseMaster
{
    /// <summary>
    /// 读写次数
    /// </summary>
    public int ReadWriteTimes { get; set; } = 3;

    /// <summary>
    /// BaseMaster
    /// </summary>
    /// <param name="streamResource"></param>
    /// <param name="logger"></param>
    protected BaseMaster(IStreamResource streamResource, IPlLogger logger)
    {
        StreamResource = streamResource;
        PlLogger = logger;
        // ReSharper disable once VirtualMemberCallInConstructor
        InitTimeout();
    }

    /// <summary>
    /// StreamResource
    /// </summary>
    public IStreamResource StreamResource { get; set; }

    /// <summary>
    /// 日志
    /// </summary>
    public IPlLogger PlLogger { get; set; }

    /// <summary>
    /// 发送数据
    /// </summary>
    /// <param name="message"></param>
    /// <param name="useHexLog">显示hex日志还是UTF8解码后的字符</param>
    public void SendData(IMessage message, bool useHexLog = true)
    {
        StreamResource.Write(message.Data, 0, message.Data.Length);
        var recStr = message.Data.Aggregate("",
            (current, data) => current + $"{data:x2}" + " ");

        if (!useHexLog)
            recStr = Encoding.UTF8.GetString(message.Data);
        PlLogger.Debug("TX:" + recStr);
    }

    /// <summary>
    /// 发送数据
    /// </summary>
    /// <param name="message"></param>
    /// <param name="useHexLog">显示hex日志还是UTF8解码后的字符</param>
    /// <param name="token"></param>
    public async Task SendDataAsync(IMessage message, bool useHexLog = true,
        CancellationToken token = default)
    {
        StreamResource.Write(message.Data, 0, message.Data.Length);
        var recStr = message.Data.Aggregate("",
            (current, data) => current + $"{data:x2}" + " ");

        if (!useHexLog)
            recStr = Encoding.UTF8.GetString(message.Data);
        PlLogger.Debug("TX:" + recStr);
        await Task.CompletedTask;
    }

    /// <summary>
    /// 虚方法，初始化读写超时
    /// </summary>
    public virtual void InitTimeout()
    {
    }

    /// <summary>
    /// 单播命令
    /// </summary>
    /// <param name="message"></param>
    /// <param name="ignoreReadBytes">忽略读取字节的长度，读取到数据就返回，主要用于TCP</param>
    /// <param name="useHexLog">显示hex日志还是UTF8解码后的字符</param>
    /// <returns></returns>
    public abstract IMessage Unicast(IMessage message, bool ignoreReadBytes = false,
        bool useHexLog = true);

    /// <summary>
    /// 异步单播命令
    /// </summary>
    /// <param name="message"></param>
    /// <param name="token"></param>
    /// <param name="ignoreReadBytes">忽略读取字节的长度，读取到数据就返回，主要用于TCP</param>
    /// <param name="useHexLog">显示hex日志还是UTF8解码后的字符</param>
    /// <returns></returns>
    public abstract Task<IMessage> UnicastAsync(IMessage message,
        CancellationToken token = default, bool ignoreReadBytes = false,
        bool useHexLog = true);

    /// <summary>
    /// 数据校验
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    public abstract bool CheckData(IMessage message);

    /// <summary>
    /// 创建发送数据体
    /// </summary>
    /// <param name="noCheckFrame"></param>
    /// <returns></returns>
    public abstract IMessage? CreateFrame((byte id, byte cmd, byte[] frame) noCheckFrame);

    /// <summary>
    /// 创建字符串发送数据体
    /// </summary>
    /// <param name="frame"></param>
    /// <returns></returns>
    public abstract IMessage? CreateStringFrame(string frame);
}