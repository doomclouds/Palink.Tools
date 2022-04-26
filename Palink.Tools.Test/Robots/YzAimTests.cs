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
        var yzAmi = FreebusFactory.CreateYzAimMaster(factory, tcp);

        var id = yzAmi.GetYzAimStatusCmd<ushort>(situation, YzAimCmd.Address);
        var speed = yzAmi.GetYzAimStatusCmd<ushort>(situation, YzAimCmd.TargetSpeed);
        var acc = yzAmi.GetYzAimStatusCmd<ushort>(situation, YzAimCmd.Acc);
        var electricity = yzAmi.GetActualElectricity(situation);
        var electricity1 =
            yzAmi.GetYzAimStatusCmd<ushort>(situation, YzAimCmd.Electricity);
        var voltage = yzAmi.GetActualVoltage(situation);
        var voltage1 = yzAmi.GetYzAimStatusCmd<ushort>(situation, YzAimCmd.Voltage);
        var dir = yzAmi.GetYzAimStatusCmd<ushort>(situation, YzAimCmd.Dir);
        var errCode = yzAmi.GetYzAimStatusCmd<ushort>(situation, YzAimCmd.ErrCode);
        var temperature =
            yzAmi.GetYzAimStatusCmd<ushort>(situation, YzAimCmd.Temperature);

        yzAmi.SetYzAimStatusCmd(situation, YzAimCmd.ModbusEnable, 1);
        yzAmi.SetYzAimStatusCmd(situation, YzAimCmd.MotorEnable, 1);

        // yzAmi.SetPosition(2, -10000);
        yzAmi.WriteAllMotionParams(new List<(int position, ushort speed, ushort acc)>()
        {
            (-10000, mySpeed, myAcc)
        });
        var group = new List<(byte id, int pos)> { (situation, -10000) };
        Task.Delay(100).Wait();

        var cSpeed = 0.0;
        while (!yzAmi.AllReady(group))
        {
            var e = yzAmi.GetActualElectricity(situation);
            var v = yzAmi.GetActualVoltage(situation);
            var s = yzAmi.GetActualSpeed(situation);

            var e1 = yzAmi.GetYzAimStatusCmd<ushort>(situation, YzAimCmd.Electricity);
            var v1 = yzAmi.GetYzAimStatusCmd<ushort>(situation, YzAimCmd.Voltage);

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

        var pos = yzAmi.GetPosition(situation);

        // yzAmi.SetPosition(2, -100000);
        yzAmi.WriteAllMotionParams(new List<(int position, ushort speed, ushort acc)>()
        {
            (-100000, mySpeed, myAcc)
        });
        Task.Delay(100).Wait();
        group = new List<(byte id, int pos)> { (situation, -100000) };
        while (!yzAmi.AllReady(group))
        {
            var e = yzAmi.GetActualElectricity(situation);
            var v = yzAmi.GetActualVoltage(situation);
            var s = yzAmi.GetActualSpeed(situation);

            var e1 = yzAmi.GetYzAimStatusCmd<ushort>(situation, YzAimCmd.Electricity);
            var v1 = yzAmi.GetYzAimStatusCmd<ushort>(situation, YzAimCmd.Voltage);

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

        pos = yzAmi.GetPosition(situation);
    }
}