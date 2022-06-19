using System;
using System.Reflection;
using System.Threading.Tasks;
using Palink.Tools.Freebus.Device;
using Palink.Tools.Freebus.Interface;
using Palink.Tools.Freebus.Message;
using Palink.Tools.Logging;

namespace Palink.Tools.Robots.DiskWall;

public enum DiskWallBaudRate : byte
{
    None = 0x00,
    Br115200,
    Br150K,
    Br200K,
    Br250K,
    Br300K,
    Br350K,
    Br400K,
    Br450K,
    Br500K
}

public enum DiskWallResponseStatus
{
    Ok = 0x00,
    Busy,
    Overflow,
    None
}

public class DiskWallMaster : FreebusMaster
{
    private enum Cmd : byte
    {
        OverturnNoResponse = 0x01,
        Overturn,
        WriteBaudRate,
        WriteHoldOnTime,
        WriteHoldOffTime,
        WriteDelayTime,
        WritePwmCount,
        Save,
        ReadBaudRate = 0x13,
        ReadHoldOnTime,
        ReadHoldOffTime,
        ReadDelayTime,
        ReadPwmCount
    }

    internal DiskWallMaster(IFreebusTransport transport) : base(transport)
    {
    }

    public void OverturnNoResponse(byte id, string unitData, char split = ',',
        bool shouldLog = false)
    {
        try
        {
            var data = unitData.Split(split);
            var frame = new byte[data.Length];
            for (var i = 0; i < data.Length; i++)
            {
                frame[i] = Convert.ToByte(data[i], 16);
            }

            var context = CreateContext((id, (byte)Cmd.OverturnNoResponse, frame));

            Transport.BroadcastMessage(context, shouldLog);
        }
        catch (Exception ex)
        {
            Transport.Logger.Error(
                $"{MethodBase.GetCurrentMethod()?.Name}命令异常:{ex.Message};{ex.StackTrace}");
        }
    }

    public Task OverturnNoResponseAsync(byte id, string unitData, char split = ',',
        bool shouldLog = false)
    {
        return Task.Run(() => OverturnNoResponse(id, unitData, split, shouldLog));
    }

    public DiskWallResponseStatus Overturn(byte id, string unitData, char split = ',')
    {
        try
        {
            var data = unitData.Split(split);
            var frame = new byte[data.Length];
            for (var i = 0; i < data.Length; i++)
            {
                frame[i] = Convert.ToByte(data[i], 16);
            }

            var context = CreateContext((id, (byte)Cmd.Overturn, frame));
            context.DruLength = 8;
            Transport.UnicastMessage(context);
            return (DiskWallResponseStatus)context.Dru[context.Dru.Length - 2];
        }
        catch (Exception ex)
        {
            Transport.Logger.Error(
                $"{MethodBase.GetCurrentMethod()?.Name}命令异常:{ex.Message};{ex.StackTrace}");
            return DiskWallResponseStatus.None;
        }
    }

    public Task<DiskWallResponseStatus> OverturnAsync(byte id, string unitData,
        char split = ',')
    {
        return Task.Run(() => Overturn(id, unitData, split));
    }

    public bool WriteBaudRate(byte id, DiskWallBaudRate baudRate)
    {
        try
        {
            var frame = new[] { (byte)baudRate };
            var context = CreateContext((id, (byte)Cmd.WriteBaudRate, frame));
            Transport.UnicastMessage(context);
            return true;
        }
        catch (Exception ex)
        {
            Transport.Logger.Error(
                $"{MethodBase.GetCurrentMethod()?.Name}命令异常:{ex.Message};{ex.StackTrace}");
            return false;
        }
    }

    public Task<bool> WriteBaudRateAsync(byte id, DiskWallBaudRate baudRate)
    {
        return Task.Run(() => WriteBaudRate(id, baudRate));
    }

