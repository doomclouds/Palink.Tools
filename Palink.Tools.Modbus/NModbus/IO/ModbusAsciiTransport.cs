using System.Diagnostics;
using System.IO;
using System.Text;
using Palink.Tools.IO;
using Palink.Tools.Logging;
using Palink.Tools.NModbus.Contracts;
using Palink.Tools.NModbus.Interfaces;
using Palink.Tools.Utility;

namespace Palink.Tools.NModbus.IO;

/// <summary>
///     Refined Abstraction - http://en.wikipedia.org/wiki/Bridge_Pattern
/// </summary>
internal class ModbusAsciiTransport : ModbusSerialTransport, IModbusAsciiTransport
{
    internal ModbusAsciiTransport(IStreamResource streamResource,
        IModbusFactory modbusFactory, IFreebusLogger logger)
        : base(streamResource, modbusFactory, logger)
    {
        Debug.Assert(streamResource != null, "Argument streamResource cannot be null.");
    }

    public override byte[] BuildMessageFrame(IModbusMessage message)
    {
        var msgFrame = message.MessageFrame;

        var msgFrameAscii = CoreTool.GetAsciiBytes(msgFrame);
        var lrcAscii = CoreTool.GetAsciiBytes(CoreTool.CalculateLrc(msgFrame));
        var nlAscii = Encoding.UTF8.GetBytes(Modbus.NewLine.ToCharArray());

        var frame =
            new MemoryStream(1 + msgFrameAscii.Length + lrcAscii.Length + nlAscii.Length);
        frame.WriteByte((byte)':');
        frame.Write(msgFrameAscii, 0, msgFrameAscii.Length);
        frame.Write(lrcAscii, 0, lrcAscii.Length);
        frame.Write(nlAscii, 0, nlAscii.Length);

        return frame.ToArray();
    }

    public override bool CheckSumsMatch(IModbusMessage message, byte[] messageFrame)
    {
        return CoreTool.CalculateLrc(message.MessageFrame) ==
            messageFrame[messageFrame.Length - 1];
    }

    public override byte[] ReadRequest()
    {
        return ReadRequestResponse();
    }

    public override IModbusMessage ReadResponse<T>()
    {
        return CreateResponse<T>(ReadRequestResponse());
    }

    internal byte[] ReadRequestResponse()
    {
        // read message frame, removing frame start ':'
        var frameHex = CoreTool.ReadLine(StreamResource, Modbus.NewLine).Substring(1);

        // convert hex to bytes
        var frame = CoreTool.HexToBytes(frameHex);
        Logger.Trace($"RX: {string.Join(", ", frame)}");

        if (frame.Length < 3)
        {
            throw new IOException("Premature end of stream, message truncated.");
        }

        return frame;
    }

    public override void IgnoreResponse()
    {
        ReadRequestResponse();
    }
}