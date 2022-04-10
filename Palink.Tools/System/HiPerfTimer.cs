using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace Palink.Tools.System;

/// <summary>
/// 纳秒级计时器，仅支持Windows系统
/// </summary>
[Obsolete("好像和Stopwatch一样，就先废弃了")]
public class HiPerfTimer
{
    [DllImport("Kernel32.dll")]
    private static extern bool QueryPerformanceCounter(out long lpPerformanceCount);

    [DllImport("Kernel32.dll")]
    private static extern bool QueryPerformanceFrequency(out long clock);

    private long _startTime;
    private long _stopTime;
    private readonly long _exFreq;

    /// <summary>
    /// 纳秒计数器
    /// </summary>
    public HiPerfTimer()
    {
        _startTime = 0;
        _stopTime = 0;
        if (QueryPerformanceFrequency(out _exFreq) == false)
        {
            throw new Win32Exception("不支持高性能计数器");
        }
    }

    /// <summary>
    /// 开始计时器
    /// </summary>
    public void Start()
    {
        // 让等待线程工作 
        Thread.Sleep(0);
        QueryPerformanceCounter(out _startTime);
    }

    /// <summary>
    /// 开始计时器
    /// </summary>
    public void Restart()
    {
        _startTime = 0;
        Start();
    }

    /// <summary>
    /// 停止计时器
    /// </summary>
    /// <returns>计时器记录的时间</returns>
    public double Stop()
    {
        QueryPerformanceCounter(out _stopTime);
        return Duration;
    }

    /// <summary>
    /// 启动一个新的计时器
    /// </summary>
    /// <returns></returns>
    public static HiPerfTimer StartNew()
    {
        var timer = new HiPerfTimer();
        timer.Start();
        return timer;
    }

    private long GetCurrentTime()
    {
        QueryPerformanceCounter(out _stopTime);
        return _stopTime;
    }

    /// <summary>
    /// 时器经过时间(单位：秒)
    /// </summary>
    public double Duration => (GetCurrentTime() - _startTime) / (double)_exFreq;

    /// <summary>
    /// 时器经过的总时间(单位：纳秒)
    /// </summary>
    private double DurationNanoseconds => GetCurrentTime() - _startTime;

    /// <summary>
    /// 时器经过的总时间(单位：秒)
    /// </summary>
    public double Elapsed => Duration;

    /// <summary>
    /// 时器经过的总时间(单位：纳秒)
    /// </summary>
    public double ElapsedNanoseconds => DurationNanoseconds;

    /// <summary>
    /// 执行一个方法并测试执行时间
    /// </summary>
    /// <param name="action"></param>
    /// <returns></returns>
    public static async Task<double> Execute(Func<Task> action)
    {
        var timer = new HiPerfTimer();
        timer.Start();
        await action();
        timer.Stop();
        return timer.Duration;
    }
}