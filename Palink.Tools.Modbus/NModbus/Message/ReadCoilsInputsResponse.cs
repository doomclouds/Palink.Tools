using System;
using System.Linq;
using Palink.Tools.Extensions.ArrayExt;
using Palink.Tools.NModbus.Contracts;
using Palink.Tools.NModbus.Data;
using Palink.Tools.NModbus.Interfaces;

namespace Palink.Tools.NModbus.Message;

internal class ReadCoilsInputsResponse :
    AbstractModbusMessageWithData<DiscreteCollection>,
    IModbusMessage
{
    public ReadCoilsInputsResponse()
    {
    }

    public ReadCoilsInputsResponse(byte functionCode, byte slaveAddress, byte byteCount,
        DiscreteCollection data)
        : base(slaveAddress, functionCode)
    {
        ByteCount = byteCount;
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
            $"Read {Data?.Count} {(FunctionCode == ModbusFunctionCodes.ReadInputs ? "inputs" : "coils")} - {Data}.";
        return msg;
    }

    protected override void InitializeUnique(byte[] frame)
    {
        if (frame.Length < 3 + frame[2])
        {
            throw new FormatException(
                "Message frame data segment does not contain enough bytes.");
        }

        ByteCount = frame[2];

        if (!ByteCount.HasValue)
            throw new ArgumentNullException(nameof(ByteCount), "ByteCount dose not null");
        Data = new DiscreteCollection(frame.Slice(3, ByteCount.Value).ToArray());
    }
}