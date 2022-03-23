using System;
using System.Runtime.InteropServices;

namespace Palink.Tools.Utils;

/// <summary>
/// CoreTool
/// </summary>
public partial class CoreTool
{
    /// <summary>
    /// 设置为前景窗口
    /// </summary>
    /// <param name="hwnd"></param>
    [DllImport("user32.dll", EntryPoint = "SetForegroundWindow", SetLastError = true)]
    public static extern void SetForegroundWindow(IntPtr hwnd);
}