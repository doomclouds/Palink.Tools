using System.Diagnostics;
using System.IO;
using Palink.Tools.IO;
using Palink.Tools.Logging;
using Palink.Tools.NModbus.Interfaces;

namespace Palink.Tools.NModbus.IO;

internal abstract class ModbusSerialTransport : ModbusTransport, IModbusSerialTransport
{
    internal ModbusSerialTransport(IStreamResource streamResource,
        IModbusFactory modbusFactory, IFreebusLogger logger)
        : base(streamResource, modbusFactory, logger)
    {
        Debug.Assert(streamResource != null, "Argument streamResource cannot be null.");
    }

    /// <summary>
    /// Gets or sets a value indicating whether LRC/CRC frame checking is performed on messages.
    /// </summary>
    public bool CheckFrame { get; set; } = true;

    public void DiscardInBuffer()
    {
        StreamResource.DiscardInBuffer();
    }

    public override void Write(IModbusMessage message)
    {
        DiscardInBuffer();

        var frame = BuildMessageFrame(message);

        Logger.LogFrameTx(frame);

        StreamResource.Write(frame, 0, frame.Length);
    }

    public override IModbusMessage CreateResponse<T>(byte[] frame)
    {
        var response = base.CreateResponse<T>(frame);

        // compare checksum
        if (!CheckFrame || CheckSumsMatch(response, frame))
        {
            return response;
        }

        var msg =
            $"CheckSums failed to match {string.Join(", ", response.MessageFrame)} != {string.Join(", ", frame)}";
        Logger.Warning(msg);
        throw new IOException(msg);
    }

    public abstract void IgnoreResponse();

    public abstract bool CheckSumsMatch(IModbusMessage message, byte[] messageFrame);

    internal override void OnValidateResponse(IModbusMessage request,
        IModbusMessage response)
    {
        // no-op
    }
}