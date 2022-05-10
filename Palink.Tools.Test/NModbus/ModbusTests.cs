using System.IO.Ports;
using System.Net.Sockets;
using Palink.Tools.IO;
using Palink.Tools.Logging;
using Palink.Tools.NModbus;
using Palink.Tools.NModbus.Extensions;
using Xunit;

namespace Palink.Tools.Test.NModbus;

public class ModbusTests
{
    [Fact]
    public void ModbusRtuReadWriteCoilsOverSerialPortTest()
    {
        var serialPort = new SerialPort("COM2");
        serialPort.Open();
        var adapter = new SerialPortAdapter(serialPort)
        {
            ReadTimeout = 500,
            WriteTimeout = 500
        };
        var master = (new ModbusFactory()).CreateRtuMaster(adapter);
        master.WriteMultipleCoils(1, 0, new[]
        {
            true, true, false, false, false,
            true, true, false, false, false,
        });
        var ret = master.ReadCoils(1, 0, 10);
        Assert.True(ret[0]);
        Assert.True(ret[1]);
        Assert.True(ret[5]);
        Assert.True(ret[6]);
    }

    [Fact]
    public void ModbusRtuReadWriteRegistersOverSerialPortTest()
    {
        var serialPort = new SerialPort("COM2");
        serialPort.Open();
        var adapter = new SerialPortAdapter(serialPort)
        {
            ReadTimeout = 500,
            WriteTimeout = 500
        };
        var master = (new ModbusFactory()).CreateRtuMaster(adapter);
        master.WriteMultipleRegisters(1, 0, new ushort[]
        {
            1, 2, 3, 4, 5,
            6, 7, 8, 9, 10,
        });
        var ret = master.ReadHoldingRegisters(1, 0, 10);
        Assert.True(ret[0] == 1);
        Assert.True(ret[1] == 2);
        Assert.True(ret[5] == 6);
        Assert.True(ret[6] == 7);
    }

    [Fact]
    public void ModbusRtuReadWriteCoilsOverTcpTest()
    {
        var tcp = new Socket(SocketType.Stream, ProtocolType.Tcp);
        tcp.Connect("127.0.0.1", 502);
        var adapter = new SocketAdapter(tcp)
        {
            ReadTimeout = 500,
            WriteTimeout = 500
        };
        var master = (new ModbusFactory()).CreateRtuMaster(adapter);
        master.WriteMultipleCoils(1, 0, new[]
        {
            true, true, false, false, false,
            true, true, false, false, false,
        });
        var ret = master.ReadCoils(1, 0, 10);
        Assert.True(ret[0]);
        Assert.True(ret[1]);
        Assert.True(ret[5]);
        Assert.True(ret[6]);
    }

    [Fact]
    public void ModbusRtuReadWriteRegistersOverTcpTest()
    {
        var tcp = new Socket(SocketType.Stream, ProtocolType.Tcp);
        tcp.Connect("127.0.0.1", 502);
        var adapter = new SocketAdapter(tcp)
        {
            ReadTimeout = 500,
            WriteTimeout = 500
        };
        var master = (new ModbusFactory()).CreateRtuMaster(adapter);
        master.WriteMultipleRegisters(1, 0, new ushort[]
        {
            1, 2, 3, 4, 5,
            6, 7, 8, 9, 10,
        });
        var ret = master.ReadHoldingRegisters(1, 0, 10);
        Assert.True(ret[0] == 1);
        Assert.True(ret[1] == 2);
        Assert.True(ret[5] == 6);
        Assert.True(ret[6] == 7);
    }

    [Fact]
    public void ModbusRtuReadWriteCoilsOverUdpTest()
    {
        var udp = new UdpClient(20000);
        udp.Connect("192.168.0.7", 20000);
        var adapter = new UdpClientOverCOMAdapter(udp)
        {
            ReadTimeout = 500,
            WriteTimeout = 500
        };
        var factory = new ModbusFactory(logger: NullFreebusLogger.Instance);
        var master = factory.CreateRtuMaster(adapter);

        var coils = master.ReadCoils(1, 5, 1);
    }
}