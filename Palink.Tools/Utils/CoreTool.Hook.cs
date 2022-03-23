using System;
using System.Runtime.InteropServices;

namespace Palink.Tools.Utils;

/// <summary>
/// CoreTool
/// </summary>
public partial class CoreTool
{
    internal struct KeyboardLlHookData
    {
        public uint VkCode;
        public uint ScanCode;
        public uint Flags;
        public uint Time;
        public IntPtr ExtraInfo;
    }

    internal struct MouseLlHookData
    {
        internal long Yx;
        // internal readonly int MouseData;
        // internal readonly uint Flags;
        // internal readonly uint Time;
        // internal readonly IntPtr DwExtraInfo;
    }

    /// <summary>
    /// 安装钩子
    /// </summary>
    /// <param name="idHook"></param>
    /// <param name="lpfn"></param>
    /// <param name="pInstance"></param>
    /// <param name="threadId"></param>
    /// <returns></returns>
    [DllImport("user32.dll")]
    public static extern IntPtr SetWindowsHookEx(int idHook, HookProc lpfn,
        IntPtr pInstance, int threadId);

    /// <summary>
    /// 卸载钩子
    /// </summary>
    /// <param name="pHookHandle"></param>
    /// <returns></returns>
    [DllImport("user32.dll", CallingConvention = CallingConvention.StdCall)]
    public static extern bool UnhookWindowsHookEx(IntPtr pHookHandle);

    /// <summary>
    /// CallNextHookEx
    /// </summary>
    /// <param name="hhk"></param>
    /// <param name="nCode"></param>
    /// <param name="wParam"></param>
    /// <param name="lParam"></param>
    /// <returns></returns>
    [DllImport("user32.dll")]
    public static extern int CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam,
        IntPtr lParam);

    private static IntPtr _pKeyboardHook = IntPtr.Zero;

    private static IntPtr _pMouseHook = IntPtr.Zero;

    /// <summary>
    /// 钩子委托声明
    /// </summary>
    public delegate int HookProc(int code, IntPtr wParam, IntPtr lParam);

    private static HookProc _keyboardHookProc;
    private static HookProc _mouseHookProc;

    private static int KeyboardHookCallback(int code, IntPtr wParam, IntPtr lParam)
    {
        if (code < 0)
        {
            return CallNextHookEx(IntPtr.Zero, code, wParam, lParam);
        }

        var khd =
            (KeyboardLlHookData)Marshal.PtrToStructure(lParam,
                typeof(KeyboardLlHookData))!;
        // System.Diagnostics.Debug.WriteLine(
        //     $"key event:{wParam}, key code:{khd.VkCode}, event time:{khd.Time}");

        return 0;
    }

    /// <summary>
    /// MouseExecute
    /// </summary>
    public static Action<IntPtr> MouseExecute { get; set; }

    private static int MouseHookCallback(int code, IntPtr wParam, IntPtr lParam)
    {
        if (code < 0)
        {
            return CallNextHookEx(IntPtr.Zero, code, wParam, lParam);
        }

        var mhd =
            (MouseLlHookData)Marshal.PtrToStructure(lParam, typeof(MouseLlHookData))!;

        MouseExecute?.Invoke(wParam);

        // System.Diagnostics.Debug.WriteLine(
        //     $"mouse event:{wParam}, ({mhd.Yx & 0xffffffff},{mhd.Yx >> 32})");

        return 0;
    }

    /// <summary>
    /// InsertKeyboardHook
    /// </summary>
    /// <returns></returns>
    public static bool InsertKeyboardHook()
    {
        if (_pKeyboardHook != IntPtr.Zero)
        {
            return true;
        }

        //创建钩子
        _keyboardHookProc = KeyboardHookCallback;
        _pKeyboardHook = SetWindowsHookEx(13, //13表示全局键盘事件。
            _keyboardHookProc,
            (IntPtr)0,
            0);

        if (_pKeyboardHook != IntPtr.Zero)
        {
            return true;
        }

        RemoveKeyboardHook();
        return false;
    }

    /// <summary>
    /// InsertMouseHook
    /// </summary>
    /// <returns></returns>
    public static bool InsertMouseHook()
    {
        if (_pMouseHook != IntPtr.Zero)
        {
            return true;
        }

        _mouseHookProc = MouseHookCallback;
        _pMouseHook = SetWindowsHookEx(14, //14表示全局鼠标事件
            _mouseHookProc,
            (IntPtr)0,
            0);

        if (_pMouseHook != IntPtr.Zero)
        {
            return true;
        }

        RemoveMouseHook();
        return false;
    }

    /// <summary>
    /// RemoveKeyboardHook
    /// </summary>
    /// <returns></returns>
    public static bool RemoveKeyboardHook()
    {
        if (_pKeyboardHook == IntPtr.Zero)
        {
            return true;
        }

        if (UnhookWindowsHookEx(_pKeyboardHook))
        {
            _pKeyboardHook = IntPtr.Zero;
        }
        else
        {
            return false;
        }

        return true;
    }

    /// <summary>
    /// RemoveMouseHook
    /// </summary>
    /// <returns></returns>
    public static bool RemoveMouseHook()
    {
        if (_pMouseHook == IntPtr.Zero)
        {
            return true;
        }

        if (UnhookWindowsHookEx(_pMouseHook))
        {
            _pMouseHook = IntPtr.Zero;
        }
        else
        {
            return false;
        }

        return true;
    }
}