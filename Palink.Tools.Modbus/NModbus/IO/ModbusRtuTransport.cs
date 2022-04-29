using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Palink.Tools.IO;
using Palink.Tools.Logging;
using Palink.Tools.NModbus.Contracts;
using Palink.Tools.NModbus.Extensions;
using Palink.Tools.NModbus.Interfaces;
using Palink.Tools.Utility;

namespace Palink.Tools.NModbus.IO;

internal class ModbusRtuTransport : ModbusSerialTransport, IModbusRtuTransport
{
    public const int RequestFrameStartLength = 7;

    public const int ResponseFrameStartLength = 4;

    internal ModbusRtuTransport(IStreamResource streamResource,
        IModbusFactory modbusFactory, IFreebusLogger logger)
        : base(streamResource, modbusFactory, logger)
    {
        if (modbusFactory == null) throw new ArgumentNullException(nameof(modbusFactory));
        Debug.Assert(streamResource != null, "Argument streamResource cannot be null.");
    }

    internal int RequestBytesToRead(byte[] frameStart)
    {
        var functionCode = frameStart[1];

        var service =
            ModbusFactory.GetFunctionServiceOrThrow(functionCode);

        return service.GetRtuRequestBytesToRead(frameStart);
    }

    internal int ResponseBytesToRead(byte[] frameStart)
    {
        var functionCode = frameStart[1];

        if (functionCode > Modbus.ExceptionOffset)
        {
            return 1;
        }

        var service =
            ModbusFactory.GetFunctionServiceOrThrow(functionCode);

        return service.GetRtuResponseBytesToRead(frameStart);
    }

    public virtual byte[] Read(int count)
    {
        var frameBytes = new byte[count];
        var numBytesRead = 0;

        while (numBytesRead != count)
        {
            numBytesRead +=
                StreamResource.Read(frameBytes, numBytesRead, count - numBytesRead);
        }

        return frameBytes;
    }

    public override byte[] BuildMessageFrame(IModbusMessage message)
    {
        var messageFrame = message.MessageFrame;
        var crc = CoreTool.CalculateCrc(messageFrame);
        var messageBody = new MemoryStream(messageFrame.Length + crc.Length);

        messageBody.Write(messageFrame, 0, messageFrame.Length);
        messageBody.Write(crc, 0, crc.Length);

        return messageBody.ToArray();
    }

    public override bool CheckSumsMatch(IModbusMessage message, byte[] messageFrame)
    {
        var messageCrc = BitConverter.ToUInt16(messageFrame, messageFrame.Length - 2);
        var calculatedCrc =
            BitConverter.ToUInt16(CoreTool.CalculateCrc(message.MessageFrame), 0);

        return messageCrc == calculatedCrc;
    }

    public override IModbusMessage ReadResponse<T>()
    {
        var frame = ReadResponse();

        Logger.LogFrameRx(frame);

        return CreateResponse<T>(frame);
    }

    private byte[] ReadResponse()
    {
        var frameStart = Read(ResponseFrameStartLength);
        var frameEnd = Read(ResponseBytesToRead(frameStart));
        var frame = frameStart.Concat(frameEnd).ToArray();

        return frame;
    }

    public override void IgnoreResponse()
    {
        var frame = ReadResponse();

        Logger.LogFrameIgnoreRx(frame);
    }

    public override byte[] ReadRequest()
    {
        var frameStart = Read(RequestFrameStartLength);
        var frameEnd = Read(RequestBytesToRead(frameStart));
        var frame = frameStart.Concat(frameEnd).ToArray();

        Logger.LogFrameRx(frame);

        return frame;
    }
}