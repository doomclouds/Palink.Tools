using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Palink.Tools.Extensions.PLAttribute;
using Palink.Tools.Logging;
using Palink.Tools.NModbus.Data;
using Palink.Tools.NModbus.Extensions.Enron;
using Palink.Tools.NModbus.Interfaces;
using Palink.Tools.NModbus.IO;
using Palink.Tools.Utility;

namespace Palink.Tools.Robots.YzAim;

public enum YzAimCmd
{
    [Description("Modbus使能")] ModbusEnable = 0x00,
    [Description("电机使能")] MotorEnable = 0x01,
    [Description("电机目标速度")] TargetSpeed = 0x02,
    [Description("电机加速度")] Acc = 0x03,
    [Description("弱磁角度")] WeakMagneticAngle = 0x04,
    [Description("速度环比例系数")] SpeedP = 0x05,
    [Description("速度环积分时间")] SpeedI = 0x06,
    [Description("位置环比例系数")] PositionP = 0x07,
    [Description("方向")] Dir = 0x09,
    [Description("电子齿轮分子")] ElectronGearMolecule = 0x0a,
    [Description("电子齿轮分母")] ElectronGearDenominator = 0x0b,
    [Description("报警代码")] ErrCode = 0x0e,
    [Description("系统电流")] Electricity = 0x0f,
    [Description("系统电压")] Voltage = 0x11,
    [Description("系统温度")] Temperature = 0x12,
    [Description("参数保存标志")] ParamsSave = 0x14,
    [Description("设备地址")] Address = 0x15,
    [Description("特殊功能")] Function = 0x19
}

public enum YzAimZeroingMode
{
    [Description("堵转模式")] LockedRotor = 0x04,
    [Description("传感器模式")] Sensor = 0x08
}

public class YzAimMaster
{
    private readonly IModbusMaster _master;
    private readonly IFreebusLogger _logger;

    internal YzAimMaster(IModbusMaster master, IFreebusLogger logger)
    {
        _master = master;
        _logger = logger;
    }

    private bool WriteRegister(byte id, ushort address, ushort value, string cmd)
    {
        try
        {
            _master.WriteSingleRegister(id, address, value);
            return true;
        }
        catch (Exception e)
        {
            _logger.Error($"{cmd}命令异常，{e.Message}");
            return false;
        }
    }

    private ushort? ReadRegister(byte id, ushort address, string cmd)
    {
        try
        {
            return _master.ReadHoldingRegisters(id, address, 1)[0];
        }
        catch (Exception e)
        {
            _logger.Error($"{cmd}命令异常，{e.Message}");
            return default;
        }
    }

    /// <summary>
    /// 获取电机参数
    /// </summary>
    /// <param name="id"></param>
    /// <param name="cmd"></param>
    /// <returns></returns>
    public ushort? GetYzAimStatusCmd(byte id, YzAimCmd cmd)
    {
        return ReadRegister(id, (ushort)cmd, cmd.EnumDescription());
    }

    /// <summary>
    /// 获取电机参数
    /// </summary>
    /// <param name="id"></param>
    /// <param name="cmd"></param>
    /// <returns></returns>
    public Task<ushort?> GetYzAimStatusCmdAsync(byte id, YzAimCmd cmd)
    {
        return Task.Run(() => GetYzAimStatusCmd(id, cmd));
    }

    /// <summary>
    /// 设置电机参数
    /// </summary>
    /// <param name="id"></param>
    /// <param name="cmd"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public bool SetYzAimStatusCmd(byte id, YzAimCmd cmd, ushort value)
    {
        ValidateSetCmd(cmd, value);
        return WriteRegister(id, (ushort)cmd, value, cmd.EnumDescription());
    }

    /// <summary>
    /// 设置电机参数
    /// </summary>
    /// <param name="id"></param>
    /// <param name="cmd"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public Task<bool> SetYzAimStatusCmdAsync(byte id, YzAimCmd cmd, ushort value)
    {
        return Task.Run(() => SetYzAimStatusCmd(id, cmd, value));
    }

    /// <summary>
    /// 设置电机位置
    /// </summary>
    /// <param name="id"></param>
    /// <param name="pos"></param>
    /// <returns></returns>
    public bool SetPosition(byte id, int pos)
    {
        var uPos = (uint)pos;
        try
        {
            _master.WriteSingleRegister32(id, 0x16, uPos);
            return true;
        }
        catch (Exception e)
        {
            _logger.Error($"设置电机位置命令异常，{e.Message}");
            return false;
        }
    }