    public DiskWallBaudRate ReadBaudRate(byte id,
        DiskWallBaudRate baudRate = DiskWallBaudRate.Br115200)
    {
        try
        {
            var frame = new[] { (byte)baudRate };
            var context = CreateContext((id, (byte)Cmd.ReadBaudRate, frame));
            Transport.UnicastMessage(context);
            return (DiskWallBaudRate)context.Dru[context.Dru.Length - 2];
        }
        catch (Exception ex)
        {
            Transport.Logger.Error(
                $"{MethodBase.GetCurrentMethod()?.Name}命令异常:{ex.Message};{ex.StackTrace}");
            return DiskWallBaudRate.None;
        }
    }

    public Task<DiskWallBaudRate> ReadBaudRateAsync(byte id,
        DiskWallBaudRate baudRate = DiskWallBaudRate.Br115200)
    {
        return Task.Run(() => ReadBaudRate(id, baudRate));
    }

    public DiskWallResponseStatus WriteHoldOnTime(byte id, byte time)
    {
        try
        {
            var frame = new[] { time };
            var context = CreateContext((id, (byte)Cmd.WriteHoldOnTime, frame));
            Transport.UnicastMessage(context);
            return (DiskWallResponseStatus)context.Dru[context.Dru.Length - 2];
        }
        catch (Exception ex)
        {
            Transport.Logger.Error(
                $"{MethodBase.GetCurrentMethod()?.Name}命令异常:{ex.Message};{ex.StackTrace}");
            return DiskWallResponseStatus.None;
        }
    }

    public Task<DiskWallResponseStatus> WriteHoldOnTimeAsync(byte id, byte time)
    {
        return Task.Run(() => WriteHoldOnTime(id, time));
    }

    public byte ReadHoldOnTime(byte id, byte time = 0)
    {
        try
        {
            var frame = new[] { time };
            var context = CreateContext((id, (byte)Cmd.ReadHoldOnTime, frame));
            Transport.UnicastMessage(context);
            return context.Dru[context.Dru.Length - 2];
        }
        catch (Exception ex)
        {
            Transport.Logger.Error(
                $"{MethodBase.GetCurrentMethod()?.Name}命令异常:{ex.Message};{ex.StackTrace}");
            return 0;
        }
    }

    public Task<byte> ReadHoldOnTimeAsync(byte id, byte time)
    {
        return Task.Run(() => ReadHoldOnTime(id, time));
    }

    public DiskWallResponseStatus WriteHoldOffTime(byte id, byte time)
    {
        try
        {
            var frame = new[] { time };
            var context = CreateContext((id, (byte)Cmd.WriteHoldOffTime, frame));
            Transport.UnicastMessage(context);
            return (DiskWallResponseStatus)context.Dru[context.Dru.Length - 2];
        }
        catch (Exception ex)
        {
            Transport.Logger.Error(
                $"{MethodBase.GetCurrentMethod()?.Name}命令异常:{ex.Message};{ex.StackTrace}");
            return DiskWallResponseStatus.None;
        }
    }

    public Task<DiskWallResponseStatus> WriteHoldOffTimeAsync(byte id, byte time)
    {
        return Task.Run(() => WriteHoldOffTime(id, time));
    }

    public byte ReadHoldOffTime(byte id, byte time = 0)
    {
        try
        {
            var frame = new[] { time };
            var context = CreateContext((id, (byte)Cmd.ReadHoldOffTime, frame));
            Transport.UnicastMessage(context);
            return context.Dru[context.Dru.Length - 2];
        }
        catch (Exception ex)
        {
            Transport.Logger.Error(
                $"{MethodBase.GetCurrentMethod()?.Name}命令异常:{ex.Message};{ex.StackTrace}");
            return 0;
        }
    }

    public Task<byte> ReadHoldOffTimeAsync(byte id, byte time)
    {
        return Task.Run(() => ReadHoldOffTime(id, time));
    }

    public DiskWallResponseStatus WriteDelayTime(byte id, byte time)
    {
        try
        {
            var frame = new[] { time };
            var context = CreateContext((id, (byte)Cmd.WriteDelayTime, frame));
            Transport.UnicastMessage(context);
            return (DiskWallResponseStatus)context.Dru[context.Dru.Length - 2];
        }
        catch (Exception ex)
        {
            Transport.Logger.Error(
                $"{MethodBase.GetCurrentMethod()?.Name}命令异常:{ex.Message};{ex.StackTrace}");
            return DiskWallResponseStatus.None;
        }
    }

