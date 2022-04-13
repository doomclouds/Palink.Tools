using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Text;
using Palink.Tools.Communication;
using Palink.Tools.Communication.Interface;
using Palink.Tools.Communication.Message;
using Palink.Tools.Extensions.PLLogging;

namespace Palink.Tools.PanShi.Robots;

/// <summary>
/// EpsonMaster
/// </summary>
public class EpsonMaster : Master
{
    private const string NewLine = "\r\n";

    /// <summary>
    /// Master
    /// </summary>
    /// <param name="streamResource"></param>
    /// <param name="logger"></param>
    public EpsonMaster(IStreamResource streamResource, IPlLogger logger) : base(
        streamResource, logger)
    {
    }

    /// <summary>
    /// 数据校验
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    public override bool CheckData(IMessage message)
    {
        return true;
    }

    /// <summary>
    /// 创建发送数据体
    /// </summary>
    /// <param name="noCheckFrame"></param>
    /// <returns></returns>
    public override IMessage? CreateFrame((byte id, byte cmd, byte[] frame) noCheckFrame)
    {
        return null;
    }

    /// <summary>
    /// 创建字符串发送数据体
    /// </summary>
    /// <param name="frame"></param>
    /// <returns></returns>
    public override IMessage CreateStringFrame(string frame)
    {
        var data = Encoding.UTF8.GetBytes(frame);
        var message = new BaseMessage()
        {
            Data = data,
            SendBytes = data.Length,
            ReadBytes = 100
        };
        return message;
    }

    /// <summary>
    /// 机器人登录
    /// </summary>
    /// <returns></returns>
    public bool Login(string pwd)
    {
        try
        {
            var message =
                CreateStringFrame(CreateCmd(MethodBase.GetCurrentMethod()?.Name, pwd));
            Unicast(message, true, false);
            var ret = Encoding.UTF8.GetString(message.Buffer).Trim('\0');
            return ret.Contains("#");
        }
        catch (Exception e)
        {
            PlLogger.Error($"{MethodBase.GetCurrentMethod()?.Name}命令异常,{e.Message}");
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
            var message =
                CreateStringFrame(CreateCmd(MethodBase.GetCurrentMethod()?.Name, ""));
            Unicast(message, true, false);
            var ret = Encoding.UTF8.GetString(message.Buffer).Trim('\0');
            return ret.Contains("#");
        }
        catch (Exception e)
        {
            PlLogger.Error($"{MethodBase.GetCurrentMethod()?.Name}命令异常,{e.Message}");
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
            var message =
                CreateStringFrame(CreateCmd(MethodBase.GetCurrentMethod()?.Name, ""));
            Unicast(message, true, false);
            var ret = Encoding.UTF8.GetString(message.Buffer).Trim('\0');
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
            PlLogger.Error($"{MethodBase.GetCurrentMethod()?.Name}命令异常,{e.Message}");
            return (false, false, false, false, false, false);
        }
    }

    /// <summary>
    /// 执行Main程序
    /// </summary>
    /// <param name="id"></param>
    /// <param name="wait">是否等待数据返回</param>
    /// <param name="waitTime">等待时间，Start命令必须等待机器人任务完全结束才有返回</param>
    /// <returns></returns>
    public bool Start(int id, bool wait = false, int waitTime = 1000)
    {
        var temp = StreamResource.ReadTimeout;
        try
        {
            var message =
                CreateStringFrame(CreateCmd(MethodBase.GetCurrentMethod()?.Name,
                    id.ToString()));
            if (wait)
            {
                StreamResource.ReadTimeout = waitTime;
                Unicast(message, true, false);
                var ret = Encoding.UTF8.GetString(message.Buffer).Trim('\0');
                return ret.Contains("#");
            }
            else
            {
                SendData(message, false);
                return true;
            }
        }
        catch (Exception e)
        {
            PlLogger.Error($"{MethodBase.GetCurrentMethod()?.Name}命令异常,{e.Message}");
            return false;
        }
        finally
        {
            StreamResource.ReadTimeout = temp;
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
            var status = GetStatus();
            if (status.ready || status.auto)
                return true;
            var message =
                CreateStringFrame(CreateCmd(MethodBase.GetCurrentMethod()?.Name, ""));
            Unicast(message, true, false);
            var ret = Encoding.UTF8.GetString(message.Buffer).Trim('\0');
            return ret.Contains("#");
        }
        catch (Exception e)
        {
            PlLogger.Error($"{MethodBase.GetCurrentMethod()?.Name}命令异常,{e.Message}");
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
            var message =
                CreateStringFrame(CreateCmd(MethodBase.GetCurrentMethod()?.Name,
                    id.ToString()));
            Unicast(message, true, false);
            var ret = Encoding.UTF8.GetString(message.Buffer).Trim('\0');
            return ret.Contains("#");
        }
        catch (Exception e)
        {
            PlLogger.Error($"{MethodBase.GetCurrentMethod()?.Name}命令异常,{e.Message}");
            return false;
        }
    }

