using System;
using System.Reflection;
using Palink.Tools.Extensions.ConvertExt;
using Palink.Tools.Freebus.Device;
using Palink.Tools.Freebus.Interface;
using Palink.Tools.Freebus.Message;
using Palink.Tools.Logging;
using Palink.Tools.Utility;

namespace Palink.Tools.Robots.LQ;

public class LQMaster : FreebusMaster
{
    private const int DefaultDelay = 5000;

    internal LQMaster(IFreebusTransport transport) : base(transport)
    {
    }

    public string? ErrorMessage { get; set; }

    /// <summary>
    /// 机器人登录
    /// </summary>
    /// <param name="level">
    /// 0 最高权限，具备远程修改系统属性、操作机器人移动、监控机器人
    /// 状态、获取机器人信息日志功能；（该权限不建议向初级操作用户开放，请谨慎使用和修改参数）
    /// 1 控制权限，具备操作机器人移动、监控机器人状态、获取机器人信息日志功能；
    /// 2 监控权限，监控机器人状态、获取机器人信息日志功能
    /// 0 和 1 权限互斥，只能同时有一个账户在线，可以同时有多个监控权限账户在线。
    /// </param>
    /// <returns></returns>
    public bool Login(int level)
    {
        try
        {
            var cmd = $"[1#{LQCmd.System}.{LQSubCmd.Login} {level}]";
            var context = new FreebusContext();
            context.SetPduString(cmd);
            context.NewLine = "]";
            var ret = ExecuteCustomMessage(context);
            return ret.Succeed;
        }
        catch (Exception e)
        {
            Transport.Logger.Error(
                $"命令{MethodBase.GetCurrentMethod()?.Name}异常：{e.Message}");
            return false;
        }
    }

    /// <summary>
    /// 退出
    /// </summary>
    /// <returns></returns>
    public bool Logout()
    {
        try
        {
            const string cmd = $"[1#{LQCmd.System}.{LQSubCmd.Logout}]";
            var context = new FreebusContext();
            context.SetPduString(cmd);
            context.NewLine = "]";
            var ret = ExecuteCustomMessage(context);
            return ret.Succeed;
        }
        catch (Exception e)
        {
            Transport.Logger.Error(
                $"命令{MethodBase.GetCurrentMethod()?.Name}异常：{e.Message}");
            return false;
        }
    }

    /// <summary>
    /// 自动模式
    /// </summary>
    /// <param name="state"></param>
    /// <returns></returns>
    public bool Auto(bool state)
    {
        try
        {
            var stateStr = state ? "1" : "0";
            var cmd = $"[1#{LQCmd.System}.{LQSubCmd.Auto} {stateStr}]";
            var context = new FreebusContext();
            context.SetPduString(cmd);
            context.NewLine = "]";
            var ret = ExecuteCustomMessage(context);
            return ret.Succeed;
        }
        catch (Exception e)
        {
            Transport.Logger.Error(
                $"命令{MethodBase.GetCurrentMethod()?.Name}异常：{e.Message}");
            return false;
        }
    }

    /// <summary>
    /// 宏指令模式
    /// </summary>
    /// <param name="isMacro"></param>
    /// <returns></returns>
    public bool ModeSwitch(bool isMacro = true)
    {
        try
        {
            var isMacroStr = isMacro ? "1" : "0";
            var cmd = $"[1#{LQCmd.System}.{LQSubCmd.ModeSwitch} {isMacroStr}]";
            var context = new FreebusContext();
            context.SetPduString(cmd);
            context.NewLine = "]";
            var ret = ExecuteCustomMessage(context);
            return ret.Succeed;
        }
        catch (Exception e)
        {
            Transport.Logger.Error(
                $"命令{MethodBase.GetCurrentMethod()?.Name}异常：{e.Message}");
            return false;
        }
    }