    /// <summary>
    /// 设置电机位置
    /// </summary>
    /// <param name="id"></param>
    /// <param name="pos"></param>
    /// <returns></returns>
    public Task<bool> SetPositionAsync(byte id, int pos)
    {
        return Task.Run(() => SetPosition(id, pos));
    }

    /// <summary>
    /// 获取电机位置
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public int? GetPosition(byte id)
    {
        try
        {
            var uPos = _master.ReadHoldingRegisters32(id, 0x16, 1);
            return (int)uPos[0];
        }
        catch (Exception e)
        {
            _logger.Error($"读取电机位置命令异常，{e.Message}");
            return default;
        }
    }

    /// <summary>
    /// 获取电机位置
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public Task<int?> GetPositionAsync(byte id)
    {
        return Task.Run(() => GetPosition(id));
    }

    /// <summary>
    /// 取消电机自动回零
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public bool CancelAutoZeroing(byte id)
    {
        return WriteRegister(id, 0x19, 0, nameof(CancelAutoZeroing)) &&
               SetYzAimStatusCmd(id, YzAimCmd.ParamsSave, 1);
    }

    /// <summary>
    /// 取消电机自动回零
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public Task<bool> CancelAutoZeroingAsync(byte id)
    {
        return Task.Run(() => CancelAutoZeroing(id));
    }

    /// <summary>
    /// 电机回零
    /// </summary>
    /// <param name="id"></param>
    /// <param name="mode"></param>
    /// <returns></returns>
    public bool Zeroing(byte id, YzAimZeroingMode mode)
    {
        return WriteRegister(id, 0x19, (ushort)mode, nameof(CancelAutoZeroing));
    }

    /// <summary>
    /// 电机回零
    /// </summary>
    /// <param name="id"></param>
    /// <param name="mode"></param>
    /// <returns></returns>
    public Task<bool> ZeroingAsync(byte id, YzAimZeroingMode mode)
    {
        return Task.Run(() => Zeroing(id, mode));
    }

    /// <summary>
    /// 获取电机实际电流/A
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public double GetActualElectricity(byte id)
    {
        const double k = 2000.0;
        return (GetYzAimStatusCmd(id, YzAimCmd.Electricity) ?? 0) / k;
    }

    /// <summary>
    /// 获取电机实际电流/A
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public Task<double> GetActualElectricityAsync(byte id)
    {
        return Task.Run(() => GetActualElectricity(id));
    }

    /// <summary>
    /// 获取电机实际电压/V
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public double GetActualVoltage(byte id)
    {
        const double k = 327.0;
        return (GetYzAimStatusCmd(id, YzAimCmd.Voltage) ?? 0) / k;
    }

    /// <summary>
    /// 获取电机实际电压/V
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public Task<double> GetActualVoltageAsync(byte id)
    {
        return Task.Run(() => GetActualVoltage(id));
    }

    /// <summary>
    /// 获取电机实际转速r/min
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public double GetActualSpeed(byte id)
    {
        const double k = 10.0;
        var speed = (short)_master.ReadHoldingRegisters(id, 0x10, 1)[0];
        return speed / k;
    }

    /// <summary>
    /// 获取电机实际转速r/min
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public Task<double> GetActualSpeedAsync(byte id)
    {
        return Task.Run(() => GetActualSpeed(id));
    }

    /// <summary>
    /// 多组到位
    /// </summary>
    /// <param name="checkGroup"></param>
    /// <param name="offset"></param>
    /// <returns></returns>
    public bool AllReady(List<(byte id, int targetPosition)> checkGroup,
        int offset = 0)
    {
        var ready = true;
        foreach (var (id, targetPosition) in checkGroup)
        {
            var pos = GetPosition(id) ?? 0;
            if (Math.Abs(pos - targetPosition) > 20 + offset)
            {
                ready = false;
            }

            if (!ready) break;
        }

        return ready;
    }

    /// <summary>
    /// 多组到位
    /// </summary>
    /// <param name="checkGroup"></param>
    /// <param name="offset"></param>
    /// <returns></returns>
    public Task<bool> AllReadyAsync(List<(byte id, int targetPosition)> checkGroup,
        int offset = 0)
    {
        return Task.Run(() => AllReady(checkGroup, offset));
    }

