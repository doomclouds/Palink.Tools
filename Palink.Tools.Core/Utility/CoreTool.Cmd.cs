using System;
using System.Diagnostics;

namespace Palink.Tools.Utility;

// shutdown命令的参数：
//
// shutdown.exe -s：关机
// shutdown.exe -r：关机并重启
// shutdown.exe -l：注销当前用户
//
// shutdown.exe -s -t 时间：设置关机倒计时
// shutdown.exe -h：休眠
// shutdown.exe -t 时间：设置关机倒计时。默认值是 30 秒。
// shutdown.exe -a：取消关机
// shutdown.exe -f：强行关闭应用程序而没有警告
// shutdown.exe -m \计算机名：控制远程计算机
// shutdown.exe -i：显示“远程关机”图形用户界面，但必须是Shutdown的第一个参数
// shutdown.exe -c "消息内容"：输入关机对话框中的消息内容
// shutdown.exe -d [u][p]:xx:yy ：列出系统关闭的原因代码：
//                                  u 是用户代码 ，
//                                  p 是一个计划的关闭代码 ，
//                                  xx 是一个主要原因代码(小于 256 的正整数) ，
//                                  yy 是一个次要原因代码(小于 65536 的正整数)
/// <summary>
/// CoreTool
/// </summary>
public partial class CoreTool
{
    /// <summary>
    /// 延时关机
    /// </summary>
    /// <param name="delay">延时时间/秒</param>
    public static void DelayShutdown(int delay)
    {
        Process.Start("c:/windows/system32/shutdown.exe", $"-s -t {delay}");
    }

    /// <summary>
    /// 延时重启
    /// </summary>
    /// <param name="delay">延时时间/秒</param>
    public static void DelayRestart(int delay)
    {
        Process.Start("c:/windows/system32/shutdown.exe", $"-r -t {delay}");
    }

    /// <summary>
    /// 定时关机
    /// </summary>
    public static void TimedShutDown(DateTime time)
    {
        Process.Start("c:/windows/system32/shutdown.exe",
            $"-s -t {(time - DateTime.Now).TotalSeconds:0000}");
    }

    /// <summary>
    /// 定时关机
    /// </summary>
    public static void TimedRestart(DateTime time)
    {
        Process.Start("c:/windows/system32/shutdown.exe",
            $"-r -t {(time - DateTime.Now).TotalSeconds:0000}");
    }

    /// <summary>
    /// 取消关机
    /// </summary>
    public static void CancelShutDown()
    {
        Process.Start("c:/windows/system32/shutdown.exe", "-a");
    }
}