    /// <summary>
    /// 上使能
    /// </summary>
    /// <param name="enable"></param>
    /// <param name="id"></param>
    /// <param name="waitTime"></param>
    /// <returns></returns>
    public bool PowerEnable(bool enable, int id = 1, int waitTime = DefaultDelay)
    {
        try
        {
            var enableStr = enable ? "1" : "0";
            var cmd = $"[1#{LQCmd.Robot}.{LQSubCmd.PowerEnable} {id},{enableStr}]";
            var context = new FreebusContext();
            context.SetPduString(cmd);
            context.NewLine = "]";
            ExecuteCustomMessage(context);
            var temp = Transport.StreamResource.ReadTimeout;
            Transport.StreamResource.ReadTimeout = waitTime;
            var res = CoreTool.ReadLine(Transport.StreamResource, "]").Split('#');
            Transport.StreamResource.ReadTimeout = temp;
            return res[1].StartsWith("1");
        }
        catch (Exception e)
        {
            Transport.Logger.Error(
                $"命令{MethodBase.GetCurrentMethod()?.Name}异常：{e.Message}");
            return false;
        }
    }

    /// <summary>
    /// 状态
    /// </summary>
    /// <returns></returns>
    public (bool power, bool homed) GetState(int id = 1)
    {
        try
        {
            var cmd = $"[1#{LQCmd.Robot}.{LQSubCmd.State} {id}]";
            var context = new FreebusContext();
            context.SetPduString(cmd);
            context.NewLine = "]";
            var ret = ExecuteCustomMessage(context);
            var res = ret.GetDruString().Split('#')[1].Split(' ')[1].Split(',');
            return (res[0].To<int>() == 1, res[1].To<int>() == 1);
        }
        catch (Exception e)
        {
            Transport.Logger.Error(
                $"命令{MethodBase.GetCurrentMethod()?.Name}异常：{e.Message}");
            return (false, false);
        }
    }

    /// <summary>
    /// 回零
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public bool Home(int id = 1)
    {
        try
        {
            var cmd = $"[1#{LQCmd.Robot}.{LQSubCmd.Home} {id}]";
            var context = new FreebusContext();
            context.SetPduString(cmd);
            context.NewLine = "]";
            var ret = ExecuteCustomMessage(context);
            return ret.Succeed;
        }
        catch (Exception e)
        {
            Transport.Logger.Error(
                $"命令{MethodBase.GetCurrentMethod()?.Name}异常：{e.Message}");
            return false;
        }
    }
    
    /// <summary>
    /// 读取速度
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public double GetSpeed(int id = 1)
    {
        try
        {
            var cmd = $"[1#{LQCmd.Robot}.{LQSubCmd.Speed} {id}]";
            var context = new FreebusContext();
            context.SetPduString(cmd);
            context.NewLine = "]";
            var ret = ExecuteCustomMessage(context);
            var res = ret.GetDruString().Split('#')[1].Split(' ')[1];
            var speed = double.Parse(res);
            return speed;
        }
        catch (Exception e)
        {
            Transport.Logger.Error(
                $"命令{MethodBase.GetCurrentMethod()?.Name}异常：{e.Message}");
            return 0;
        }
    }

    /// <summary>
    /// 读取系统速度
    /// </summary>
    /// <returns></returns>
    public double GetSysSpeed()
    {
        try
        {
            const string cmd = $"[1#{LQCmd.System}.{LQSubCmd.Speed}]";
            var context = new FreebusContext();
            context.SetPduString(cmd);
            context.NewLine = "]";
            var ret = ExecuteCustomMessage(context);
            var res = ret.GetDruString().Split('#')[1].Split(' ')[1];
            var speed = double.Parse(res);
            return speed;
        }
        catch (Exception e)
        {
            Transport.Logger.Error(
                $"命令{MethodBase.GetCurrentMethod()?.Name}异常：{e.Message}");
            return 0;
        }
    }

    /// <summary>
    /// 设置速度
    /// </summary>
    /// <param name="speed"></param>
    /// <param name="id"></param>
    /// <returns></returns>
    public bool SetSpeed(double speed, int id = 1)
    {
        try
        {
            var cmd = $"[1#{LQCmd.Robot}.{LQSubCmd.Speed} {id},{speed}]";
            var context = new FreebusContext();
            context.SetPduString(cmd);
            context.NewLine = "]";
            var ret = ExecuteCustomMessage(context);
            return ret.Succeed;
        }
        catch (Exception e)
        {
            Transport.Logger.Error(
                $"命令{MethodBase.GetCurrentMethod()?.Name}异常：{e.Message}");
            return false;
        }
    }

