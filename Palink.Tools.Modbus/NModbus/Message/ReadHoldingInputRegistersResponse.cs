using System;
using System.Linq;
using Palink.Tools.Extensions.ArrayExt;
using Palink.Tools.NModbus.Contracts;
using Palink.Tools.NModbus.Data;
using Palink.Tools.NModbus.Interfaces;

namespace Palink.Tools.NModbus.Message;

internal class
    ReadHoldingInputRegistersResponse : AbstractModbusMessageWithData<RegisterCollection>,
        IModbusMessage
{
    public ReadHoldingInputRegistersResponse()
    {
    }

    public ReadHoldingInputRegistersResponse(byte functionCode, byte slaveAddress,
        RegisterCollection data)
        : base(slaveAddress, functionCode)
    {
        if (data == null)
        {
            throw new ArgumentNullException(nameof(data));
        }

        ByteCount = data.ByteCount;
        Data = data;
    }

    public byte? ByteCount
    {
        get => MessageImpl.ByteCount ?? default;
        set => MessageImpl.ByteCount = value;
    }

    public override int MinimumFrameSize => 3;

    public override string ToString()
    {
        var msg =
            $"Read {Data?.Count} {(FunctionCode == ModbusFunctionCodes.ReadHoldingRegisters ? "holding" : "input")} registers.";
        return msg;
    }

    protected override void InitializeUnique(byte[] frame)
    {
        if (frame.Length < MinimumFrameSize + frame[2])
        {
            throw new FormatException("Message frame does not contain enough bytes.");
        }

        ByteCount = frame[2];
        Data = new RegisterCollection(frame.Slice(3, ByteCount.Value).ToArray());
    }
}