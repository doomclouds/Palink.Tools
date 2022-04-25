#nullable enable
using System;
using System.Net.Sockets;
using Palink.Tools.Freebus;
using Palink.Tools.Freebus.Interface;
using Palink.Tools.Freebus.Message;
using Palink.Tools.IO;
using Palink.Tools.Logging;
using Xunit;

namespace Palink.Tools.Test.Communication;

public class MasterTest
{
    [Fact]
    public void MyMasterTest()
    {
        //tcp发送数据
        var tcp = new TcpClient();
        tcp.Connect("127.0.0.1", 9000);
        var adapter = new TcpClientAdapter(tcp);
        var master = new MyMaster(adapter, new ConsoleFreebusLogger(LoggingLevel.Debug));
        master.TestCmd();

        //udp发送数据
        var udp = new UdpClient();
        udp.Connect("127.0.0.1", 9000);
        var udpAdapter = new UdpClientOverCOMAdapter(udp);
        var udpMaster = new MyMaster(udpAdapter, new ConsoleFreebusLogger(LoggingLevel.Debug));
        udpMaster.TestCmd();
    }
}

public class MyMaster : Master
{
    /// <summary>
    /// Master
    /// </summary>
    /// <param name="streamResource"></param>
    /// <param name="logger"></param>
    public MyMaster(IStreamResource streamResource, IFreebusLogger logger) : base(
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
        return message.Buffer.Length == 9;
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
    public override IMessage? CreateStringFrame(string frame)
    {
        return null;
    }

    /// <summary>
    /// 测试命令
    /// </summary>
    public void TestCmd()
    {
        var message = new BaseMessage()
        {
            Data = new byte[]
            {
                0xa5,
                0x5a,
                01,
                02,
                03
            },
            SendBytes = 5,
            ReadBytes = 9
        };

        try
        {
            //单播命令，并等待返回9个字节的数据
            Unicast(message);

            //广播命令，无需等待返回
            // SendData(message);
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }
    }
}