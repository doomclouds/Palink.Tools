using System;
using Palink.Tools.NModbus.Contracts;
using Palink.Tools.NModbus.Data;
using Palink.Tools.NModbus.Interfaces;

namespace Palink.Tools.NModbus.Message;

internal class
    WriteFileRecordResponse : AbstractModbusMessageWithData<FileRecordCollection>,
        IModbusMessage
{
    public WriteFileRecordResponse()
    {
    }

    public WriteFileRecordResponse(byte slaveAddress)
        : base(slaveAddress, ModbusFunctionCodes.WriteFileRecord)
    {
    }

    public WriteFileRecordResponse(byte slaveAddress, FileRecordCollection data)
        : base(slaveAddress, ModbusFunctionCodes.WriteFileRecord)
    {
        Data = data;
    }

    public override int MinimumFrameSize => 10;

    public byte? ByteCount
    {
        get => MessageImpl.ByteCount ?? default;
        set => MessageImpl.ByteCount = value;
    }

    protected override void InitializeUnique(byte[] frame)
    {
        if (frame.Length < frame[2])
        {
            throw new FormatException("Message frame does not contain enough bytes.");
        }

        ByteCount = frame[2];
        Data = new FileRecordCollection(frame);
    }

    public override string ToString()
    {
        var msg =
            $"Wrote {Data?.DataBytes?.Count} bytes for file {Data?.FileNumber} starting at address {Data?.StartingAddress}.";
        return msg;
    }
}