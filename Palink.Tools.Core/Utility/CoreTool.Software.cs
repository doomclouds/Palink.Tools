using System;
using System.Diagnostics;
using System.Reflection;
using System.Threading;

namespace Palink.Tools.Utility;

/// <summary>
/// CoreTool
/// </summary>
public partial class CoreTool
{
    /// <summary>
    /// 软件多开锁
    /// </summary>
    /// <param name="appMutex"></param>
    /// <param name="autoClose">是否自动关闭重复开启的软件</param>
    /// <returns></returns>
    public static bool SoftwareMutex(out Mutex appMutex, bool autoClose = false)
    {
        var name = Assembly.GetEntryAssembly()?.GetName().Name;
        appMutex = new Mutex(true, name, out var createdNew);
        if (createdNew) return true;
        var current = Process.GetCurrentProcess();

        foreach (var process in Process.GetProcessesByName(current.ProcessName))
        {
            if (process.Id == current.Id) continue;
            SetForegroundWindow(process.MainWindowHandle);
            break;
        }

        if (autoClose)
            Environment.Exit(0);
        return false;
    }
}