    /// <summary>
    /// 设置系统速度
    /// </summary>
    /// <param name="speed"></param>
    /// <returns></returns>
    public bool SetSysSpeed(double speed)
    {
        try
        {
            var cmd = $"[1#{LQCmd.System}.{LQSubCmd.Speed} {speed}]";
            var context = new FreebusContext();
            context.SetPduString(cmd);
            context.NewLine = "]";
            var ret = ExecuteCustomMessage(context);
            return ret.Succeed;
        }
        catch (Exception e)
        {
            Transport.Logger.Error(
                $"命令{MethodBase.GetCurrentMethod()?.Name}异常：{e.Message}");
            return false;
        }
    }

    /// <summary>
    /// 读取位置
    /// </summary>
    /// <param name="id"></param>
    /// <returns>m表示配置是左手系还是右手系</returns>
    public (double x, double y, double z, double u, double m) GetPosition(int id = 1)
    {
        try
        {
            var cmd = $"[1#{LQCmd.Robot}.{LQSubCmd.Where} {id}]";
            var context = new FreebusContext();
            context.SetPduString(cmd);
            context.NewLine = "]";

            for (var i = 0; i < Transport.Retries; i++)
            {
                try
                {
                    var ret = ExecuteCustomMessage(context);
                    var res = ret.GetDruString()
                        .Split('#')[1]
                        .Split(' ')[1]
                        .Split(',');

                    var x = double.Parse(res[0]);
                    var y = double.Parse(res[1]);
                    var z = double.Parse(res[2]);
                    var u = double.Parse(res[5]);
                    var m = double.Parse(res[6]);
                    return (x, y, z, u, m);
                }
                catch (Exception)
                {
                    // ignored
                }
            }

            return (0d, 0d, 0d, 0d, 0d);
        }
        catch (Exception e)
        {
            Transport.Logger.Error(
                $"命令{MethodBase.GetCurrentMethod()?.Name}异常：{e.Message}");
            return (0, 0, 0, 0, 0);
        }
    }

    /// <summary>
    /// 记录点位
    /// </summary>
    /// <param name="pName"></param>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="z"></param>
    /// <param name="u"></param>
    /// <param name="m"></param>
    /// <returns></returns>
    public bool RecordPosition(string pName, double x, double y, double z, double u,
        double m)
    {
        try
        {
            var cmd = $"[1#{LQCmd.Location} {pName} = {x},{y},{z},{0},{180},{u},{m}]";
            var context = new FreebusContext();
            context.SetPduString(cmd);
            context.NewLine = "]";
            var ret = ExecuteCustomMessage(context);
            return ret.Succeed;
        }
        catch (Exception e)
        {
            Transport.Logger.Error(
                $"命令{MethodBase.GetCurrentMethod()?.Name}异常：{e.Message}");
            return false;
        }
    }

    /// <summary>
    /// 删除点位数据
    /// </summary>
    /// <param name="pName"></param>
    /// <returns></returns>
    public bool DeletePosition(string pName)
    {
        try
        {
            var cmd = $"[1#{pName}.Delete]";
            var context = new FreebusContext();
            context.SetPduString(cmd);
            context.NewLine = "]";
            var ret = ExecuteCustomMessage(context);
            return ret.Succeed;
        }
        catch (Exception e)
        {
            Transport.Logger.Error(
                $"命令{MethodBase.GetCurrentMethod()?.Name}异常：{e.Message}");
            return false;
        }
    }

    /// <summary>
    /// 执行点位
    /// </summary>
    /// <param name="pName"></param>
    /// <param name="waitTime"></param>
    /// <returns></returns>
    public bool ExecutePosition(string pName, int waitTime = DefaultDelay)
    {
        try
        {
            var cmd = $"[1#{LQCmd.Move}.{LQSubCmd.Joint} {pName}]";
            var context = new FreebusContext();
            context.SetPduString(cmd);
            context.NewLine = "]";
            ExecuteCustomMessage(context);
            var temp = Transport.StreamResource.ReadTimeout;
            Transport.StreamResource.ReadTimeout = waitTime;
            var res = CoreTool.ReadLine(Transport.StreamResource, "]").Split('#');
            Transport.StreamResource.ReadTimeout = temp;
            return res[1].StartsWith("1");
        }
        catch (Exception e)
        {
            Transport.Logger.Error(
                $"命令{MethodBase.GetCurrentMethod()?.Name}异常：{e.Message}");
            return false;
        }
    }

