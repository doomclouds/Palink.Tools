﻿using Palink.Tools.IO;
using Palink.Tools.Logging;
using Palink.Tools.Robots.Epson;
using Palink.Tools.Robots.YzAim;

namespace Palink.Tools.Freebus;

public class FreebusFactory
{
    /// <summary>
    /// CreateEpsonMaster
    /// </summary>
    /// <param name="streamResource"></param>
    /// <param name="logger"></param>
    /// <param name="retries">重试次数</param>
    /// <param name="waitToRetryMilliseconds">重试等待时间</param>
    /// <returns></returns>
    public static EpsonMaster CreateEpsonMaster(IStreamResource streamResource,
        IFreebusLogger logger, ushort? retries = 3,
        ushort? waitToRetryMilliseconds = 50)
    {
        var transport = new EpsonTransport(streamResource, logger);
        if (retries.HasValue)
            transport.Retries = retries.Value;
        if (waitToRetryMilliseconds.HasValue)
            transport.WaitToRetryMilliseconds = waitToRetryMilliseconds.Value;
        return new EpsonMaster(transport);
    }

    /// <summary>
    /// CreateYzAimMaster
    /// </summary>
    /// <param name="streamResource"></param>
    /// <param name="logger"></param>
    /// <param name="retries">重试次数</param>
    /// <param name="waitToRetryMilliseconds">重试等待时间</param>
    /// <returns></returns>
    public static YzAimMaster CreateYzAimMaster(IStreamResource streamResource,
        IFreebusLogger logger, ushort? retries = 3,
        ushort? waitToRetryMilliseconds = 50)
    {
        var transport = new YzAimTransport(streamResource, logger);
        if (retries.HasValue)
            transport.Retries = retries.Value;
        if (waitToRetryMilliseconds.HasValue)
            transport.WaitToRetryMilliseconds = waitToRetryMilliseconds.Value;
        return new YzAimMaster(transport);
    }
}