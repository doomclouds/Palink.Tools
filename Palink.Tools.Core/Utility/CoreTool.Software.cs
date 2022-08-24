﻿using System;
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
    /// <returns></returns>
    public static bool SoftwareMutex(out Mutex appMutex)
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

        Environment.Exit(0);
        return false;
    }
}