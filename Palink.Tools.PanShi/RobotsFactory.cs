using Palink.Tools.IO;
using Palink.Tools.Logging;
using Palink.Tools.Robots.DiskWall;

namespace Palink.Tools;

public class PSRobotsFactory
{
    /// <summary>
    /// 磁片墙控制
    /// </summary>
    /// <param name="streamResource"></param>
    /// <param name="logger"></param>
    /// <param name="retries">重试次数</param>
    /// <param name="waitToRetryMilliseconds">重试等待时间</param>
    /// <returns></returns>
    public static DiskWallMaster CreateEpsonMaster(IStreamResource streamResource,
        IFreebusLogger logger, ushort? retries = 3,
        ushort? waitToRetryMilliseconds = 50)
    {
        var transport = new DiskWallTransport(streamResource, logger);
        if (retries.HasValue)
            transport.Retries = retries.Value;
        if (waitToRetryMilliseconds.HasValue)
            transport.WaitToRetryMilliseconds = waitToRetryMilliseconds.Value;
        return new DiskWallMaster(transport);
    }
}