    /// <summary>
    /// 机器人下使能
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public bool SetMotorsOff(int id = 0)
    {
        try
        {
            var message =
                CreateStringFrame(CreateCmd(MethodBase.GetCurrentMethod()?.Name,
                    id.ToString()));
            Unicast(message, true, false);
            var ret = Encoding.UTF8.GetString(message.Buffer).Trim('\0');
            return ret.Contains("#");
        }
        catch (Exception e)
        {
            PlLogger.Error($"{MethodBase.GetCurrentMethod()?.Name}命令异常,{e.Message}");
            return false;
        }
    }

    /// <summary>
    /// 机器人回零
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public bool Home(int id = 0)
    {
        try
        {
            var message =
                CreateStringFrame(CreateCmd(MethodBase.GetCurrentMethod()?.Name,
                    id.ToString()));
            Unicast(message, true, false);
            var ret = Encoding.UTF8.GetString(message.Buffer).Trim('\0');
            return ret.Contains("#");
        }
        catch (Exception e)
        {
            PlLogger.Error($"{MethodBase.GetCurrentMethod()?.Name}命令异常,{e.Message}");
            return false;
        }
    }

    /// <summary>
    /// 读取IO
    /// </summary>
    /// <returns></returns>
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public bool GetIO(int id)
    {
        try
        {
            var message =
                CreateStringFrame(CreateCmd(MethodBase.GetCurrentMethod()?.Name,
                    id.ToString()));
            Unicast(message, true, false);
            var msg = Encoding.UTF8.GetString(message.Buffer).Trim('\0');
            var temp = msg.Split(',')[1].Substring(0, 1);
            return temp == "1";
        }
        catch (Exception e)
        {
            PlLogger.Error($"{MethodBase.GetCurrentMethod()?.Name}命令异常,{e.Message}");
            return false;
        }
    }

    /// <summary>
    /// 设置IO
    /// </summary>
    /// <param name="id"></param>
    /// <param name="open"></param>
    /// <returns></returns>
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public bool SetIO(int id, bool open)
    {
        try
        {
            var openRet = open ? "1" : "0";
            var message =
                CreateStringFrame(CreateCmd(MethodBase.GetCurrentMethod()?.Name,
                    $"{id},{openRet}"));
            Unicast(message, true, false);
            var ret = Encoding.UTF8.GetString(message.Buffer).Trim('\0');
            return ret.Contains("#");
        }
        catch (Exception e)
        {
            PlLogger.Error($"{MethodBase.GetCurrentMethod()?.Name}命令异常,{e.Message}");
            return false;
        }
    }

    /// <summary>
    /// 执行Main程序
    /// </summary>
    /// <param name="cmd">执行SPEL+语言命令，命令需要用引号</param>
    /// <param name="wait">是否等待数据返回</param>
    /// <param name="waitTime">等待时间，Start命令必须等待机器人任务完全结束才有返回</param>
    /// <returns></returns>
    public bool Execute(string cmd, bool wait = false, int waitTime = 1000)
    {
        var temp = StreamResource.ReadTimeout;
        try
        {
            var c =CreateCmd(MethodBase.GetCurrentMethod()?.Name,
                    cmd);
            var message =
                CreateStringFrame(CreateCmd(MethodBase.GetCurrentMethod()?.Name,
                    cmd));
            if (wait)
            {
                StreamResource.ReadTimeout = waitTime;
                Unicast(message, true, false);
                var ret = Encoding.UTF8.GetString(message.Buffer).Trim('\0');
                return ret.Contains("#");
            }
            else
            {
                SendData(message, false);
                return true;
            }
        }
        catch (Exception e)
        {
            PlLogger.Error($"{MethodBase.GetCurrentMethod()?.Name}命令异常,{e.Message}");
            return false;
        }
        finally
        {
            StreamResource.ReadTimeout = temp;
        }
    }

    private string CreateCmd(string? cmd, string parameters)
    {
        return $"${cmd},{parameters}{NewLine}";
    }
}