    public Task<DiskWallResponseStatus> WriteDelayTimeAsync(byte id, byte time)
    {
        return Task.Run(() => WriteDelayTime(id, time));
    }

    public byte ReadDelayTime(byte id, byte time = 0)
    {
        try
        {
            var frame = new[] { time };
            var context = CreateContext((id, (byte)Cmd.ReadDelayTime, frame));
            Transport.UnicastMessage(context);
            return context.Dru[context.Dru.Length - 2];
        }
        catch (Exception ex)
        {
            Transport.Logger.Error(
                $"{MethodBase.GetCurrentMethod()?.Name}命令异常:{ex.Message};{ex.StackTrace}");
            return 0;
        }
    }

    public Task<byte> ReadDelayTimeAsync(byte id, byte time)
    {
        return Task.Run(() => ReadDelayTime(id, time));
    }

    public DiskWallResponseStatus WritePwmCount(byte id, byte time)
    {
        try
        {
            var frame = new[] { time };
            var context = CreateContext((id, (byte)Cmd.WritePwmCount, frame));
            Transport.UnicastMessage(context);
            return (DiskWallResponseStatus)context.Dru[context.Dru.Length - 2];
        }
        catch (Exception ex)
        {
            Transport.Logger.Error(
                $"{MethodBase.GetCurrentMethod()?.Name}命令异常:{ex.Message};{ex.StackTrace}");
            return DiskWallResponseStatus.None;
        }
    }

    public Task<DiskWallResponseStatus> WritePwmCountAsync(byte id, byte time)
    {
        return Task.Run(() => WritePwmCount(id, time));
    }

    public byte ReadPwmCount(byte id, byte time = 0)
    {
        try
        {
            var frame = new[] { time };
            var context = CreateContext((id, (byte)Cmd.ReadPwmCount, frame));
            Transport.UnicastMessage(context);
            return context.Dru[context.Dru.Length - 2];
        }
        catch (Exception ex)
        {
            Transport.Logger.Error(
                $"{MethodBase.GetCurrentMethod()?.Name}命令异常:{ex.Message};{ex.StackTrace}");
            return 0;
        }
    }

    public Task<byte> ReadPwmCountAsync(byte id, byte time)
    {
        return Task.Run(() => ReadPwmCount(id, time));
    }

    public bool Save(byte id)
    {
        try
        {
            var frame = new byte[] { 0x00 };
            var context = CreateContext((id, (byte)Cmd.Save, frame));
            Transport.UnicastMessage(context);
            return context.Dru[context.Dru.Length - 2] == 0x00;
        }
        catch (Exception ex)
        {
            Transport.Logger.Error(
                $"{MethodBase.GetCurrentMethod()?.Name}命令异常:{ex.Message};{ex.StackTrace}");
            return false;
        }
    }

    public Task<bool> SaveAsync(byte id)
    {
        return Task.Run(() => Save(id));
    }

    public IFreebusContext CreateContext((byte id, byte cmd, byte[] frame) noCheckFrame)
    {
        var header = new byte[] { 0xa5, 0x5a };
        var data = new byte[header.Length + noCheckFrame.frame.Length + 5];
        var offset = 0;
        Buffer.BlockCopy(header, 0, data, offset, header.Length);
        offset += header.Length;
        data[offset] = noCheckFrame.cmd;
        offset++;
        data[offset] = noCheckFrame.id;
        offset++;
        byte[] length = BitConverter.GetBytes((short)noCheckFrame.frame.Length);
        Buffer.BlockCopy(length, 1, data, offset, 1);
        offset++;
        Buffer.BlockCopy(length, 0, data, offset, 1);
        offset++;
        Buffer.BlockCopy(noCheckFrame.frame, 0, data, offset, noCheckFrame.frame.Length);
        offset += noCheckFrame.frame.Length;

        var sumInt = 0;
        for (var i = 0; i < data.Length - 1; i++)
        {
            sumInt += data[i];
        }

        var sum = BitConverter.GetBytes(sumInt);
        data[offset] = sum[0];

        return new FreebusContext()
        {
            Pdu = data,
            DruLength = data.Length
        };
    }
}