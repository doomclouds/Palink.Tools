using System;
using System.Reflection;
using System.Threading.Tasks;
using Palink.Tools.Freebus.Device;
using Palink.Tools.Freebus.Interface;
using Palink.Tools.Freebus.Message;
using Palink.Tools.Logging;

namespace Palink.Tools.Robots.Epson;

public class EpsonMaster : FreebusMaster
{
    private const string NewLine = "\r\n";
    private const int DefaultDelay = 5000;

    internal EpsonMaster(IFreebusTransport transport) : base(transport)
    {
    }

    private static string CreateCmd(string? cmd, string parameters)
    {
        return $"${cmd},{parameters}{NewLine}";
    }

    /// <summary>
    /// 机器人登录
    /// </summary>
    /// <returns></returns>
    public bool Login(string pwd)
    {
        try
        {
            var cmd = CreateCmd(MethodBase.GetCurrentMethod()?.Name, pwd);
            var context = new FreebusContext();
            context.SetPduString(cmd);
            context.NewLine = NewLine;
            var ret = ExecuteCustomMessage(context);
            return ret.Succeed;
        }
        catch (Exception e)
        {
            Transport.Logger.Error(
                $"{MethodBase.GetCurrentMethod()?.Name}命令异常,{e.Message}");
            return false;
        }
    }

    /// <summary>
    /// 机器人退出
    /// </summary>
    /// <returns></returns>
    public bool Logout()
    {
        try
        {
            var cmd = CreateCmd(MethodBase.GetCurrentMethod()?.Name, "");
            var context = new FreebusContext();
            context.SetPduString(cmd);
            context.NewLine = NewLine;
            var ret = ExecuteCustomMessage(context);
            return ret.Succeed;
        }
        catch (Exception e)
        {
            Transport.Logger.Error(
                $"{MethodBase.GetCurrentMethod()?.Name}命令异常,{e.Message}");
            return false;
        }
    }

    /// <summary>
    /// 获取机器人状态
    /// 返回#GetStatus, aaaaaaaaaaa, bbbb
    /// 10位数字“aaaaaaaaaa”是用于下列10 个标志的 Test/Teach/Auto/Warning/SError/Safeguard/EStop/Error/Paused/Running/Ready
    /// 1 为开/0 为关,如果Teach 和Auto 为开，则其为1100000000
    /// Test 在TEST模式下打开
    /// Teach 在TEACH模式下打开
    /// Auto 在远程输入接受条件下打开       
    /// Warning在警告条件下打开甚至在警告条件下也可以像往常一样执行任务。然而，应尽快采取警告行动。
    /// SError在严重错误状态下打开发生严重错误时，重新启动控制器从错误状态中恢复过来。“Reset输入”不可用。
    /// Safeguard 安全门打开时打开
    /// EStop 在紧急状态下打开
    /// Error 在错误状态下打开使用“Reset输入”从错误状态中恢复。
    /// Paused 打开暂停的任务
    /// Running执行任务时打开在“Paused输出”为开时关闭。
    /// Ready 控制器完成启动且无任务执行时打开
    /// </summary>
    /// <returns></returns>
    public (bool running, bool safeguard, bool eStop, bool error, bool ready, bool auto)
        GetStatus()
    {
        try
        {
            var cmd = CreateCmd(MethodBase.GetCurrentMethod()?.Name, "");
            var context = new FreebusContext();
            context.SetPduString(cmd);
            context.NewLine = NewLine;
            var ret = ExecuteCustomMessage(context).GetDruString();
            if (ret == "" || ret.Split(',')[1].Length != 11)
                return (false, false, false, false, false, false);
            var running = ret.Split(',')[1].Substring(9, 1) == "1";
            var safeguard = ret.Split(',')[1].Substring(5, 1) == "1";
            var eStop = ret.Split(',')[1].Substring(6, 1) == "1";
            var error = ret.Split(',')[1].Substring(7, 1) == "1";
            var ready = ret.Split(',')[1].Substring(10, 1) == "1";
            var auto = ret.Split(',')[1].Substring(2, 1) == "1";
            return (running, safeguard, eStop, error, ready, auto);
        }
        catch (Exception e)
        {
            Transport.Logger.Error(
                $"{MethodBase.GetCurrentMethod()?.Name}命令异常,{e.Message}");
            return (false, false, false, false, false, false);
        }
    }

    /// <summary>
    /// 执行Main程序
    /// </summary>
    /// <param name="id"></param>
    /// <param name="waitTime">等待时间，Start命令必须等待机器人任务完全结束才有返回</param>
    /// <returns></returns>
    public bool Start(int id, int waitTime = DefaultDelay)
    {
        try
        {
            var temp = Transport.ReadTimeout;
            Transport.ReadTimeout = waitTime;
            var cmd = CreateCmd(MethodBase.GetCurrentMethod()?.Name, id.ToString());
            var context = new FreebusContext();
            context.SetPduString(cmd);
            context.NewLine = NewLine;
            var ret = ExecuteCustomMessage(context);
            Transport.ReadTimeout = temp;
            return ret.Succeed;
        }
        catch (Exception e)
        {
            Transport.Logger.Error(
                $"{MethodBase.GetCurrentMethod()?.Name}命令异常,{e.Message}");
            return false;
        }
    }