    /// <summary>
    /// 广播位移
    /// </summary>
    /// <param name="motionParams"></param>
    public void WriteAllMotionParams(
        List<(int position, ushort speed, ushort acc)> motionParams)
    {
        var data = new byte[]
        {
            0x00,
            0x10,
            0x00,
            0x16
        };
        var lengthBytes = BitConverter.GetBytes((short)(motionParams.Count * 4))
            .Reverse().ToArray();
        data = data.Concat(lengthBytes).ToArray();
        data = data.Concat(new byte[]
        {
            0
        }).ToArray();

        foreach (var (position, speed, acc) in motionParams)
        {
            var positionBytes = BitConverter.GetBytes(position).ToArray();
            var speedBytes = BitConverter.GetBytes(speed).Reverse().ToArray();
            var accBytes = BitConverter.GetBytes(acc).Reverse().ToArray();
            data = data.Concat(new[]
            {
                positionBytes[1]
            }).Concat(new[]
            {
                positionBytes[0]
            }).Concat(new[]
            {
                positionBytes[3]
            }).Concat(new[]
            {
                positionBytes[2]
            }).Concat(speedBytes).Concat(accBytes).ToArray();
        }

        var crc = CoreTool.CalculateCrc(data).ToArray();
        data = data.Concat(crc).ToArray();

        _master.Transport.StreamResource.Write(data, 0, data.Length);
    }

    /// <summary>
    /// 修改电机Id
    /// </summary>
    /// <param name="id"></param>
    /// <param name="targetId"></param>
    public bool ModifyId(byte id, byte targetId)
    {
        try
        {
            _master.Transport.Write(new ModifyAddressRequestResponse(id, targetId,
                new RegisterCollection(0, targetId)));
            var buffer = new byte[2];
            _master.Transport.StreamResource.Read(buffer, 0, buffer.Length);

            return buffer[0] == targetId && buffer[1] == 0x7a;
        }
        catch (Exception e)
        {
            _logger.Error($"{nameof(ModifyId)}命令异常，{e.Message}");
        }

        return false;
    }

    /// <summary>
    /// 广播位移
    /// </summary>
    /// <param name="motionParams"></param>
    public Task WriteAllMotionParamsAsync(
        List<(int position, ushort speed, ushort acc)> motionParams)
    {
        return Task.Run(() => WriteAllMotionParams(motionParams));
    }

    private static void ValidateSetCmd(YzAimCmd cmd, ushort value)
    {
        switch (cmd)
        {
            case YzAimCmd.Dir:
            case YzAimCmd.ModbusEnable:
            case YzAimCmd.MotorEnable:
            case YzAimCmd.ParamsSave:
                if (value > 1)
                {
                    throw new ArgumentException("值只能是0或1", cmd.EnumDescription());
                }

                break;
            case YzAimCmd.TargetSpeed:
                if (value > 3000)
                {
                    throw new ArgumentException("值必须小于等于3000", cmd.EnumDescription());
                }

                break;
            case YzAimCmd.Acc:
                if (value > 60098)
                {
                    throw new ArgumentException("值必须小于等于60098", cmd.EnumDescription());
                }

                break;
            case YzAimCmd.WeakMagneticAngle:
                throw new ArgumentException("内部参数不需要另外设置", cmd.EnumDescription());
            case YzAimCmd.SpeedP:
                if (value > 10000)
                {
                    throw new ArgumentException("值必须小于等于10000", cmd.EnumDescription());
                }

                break;
            case YzAimCmd.SpeedI:
                if (value is > 2000 or < 2)
                {
                    throw new ArgumentException("值必须大于等于2小于等于2000",
                        cmd.EnumDescription());
                }

                break;
            case YzAimCmd.PositionP:
                if (value is > 30000 or < 60)
                {
                    throw new ArgumentException("值必须大于等于60小于等于30000",
                        cmd.EnumDescription());
                }

                break;
            case YzAimCmd.ElectronGearMolecule:
                break;
            case YzAimCmd.ElectronGearDenominator:
                if (value == 0)
                {
                    throw new ArgumentException("值必须大于0",
                        cmd.EnumDescription());
                }

                break;
            case YzAimCmd.Temperature:
            case YzAimCmd.Voltage:
            case YzAimCmd.Electricity:
            case YzAimCmd.ErrCode:
            case YzAimCmd.Address:
                throw new ArgumentException("只读参数", cmd.EnumDescription());
            case YzAimCmd.Function:
                if (value != 0 && value != 1 && value != 2 && value != 3 && value != 4 &&
                    value != 5 && value != 8)
                {
                    throw new ArgumentException("有效值是0,1,2,3,4,5,8",
                        cmd.EnumDescription());
                }

                break;
            default:
                throw new ArgumentOutOfRangeException(cmd.EnumDescription(), cmd, null);
        }
    }
}