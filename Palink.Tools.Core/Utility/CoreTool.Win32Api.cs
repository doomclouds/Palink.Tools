using System;
using System.Runtime.InteropServices;
using Palink.Tools.Extensions.ObjectExt;

namespace Palink.Tools.Utility;

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

    #region ShowWindow 方法窗体状态的参数枚举

    private enum WindowStatus
    {
        /// <summary>
        /// 隐藏窗口并激活其他窗口
        /// </summary>
        SwHide = 0,

        // /// <summary>
        // /// 激活并显示一个窗口。如果窗口被最小化或最大化，
        // /// 系统将其恢复到原来的尺寸和大小。应用程序在第一次显示窗口的时候应该指定此标志
        // /// </summary>
        // SwShowNormal = 1,
        //
        // /// <summary>
        // /// 激活窗口并将其最小化
        // /// </summary>
        // SwShowMinimized = 2,
        //
        // /// <summary>
        // /// 激活窗口并将其最大化
        // /// </summary>
        // SwShowMaximized = 3,
        //
        // /// <summary>
        // /// 以窗口最近一次的大小和状态显示窗口。SwShowNormal，只是窗口没有被激活
        // /// </summary>
        // SwShowNoActivate = 4,

        /// <summary>
        /// 在窗口原来的位置以原来的尺寸激活和显示窗口
        /// </summary>
        SwShow = 5,

        // /// <summary>
        // /// 最小化指定的窗口并且激活在Z序中的下一个顶层窗口
        // /// </summary>
        // SwMinimize = 6,
        //
        // /// <summary>
        // /// 最小化的方式显示窗口，此值与SwShowMinimized相似，只是窗口没有被激活
        // /// </summary>
        // SwShowMinNoActive = 7,
        //
        // /// <summary>
        // /// 以窗口原来的状态显示窗口。此值与SW_SHOW相似，只是窗口没有被激活
        // /// </summary>
        // SwShowNa = 8,
        //
        // /// <summary>
        // /// 激活并显示窗口。如果窗口最小化或最大化，则系统将窗口恢复到原来的尺寸和位置。在恢复最小化窗口时，应用程序应该指定这个标志
        // /// </summary>
        // SwRestore = 9,
        //
        // /// <summary>
        // /// 依据在STARTUPINFO结构中指定的SW_FLAG标志设定显示状态，
        // /// STARTUPINFO 结构是由启动应用程序的程序传递给CreateProcess函数的
        // /// </summary>
        // SwShowDefault = 10,
        //
        // /// <summary>
        // /// 最小化窗口，即使拥有窗口的线程被挂起也会最小化。在从其他线程最小化窗口时才使用这个参数
        // /// </summary>
        // SwForceMinimize = 11
    }

    #endregion ShowWindow 方法窗体状态的参数枚举

    //窗体置顶
    private static readonly IntPtr HwndTopmost = new(-1);

    //取消窗体置顶
    private static readonly IntPtr HwndNoTopmost = new(-2);

    //不调整窗体位置
    private const uint SwpNoMove = 0x0002;

    //不调整窗体大小
    private const uint SwpNoSize = 0x0001;

    [DllImport("User32.dll", EntryPoint = "FindWindow")]
    private static extern IntPtr FindWindow(string? lpClassName, string lpWindowName);

    [DllImport("user32.dll")]
    private static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int x,
        int y, int cx, int cy, uint uFlags);

    [DllImport("user32.dll", EntryPoint = "ShowWindow")]
    private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

    /// <summary>
    /// 使窗体置顶
    /// </summary>
    /// <param name="name">窗体的名字</param>
    public static bool TopmostWin(string name)
    {
        var customBar = FindWindow(null, name);
        return customBar.NotNull() && SetWindowPos(customBar, HwndTopmost, 0, 0, 0, 0,
            SwpNoMove | SwpNoSize);
    }

    /// <summary>
    /// 取消窗体置顶
    /// </summary>
    /// <param name="name">窗体的名字</param>
    public static bool UnTopmostWin(string name)
    {
        var customBar = FindWindow(null, name);
        return customBar.NotNull() && SetWindowPos(customBar, HwndNoTopmost, 0, 0, 0, 0,
            SwpNoMove | SwpNoSize);
    }

    /// <summary>
    /// 显示窗口
    /// </summary>
    /// <param name="name">窗体的名字</param>
    public static bool ShowWin(string name)
    {
        var customBar = FindWindow(null, name);
        return customBar.NotNull() && ShowWindow(customBar, (int)WindowStatus.SwShow);
    }

    /// <summary>
    /// 隐藏窗口
    /// </summary>
    /// <param name="name">窗体的名字</param>
    public static bool HideWin(string name)
    {
        var customBar = FindWindow(null, name);
        return customBar.NotNull() && ShowWindow(customBar, (int)WindowStatus.SwHide);
    }

    public static bool SetWinPos(string name, int x, int y, bool topmost = false)
    {
        var customBar = FindWindow(null, name);
        var t = topmost ? HwndTopmost : HwndNoTopmost;
        return customBar.NotNull() && SetWindowPos(customBar, t, x, y, 0, 0, SwpNoSize);
    }
}