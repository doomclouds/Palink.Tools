using System.Runtime.InteropServices;
using System;
using System.ComponentModel;

namespace Palink.Tools.System.TimeExt;

public class HighTimer
{
    private bool _disposed;

    // private int _interval;
    private int _resolution;

    /// <summary>
    /// 当前定时器实例ID
    /// </summary>
    private uint _timerId;

    // 保持定时器回调以防止垃圾收集。
    private readonly MultimediaTimerCallback _callback;

    /// <summary>
    /// API使用的回调
    /// </summary>
    public event EventHandler? Elapsed;

    public HighTimer()
    {
        _callback = TimerCallbackMethod;
        Resolution = 0;
    }

    ~HighTimer()
    {
        Dispose(false);
    }

    /// <summary>
    /// 注意最小分辨率为 0，表示可能的最高分辨率。
    /// </summary>
    public int Resolution
    {
        get => _resolution;
        set
        {
            CheckDisposed();

            if (value < 0)
                throw new ArgumentOutOfRangeException(nameof(value));

            _resolution = value;
        }
    }

    /// <summary>
    /// 是否在运行
    /// </summary>
    public bool IsRunning => _timerId != 0;

    /// <summary>
    /// 启动一个定时器实例
    /// </summary>
    /// <param name="ms">以毫秒为单位的计时器间隔</param>
    /// <param name="repeat">如果为 true 设置重复事件，否则设置一次性</param>
    public void Start(uint ms, bool repeat)
    {
        //杀死任何现有的计时器
        CheckDisposed();

        if (IsRunning)
            throw new InvalidOperationException("Timer is already running");

        //Set the timer type flags
        var f = FuEvent.TimeCallbackFunction |
                (repeat ? FuEvent.TimePeriodic : FuEvent.TimeOneShot);
        // Event type = 0, one off event
        // Event type = 1, periodic event
        uint userCtx = 0;
        lock (this)
        {
            _timerId = NativeMethods.TimeSetEvent(ms, (uint)Resolution, _callback,
                ref userCtx, (uint)f);
            if (_timerId == 0)
            {
                var error = Marshal.GetLastWin32Error();
                throw new Win32Exception(error);
            }
        }
    }

    /// <summary>
    /// 停止
    /// </summary>
    public void Stop()
    {
        CheckDisposed();

        if (!IsRunning)
            throw new InvalidOperationException("Timer has not been started");

        StopInternal();
    }

    private void StopInternal()
    {
        lock (this)
        {
            if (_timerId != 0)
            {
                NativeMethods.TimeKillEvent(_timerId);
                _timerId = 0;
            }
        }
    }

    public void Dispose()
    {
        Dispose(true);
    }

    private void TimerCallbackMethod(uint id, uint msg, ref uint userCtx, uint rsv1,
        uint rsv2)
    {
        Elapsed?.Invoke(this, EventArgs.Empty);
    }

    private void CheckDisposed()
    {
        if (_disposed)
            throw new ObjectDisposedException("MultimediaTimer");
    }

    private void Dispose(bool disposing)
    {
        if (_disposed)
            return;

        _disposed = true;
        if (IsRunning)
        {
            StopInternal();
        }

        if (disposing)
        {
            Elapsed = null;
            GC.SuppressFinalize(this);
        }
    }
}

internal delegate void MultimediaTimerCallback(uint id, uint msg, ref uint userCtx,
    uint rsv1, uint rsv2);

/// <summary>
/// 此结构包含有关计时器分辨率的信息。单位是ms
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public struct TimeCaps
{
    /// <summary>
    /// 支持的最小期限。
    /// </summary>
    public uint wPeriodMin;

    /// <summary>
    /// 支持的最大期限。
    /// </summary>
    public uint wPeriodMax;
}

/// <summary>
/// 定时器类型定义
/// </summary>
[Flags]
public enum FuEvent : uint
{
    TimeOneShot = 0, //Event occurs once, after uDelay milliseconds.
    TimePeriodic = 1,
    TimeCallbackFunction = 0x0000, /* callback is function */
    //TIME_CALLBACK_EVENT_SET = 0x0010, /* callback is event - use SetEvent */
    //TIME_CALLBACK_EVENT_PULSE = 0x0020  /* callback is event - use PulseEvent */
}

internal static class NativeMethods
{
    /// <summary>
    /// 此函数启动指定的计时器事件。
    /// </summary>
    /// <param name="msDelay">事件延迟，以毫秒为单位。如果该值不在计时器支持的最小和最大事件延迟范围内，则该函数返回错误。</param>
    /// <param name="msResolution">计时器事件的分辨率，以毫秒为单位。分辨率越高，分辨率越高；零分辨率表示周期性事件应该以最大可能的精度发生。但是，为减少系统开销，应使用适合您的应用程序的最大值。</param>
    /// <param name="callback">如果fuEvent指定TIME_CALLBACK_EVENT_SET或TIME_CALLBACK_EVENT_PULSE标志，则fptc参数将解释为事件对象的句柄。事件将在单个事件完成时设置或发出脉冲，或者在周期性事件完成时定期设置或触发。对于fuEvent的任何其他值，fptc参数将被解释为函数指针。</param>
    /// <param name="userCtx">用户提供的回调数据。</param>
    /// <param name="fuEvent">计时器事件类型。下表显示了fuEvent参数可以包含的值。</param>
    [DllImport("winmm.dll", SetLastError = true, EntryPoint = "timeSetEvent")]
    internal static extern uint TimeSetEvent(uint msDelay, uint msResolution,
        MultimediaTimerCallback callback, ref uint userCtx, uint fuEvent);

    /// <summary>
    /// 此功能取消指定的计时器事件。
    /// </summary>
    /// <param name="uTimerId">要取消的计时器事件的标识符。此标识符由timeSetEvent函数返回，该函数启动指定的计时器事件。</param>
    /// <returns></returns>
    [DllImport("winmm.dll", SetLastError = true, EntryPoint = "timeKillEvent")]
    internal static extern void TimeKillEvent(uint uTimerId);

    /// <summary>
    /// 此函数查询计时器设备以确定其分辨率。
    /// </summary>
    /// <param name="ptc">指向TIMECAPS结构的指针。该结构充满了有关计时器设备分辨率的信息。</param>
    /// <param name="cbtc">TIMECAPS结构的大小（以字节为单位）。</param>
    /// <returns>如果成功，则返回TIMERR_NOERROR，如果未能返回计时器设备功能，则返回TIMERR_STRUCT。</returns>
    [DllImport("winmm.dll")]
    internal static extern uint timeGetDevCaps(ref TimeCaps ptc, int cbtc);


    [DllImport("Winmm.dll", CharSet = CharSet.Auto)]
    internal static extern uint timeGetTime();

    [DllImport("Winmm.dll", CharSet = CharSet.Auto)]
    internal static extern uint timeBeginPeriod(uint uPeriod);

    [DllImport("Winmm.dll", CharSet = CharSet.Auto)]
    internal static extern uint timeEndPeriod(uint uPeriod);
}