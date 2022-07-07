using System;
using Palink.Tools.Freebus.Device;
using Palink.Tools.Freebus.Interface;
using Palink.Tools.Freebus.Message;
using Palink.Tools.Logging;

namespace Palink.Tools.Robots.UPS;

/// <summary>
/// 山特UPS
/// </summary>
public class SanTakUPSMaster : FreebusMaster
{
    internal SanTakUPSMaster(IFreebusTransport transport) : base(transport)
    {
    }

    public bool ShouldClose()
    {
        try
        {
            var context = new FreebusContext();
            context.SetPduString("Q1\n");
            context.DruLength = 47;
            Transport.UnicastMessage(context);

            var buffer = context.GetDruString();
            var ret = buffer.Substring(38, 8);
            return !ret.StartsWith("0");
        }
        catch (Exception e)
        {
            Transport.Logger.Error(e.Message);
            return false;
        }
    }

    public bool AtClosing()
    {
        try
        {
            var context = new FreebusContext();
            context.SetPduString("Q1\n");
            context.DruLength = 47;
            Transport.UnicastMessage(context);

            var buffer = context.GetDruString();
            var ret = buffer.Substring(38, 8);
            return ret[6] == '1';
        }
        catch (Exception e)
        {
            Transport.Logger.Error(e.Message);
            return false;
        }
    }

    public bool CloseUPS()
    {
        try
        {
            var context = new FreebusContext();
            // context.SetPduString("S.1R0001\n");
            context.SetPduString("S01\n");
            context.DruLength = 3;
            Transport.UnicastMessage(context);

            var buffer = context.GetDruString();
            //失败为NAK
            return buffer == "ACK";
        }
        catch (Exception e)
        {
            Transport.Logger.Error(e.Message);
            return false;
        }
    }
}

/// <summary>
/// 商宇UPS
/// </summary>
public class CPSYUPSMaster : FreebusMaster
{
    internal CPSYUPSMaster(IFreebusTransport transport) : base(transport)
    {
    }

    public bool ShouldClose()
    {
        try
        {
            var context = new FreebusContext();
            context.SetPduString("Q1\n");
            context.DruLength = 47;
            Transport.UnicastMessage(context);

            var buffer = context.GetDruString();
            var ret = buffer.Substring(38, 8);
            return !ret.StartsWith("0");
        }
        catch (Exception e)
        {
            Transport.Logger.Error(e.Message);
            return false;
        }
    }

    public bool AtClosing()
    {
        try
        {
            var context = new FreebusContext();
            context.SetPduString("Q1\n");
            context.DruLength = 47;
            Transport.UnicastMessage(context);

            var buffer = context.GetDruString();
            var ret = buffer.Substring(38, 8);
            return ret[6] == '1';
        }
        catch (Exception e)
        {
            Transport.Logger.Error(e.Message);
            return false;
        }
    }

    /// <summary>
    /// 重启UPS：关闭UPS直到市电通电为止
    /// </summary>
    /// <returns></returns>
    public bool RestartUPS()
    {
        try
        {
            var context = new FreebusContext();
            // context.SetPduString("S.1R0001\n");
            context.SetPduString("S.1R0001\n");
            context.DruLength = 8;
            Transport.UnicastMessage(context);

            var buffer = context.GetDruString();
            //失败为NAK
            return buffer.Contains("ACK");
        }
        catch (Exception e)
        {
            Transport.Logger.Error(e.Message);
            return false;
        }
    }
}