    /// <summary>
    /// 重置机器人
    /// </summary>
    /// <returns></returns>
    public bool Reset()
    {
        try
        {
            var (_, _, _, _, ready, auto) = GetStatus();
            if (ready || auto)
                return true;
            var cmd = CreateCmd(MethodBase.GetCurrentMethod()?.Name, "");
            var context = new FreebusContext();
            context.SetPduString(cmd);
            context.NewLine = NewLine;
            var ret = ExecuteCustomMessage(context);
            return ret.Succeed;
        }
        catch (Exception e)
        {
            Transport.Logger.Error(
                $"{MethodBase.GetCurrentMethod()?.Name}命令异常,{e.Message}");
            return false;
        }
    }

    /// <summary>
    /// 停止机器人
    /// </summary>
    /// <returns></returns>
    public bool Stop()
    {
        try
        {
            var (_, _, _, _, ready, auto) = GetStatus();
            if (ready || auto)
                return true;
            var cmd = CreateCmd(MethodBase.GetCurrentMethod()?.Name, "");
            var context = new FreebusContext();
            context.SetPduString(cmd);
            context.NewLine = NewLine;
            var ret = ExecuteCustomMessage(context);
            return ret.Succeed;
        }
        catch (Exception e)
        {
            Transport.Logger.Error(
                $"{MethodBase.GetCurrentMethod()?.Name}命令异常,{e.Message}");
            return false;
        }
    }

    /// <summary>
    /// 机器人使能
    /// </summary>
    /// <returns></returns>
    public bool SetMotorsOn(int id = 0)
    {
        try
        {
            var cmd = CreateCmd(MethodBase.GetCurrentMethod()?.Name, id.ToString());
            var context = new FreebusContext();
            context.SetPduString(cmd);
            context.NewLine = NewLine;
            var ret = ExecuteCustomMessage(context);
            return ret.Succeed;
        }
        catch (Exception e)
        {
            Transport.Logger.Error(
                $"{MethodBase.GetCurrentMethod()?.Name}命令异常,{e.Message}");
            return false;
        }
    }

    /// <summary>
    /// 机器人下使能
    /// </summary>
    /// <returns></returns>
    public bool SetMotorsOff(int id = 0)
    {
        try
        {
            var cmd = CreateCmd(MethodBase.GetCurrentMethod()?.Name, id.ToString());
            var context = new FreebusContext();
            context.SetPduString(cmd);
            context.NewLine = NewLine;
            var ret = ExecuteCustomMessage(context);
            return ret.Succeed;
        }
        catch (Exception e)
        {
            Transport.Logger.Error(
                $"{MethodBase.GetCurrentMethod()?.Name}命令异常,{e.Message}");
            return false;
        }
    }

    /// <summary>
    /// 机器人回零
    /// </summary>
    /// <returns></returns>
    public bool Home(int waitTime = DefaultDelay, int id = 0)
    {
        try
        {
            var temp = Transport.ReadTimeout;
            Transport.ReadTimeout = waitTime;
            var cmd = CreateCmd(MethodBase.GetCurrentMethod()?.Name, id.ToString());
            var context = new FreebusContext();
            context.SetPduString(cmd);
            context.NewLine = NewLine;
            var ret = ExecuteCustomMessage(context);
            Transport.ReadTimeout = temp;
            return ret.Succeed;
        }
        catch (Exception e)
        {
            Transport.Logger.Error(
                $"{MethodBase.GetCurrentMethod()?.Name}命令异常,{e.Message}");
            return false;
        }
    }

    /// <summary>
    /// 读取IO
    /// </summary>
    /// <returns></returns>
    public bool GetIO(int id)
    {
        try
        {
            var cmd = CreateCmd(MethodBase.GetCurrentMethod()?.Name, id.ToString());
            var context = new FreebusContext();
            context.SetPduString(cmd);
            context.NewLine = NewLine;
            var ret = ExecuteCustomMessage(context).GetDruString();
            var temp = ret.Split(',')[1].Substring(0, 1);
            return temp == "1";
        }
        catch (Exception e)
        {
            Transport.Logger.Error(
                $"{MethodBase.GetCurrentMethod()?.Name}命令异常,{e.Message}");
            return false;
        }
    }

    /// <summary>
    /// 设置IO
    /// </summary>
    /// <param name="id"></param>
    /// <param name="open"></param>
    /// <returns></returns>
    public bool SetIO(int id, bool open)
    {
        try
        {
            var openRet = open ? "1" : "0";
            var cmd = CreateCmd(MethodBase.GetCurrentMethod()?.Name, $"{id},{openRet}");
            var context = new FreebusContext();
            context.SetPduString(cmd);
            context.NewLine = NewLine;
            var ret = ExecuteCustomMessage(context);
            return ret.Succeed;
        }
        catch (Exception e)
        {
            Transport.Logger.Error(
                $"{MethodBase.GetCurrentMethod()?.Name}命令异常,{e.Message}");
            return false;
        }
    }

    /// <summary>
    /// 执行SPEL程序
    /// </summary>
    /// <param name="spel">执行SPEL+语言命令，命令需要用引号</param>
    /// <param name="waitTime">等待时间，Execute命令必须等待机器人任务完全结束才有返回</param>
    /// <returns></returns>
    public bool Execute(string spel, int waitTime = DefaultDelay)
    {
        try
        {
            var temp = Transport.ReadTimeout;
            Transport.ReadTimeout = waitTime;
            var cmd = CreateCmd(MethodBase.GetCurrentMethod()?.Name, spel);
            var context = new FreebusContext();
            context.SetPduString(cmd);
            context.NewLine = NewLine;
            var ret = ExecuteCustomMessage(context);
            Transport.ReadTimeout = temp;
            return ret.Succeed;
        }
        catch (Exception e)
        {
            Transport.Logger.Error(
                $"{MethodBase.GetCurrentMethod()?.Name}命令异常,{e.Message}");
            return false;
        }
    }
}