    /// <summary>
    /// 读取IO
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public bool GetIO(uint id)
    {
        try
        {
            var cmd = $"[1#{LQCmd.IO}.Get {LQSubCmd.DIN}({LQSubCmd.DINStart + id})]";
            var context = new FreebusContext();
            context.SetPduString(cmd);
            context.NewLine = "]";
            var ret = ExecuteCustomMessage(context);
            var res = ret.GetDruString().Split('#')[1].Split(' ')[1];
            return res == "1";
        }
        catch (Exception e)
        {
            Transport.Logger.Error(
                $"命令{MethodBase.GetCurrentMethod()?.Name}异常：{e.Message}");
            return false;
        }
    }

    /// <summary>
    /// 设置IO
    /// </summary>
    /// <param name="id"></param>
    /// <param name="state"></param>
    /// <returns></returns>
    public bool SetIO(uint id, bool state)
    {
        try
        {
            var stateStr = state ? "1" : "0";
            var cmd =
                $"[1#{LQCmd.IO}.Set {LQSubCmd.DOUT}({LQSubCmd.DOUTStart + id}),{stateStr}]";
            var context = new FreebusContext();
            context.SetPduString(cmd);
            context.NewLine = "]";
            var ret = ExecuteCustomMessage(context);
            return ret.Succeed;
        }
        catch (Exception e)
        {
            Transport.Logger.Error(
                $"命令{MethodBase.GetCurrentMethod()?.Name}异常：{e.Message}");
            return false;
        }
    }

    /// <summary>
    /// 启动线程
    /// </summary>
    /// <returns></returns>
    public bool StartThread()
    {
        try
        {
            const string cmd = $"[1#{LQCmd.Thread}.{LQSubCmd.Start}]";
            var context = new FreebusContext();
            context.SetPduString(cmd);
            context.NewLine = "]";
            var ret = ExecuteCustomMessage(context);
            return ret.Succeed;
        }
        catch (Exception e)
        {
            Transport.Logger.Error(
                $"命令{MethodBase.GetCurrentMethod()?.Name}异常：{e.Message}");
            return false;
        }
    }
    
    /// <summary>
    /// 终止线程
    /// </summary>
    /// <returns></returns>
    public bool AbortThread()
    {
        try
        {
            const string cmd = $"[1#{LQCmd.Thread}.{LQSubCmd.Abort}]";
            var context = new FreebusContext();
            context.SetPduString(cmd);
            context.NewLine = "]";
            var ret = ExecuteCustomMessage(context);
            return ret.Succeed;
        }
        catch (Exception e)
        {
            Transport.Logger.Error(
                $"命令{MethodBase.GetCurrentMethod()?.Name}异常：{e.Message}");
            return false;
        }
    }
}

public static class LQCmd
{
    public const string System = nameof(System);

    public const string Robot = nameof(Robot);

    public const string Move = nameof(Move);

    public const string Location = nameof(Location);

    public const string IO = nameof(IO);

    public const string Thread = nameof(Thread);
}

public static class LQSubCmd
{
    public const string Login = nameof(Login);

    public const string Logout = nameof(Logout);

    public const string PowerEnable = nameof(PowerEnable);

    public const string Home = nameof(Home);

    public const string Speed = nameof(Speed);

    public const string State = nameof(State);

    public const string Where = nameof(Where);

    public const string Joint = nameof(Joint);

    public const string ClearErrorList = nameof(ClearErrorList);

    public const string Auto = nameof(Auto);

    public const string ModeSwitch = nameof(ModeSwitch);

    public const string Delete = nameof(Delete);

    public const string DIN = nameof(DIN);

    public const string DOUT = nameof(DOUT);

    public const string Start = nameof(Start);

    public const string Abort = nameof(Abort);

    public const int DINStart = 10101;

    public const int DOUTStart = 20101;
}