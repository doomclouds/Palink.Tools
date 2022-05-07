using System;

namespace Palink.Tools.Extensions.PLRandom;

/// <summary>
/// RandomExtensions
/// </summary>
public static class RandomExtensions
{
    /// <summary>
    /// Strict Next
    /// </summary>
    /// <param name="r"></param>
    /// <param name="minValue"></param>
    /// <param name="maxValue"></param>
    /// <returns></returns>
    public static int StrictNext(this Random r, int minValue = int.MinValue,
        int maxValue = int.MaxValue)
    {
        return new Random(BitConverter.ToInt32(Guid.NewGuid().ToByteArray(), 0)).Next(
            minValue
            , maxValue);
    }

    /// <summary>
    /// Next Gauss
    /// </summary>
    /// <param name="rand"></param>
    /// <param name="mean">均值</param>
    /// <param name="stdDev">方差</param>
    /// <returns></returns>
    public static double NextGauss(this Random rand, double mean, double stdDev)
    {
        var u1 = 1.0 - rand.NextDouble();
        var u2 = 1.0 - rand.NextDouble();
        var randStdNormal =
            Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Sin(2.0 * Math.PI * u2);
        return mean + stdDev * randStdNormal;
    }
}