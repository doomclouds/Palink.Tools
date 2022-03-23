using System;
using System.Diagnostics;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Win32;

namespace Palink.Tools.Utils;

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

    /// <summary>
    /// 设置开机自启，需要在管理员模式下运行才可有效
    /// </summary>
    /// <param name="taskName">计划任务名称</param>
    /// <param name="exeFullName">exe文件全路径</param>
    /// <param name="desc">描述</param>
    /// <param name="delay">延时启动时间</param>
    /// <param name="used">是否开机自启</param>
    public static bool AutoStart(string taskName, string exeFullName, string desc,
        TimeSpan delay, bool used = true)
    {
        if (used)
        {
            return AddTask(taskName, exeFullName, desc, delay);
        }

        DeleteTask(taskName);

        return true;
    }

    /// <summary>
    /// 窗口置顶
    /// </summary>
    /// <param name="hwnd"></param>
    /// <param name="delay"></param>
    public static async void TopMost(IntPtr? hwnd, int delay)
    {
        while (hwnd.HasValue)
        {
            await Task.Delay(delay);
            SetForegroundWindow(hwnd.Value);
        }
    }

    /// <summary>
    /// 边缘侧滑
    /// </summary>
    /// <param name="status"></param>
    /// <returns></returns>
    public static bool AllowEdgeSwipe(bool status)
    {
        const string targetPath = "SOFTWARE\\Policies\\Microsoft\\Windows\\EdgeUI";
        CreateFolderInLocalMachine(targetPath);
        return SetupKeyValueInLocalMachine(targetPath,
            "AllowEdgeSwipe", status ? 1 : 0, RegistryValueKind.DWord);
    }
}