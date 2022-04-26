using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading.Tasks;
using Palink.Tools.Freebus;
using Palink.Tools.Logging;
using Palink.Tools.NModbus;
using Palink.Tools.Robots.YzAim;
using Xunit;

namespace Palink.Tools.Test.Robots;

public class YzAimTests
{
    [Fact]
    public void FunctionTests()
    {
        const byte situation = 1;
        const ushort myAcc = 15000;
        const ushort mySpeed = 1500;
        var tcp = new Socket(SocketType.Stream, ProtocolType.Tcp);
        tcp.Connect("192.168.0.8", 503);
        tcp.ReceiveTimeout = 500;
        tcp.SendTimeout = 500;
        var factory = new ModbusFactory(logger: new DebugFreebusLogger());
        var yzAim = FreebusFactory.CreateYzAimMaster(factory, tcp);

        var id = yzAim.GetYzAimStatusCmd<ushort>(situation, YzAimCmd.Address);
        var speed = yzAim.GetYzAimStatusCmd<ushort>(situation, YzAimCmd.TargetSpeed);
        var acc = yzAim.GetYzAimStatusCmd<ushort>(situation, YzAimCmd.Acc);
        var electricity = yzAim.GetActualElectricity(situation);
        var electricity1 =
            yzAim.GetYzAimStatusCmd<ushort>(situation, YzAimCmd.Electricity);
        var voltage = yzAim.GetActualVoltage(situation);
        var voltage1 = yzAim.GetYzAimStatusCmd<ushort>(situation, YzAimCmd.Voltage);
        var dir = yzAim.GetYzAimStatusCmd<ushort>(situation, YzAimCmd.Dir);
        var errCode = yzAim.GetYzAimStatusCmd<ushort>(situation, YzAimCmd.ErrCode);
        var temperature =
            yzAim.GetYzAimStatusCmd<ushort>(situation, YzAimCmd.Temperature);

        yzAim.SetYzAimStatusCmd(situation, YzAimCmd.ModbusEnable, 1);
        yzAim.SetYzAimStatusCmd(situation, YzAimCmd.MotorEnable, 1);

        // yzAmi.SetPosition(2, -10000);
        yzAim.WriteAllMotionParams(new List<(int position, ushort speed, ushort acc)>()
        {
            (-10000, mySpeed, myAcc)
        });
        var group = new List<(byte id, int pos)> { (situation, -10000) };
        Task.Delay(100).Wait();

        var cSpeed = 0.0;
        while (!yzAim.AllReady(group))
        {
            var e = yzAim.GetActualElectricity(situation);
            var v = yzAim.GetActualVoltage(situation);
            var s = yzAim.GetActualSpeed(situation);

            var e1 = yzAim.GetYzAimStatusCmd<ushort>(situation, YzAimCmd.Electricity);
            var v1 = yzAim.GetYzAimStatusCmd<ushort>(situation, YzAimCmd.Voltage);

            if (e > electricity)
                electricity = e;
            if (v > voltage)
                voltage = v;
            if (e1 > electricity1)
                electricity1 = e1;
            if (v1 > voltage1)
                voltage1 = v1;
            if (Math.Abs(s) > Math.Abs(cSpeed))
                cSpeed = s;

            Task.Delay(100).Wait();
        }

        var pos = yzAim.GetPosition(situation);

        // yzAmi.SetPosition(2, -100000);
        yzAim.WriteAllMotionParams(new List<(int position, ushort speed, ushort acc)>()
        {
            (-100000, mySpeed, myAcc)
        });
        Task.Delay(100).Wait();
        group = new List<(byte id, int pos)> { (situation, -100000) };
        while (!yzAim.AllReady(group))
        {
            var e = yzAim.GetActualElectricity(situation);
            var v = yzAim.GetActualVoltage(situation);
            var s = yzAim.GetActualSpeed(situation);

            var e1 = yzAim.GetYzAimStatusCmd<ushort>(situation, YzAimCmd.Electricity);
            var v1 = yzAim.GetYzAimStatusCmd<ushort>(situation, YzAimCmd.Voltage);

            if (e > electricity)
                electricity = e;
            if (v > voltage)
                voltage = v;
            if (e1 > electricity1)
                electricity1 = e1;
            if (v1 > voltage1)
                voltage1 = v1;
            if (Math.Abs(s) > Math.Abs(cSpeed))
                cSpeed = s;
        }

        pos = yzAim.GetPosition(situation);
    }
}