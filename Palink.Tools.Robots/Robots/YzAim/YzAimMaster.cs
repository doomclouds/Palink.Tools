using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Palink.Tools.Extensions.AttributeExt;
using Palink.Tools.Freebus.Device;
using Palink.Tools.Freebus.Interface;
using Palink.Tools.Freebus.Message;
using Palink.Tools.Logging;
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

public class YzAimMaster : FreebusMaster
{
    internal YzAimMaster(IFreebusTransport transport) : base(transport)
    {
    }

    private bool WriteRegister(byte id, ushort address, ushort value, string cmd)
    {
        var context = new FreebusContext
        {
            Pdu = new byte[]
            {
                id,
                0x06
            },
            DruLength = 8,
        };

        var startAddressBytes =
            BitConverter.GetBytes(address).Reverse().ToArray();
        context.Pdu = context.Pdu.Concat(startAddressBytes).ToArray();
        var numOfPointBytes = BitConverter.GetBytes(value).Reverse().ToArray();
        context.Pdu = context.Pdu.Concat(numOfPointBytes).ToArray();
        var crc = CoreTool.CalculateCrc(context.Pdu).ToArray();
        context.Pdu = context.Pdu.Concat(crc).ToArray();

        try
        {
            ExecuteCustomMessage(context);
            return true;
        }
        catch (Exception e)
        {
            Transport.Logger.Error($"{cmd}命令异常，{e.Message}");
            return false;
        }
    }

    private bool WriteRegister(byte id, ushort address, byte function, ushort value,
        string cmd)
    {
        var context = new FreebusContext
        {
            Pdu = new[]
            {
                id,
                function
            },
            DruLength = 2,
        };

        var startAddressBytes =
            BitConverter.GetBytes(address).Reverse().ToArray();
        context.Pdu = context.Pdu.Concat(startAddressBytes).ToArray();
        var numOfPointBytes = BitConverter.GetBytes(value).Reverse().ToArray();
        context.Pdu = context.Pdu.Concat(numOfPointBytes).ToArray();
        var crc = CoreTool.CalculateCrc(context.Pdu).ToArray();
        context.Pdu = context.Pdu.Concat(crc).ToArray();

        try
        {
            ExecuteCustomMessage(context);
            return true;
        }
        catch (Exception e)
        {
            Transport.Logger.Error($"{cmd}命令异常，{e.Message}");
            return false;
        }
    }

    private bool WritePluse(byte situation, int value, string cmd)
    {
        var context = new FreebusContext()
        {
            Pdu = new byte[]
            {
                situation,
                0x10
            },
            DruLength = 8
        };

        var startAddressBytes =
            BitConverter.GetBytes((short)0x016).Reverse().ToArray();
        context.Pdu = context.Pdu.Concat(startAddressBytes).ToArray();
        var numOfPointBytes = BitConverter.GetBytes((short)2).Reverse().ToArray();
        context.Pdu = context.Pdu.Concat(numOfPointBytes).ToArray();
        context.Pdu = context.Pdu.Concat(new byte[]
        {
            4
        }).ToArray();

        var valueBytes = BitConverter.GetBytes(value).ToArray();

        context.Pdu = context.Pdu.Concat(new[]
        {
            valueBytes[1]
        }).ToArray();
        context.Pdu = context.Pdu.Concat(new[]
        {
            valueBytes[0]
        }).ToArray();
        context.Pdu = context.Pdu.Concat(new[]
        {
            valueBytes[3]
        }).ToArray();
        context.Pdu = context.Pdu.Concat(new[]
        {
            valueBytes[2]
        }).ToArray();


        var crc = CoreTool.CalculateCrc(context.Pdu).ToArray();
        context.Pdu = context.Pdu.Concat(crc).ToArray();

        try
        {
            ExecuteCustomMessage(context);
            return true;
        }
        catch (Exception e)
        {
            Transport.Logger.Error($"{cmd}命令异常，{e.Message}");
            return false;
        }
    }

    private ushort? ReadRegister(byte id, ushort address, string cmd)
    {
        var context = new FreebusContext()
        {
            Pdu = new byte[]
            {
                id,
                0x03
            },
            DruLength = 7,
            Dru = new byte[7]
        };

        var startAddressBytes =
            BitConverter.GetBytes(address).Reverse().ToArray();
        context.Pdu = context.Pdu.Concat(startAddressBytes).ToArray();
        var numOfPointBytes = BitConverter.GetBytes((short)1).Reverse().ToArray();
        context.Pdu = context.Pdu.Concat(numOfPointBytes).ToArray();
        var crc = CoreTool.CalculateCrc(context.Pdu).ToArray();
        context.Pdu = context.Pdu.Concat(crc).ToArray();

        try
        {
            ExecuteCustomMessage(context);
            var result = new byte[2];
            Buffer.BlockCopy(context.Dru, 3, result, 0, context.Dru[2]);
            return BitConverter.ToUInt16(result.Reverse().ToArray(), 0);
        }
        catch (Exception e)
        {
            Transport.Logger.Error($"{cmd}命令异常，{e.Message}");
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
    /// 设置电机位置
    /// </summary>
    /// <param name="id"></param>
    /// <param name="pos"></param>
    /// <returns></returns>
    public bool SetPosition(byte id, int pos)
    {
        try
        {
            var ret = WritePluse(id, pos, nameof(SetPosition));
            return ret;
        }
        catch (Exception e)
        {
            Transport.Logger.Error($"设置电机位置命令异常，{e.Message}");
            return false;
        }
    }

    /// <summary>
    /// 获取电机位置
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public int GetPosition(byte id)
    {
        try
        {
            var us1 = ReadRegister(id, 0x16, nameof(GetPosition)) ?? 0;
            var us2 = ReadRegister(id, 0x17, nameof(GetPosition)) ?? 0;
            return (int)CoreTool.GetUInt32(us2, us1);
        }
        catch (Exception e)
        {
            Transport.Logger.Error($"读取电机位置命令异常，{e.Message}");
            return default;
        }
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
    /// 获取电机实际转速r/min
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public double GetActualSpeed(byte id)
    {
        const double k = 10.0;
        var speed = (short)(ReadRegister(id, 0x10, nameof(GetActualSpeed)) ?? 0);
        return speed / k;
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
            var pos = GetPosition(id);
            if (Math.Abs(pos - targetPosition) > 20 + offset)
            {
                ready = false;
            }

            if (!ready) break;
        }

        return ready;
    }

    /// <summary>
    /// 广播位移
    /// </summary>
    /// <param name="motionParams"></param>
    /// <param name="shouldLog"></param>
    public void WriteAllMotionParams(
        List<(int position, ushort speed, ushort acc)> motionParams,
        bool shouldLog = false)
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

        BroadcastMessage(new FreebusContext
        {
            Pdu = data
        }, shouldLog);
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
            var ret = WriteRegister(id, 0, 0x7a, targetId, nameof(ModifyId));

            return ret;
        }
        catch (Exception e)
        {
            Transport.Logger.Error($"{nameof(ModifyId)}命令异常，{e.Message}");
        }

        return false;
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