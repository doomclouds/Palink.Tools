using System;
using System.IO;
using Palink.Tools.NModbus.Contracts;
using Palink.Tools.NModbus.Data;
using Palink.Tools.NModbus.Interfaces;

namespace Palink.Tools.NModbus.Message;

internal class
    WriteFileRecordRequest : AbstractModbusMessageWithData<FileRecordCollection>,
        IModbusRequest
{
    public WriteFileRecordRequest()
    {
    }

    public WriteFileRecordRequest(byte slaveAddress, FileRecordCollection data)
        : base(slaveAddress, ModbusFunctionCodes.WriteFileRecord)
    {
        Data = data;
        ByteCount = data.ByteCount;
    }

    public override int MinimumFrameSize => 10;

    public byte? ByteCount
    {
        get => MessageImpl.ByteCount ?? default;
        set => MessageImpl.ByteCount = value;
    }

    public void ValidateResponse(IModbusMessage response)
    {
        var typedResponse = (WriteFileRecordResponse)response;

        if (Data?.FileNumber != typedResponse.Data?.FileNumber)
        {
            var msg =
                $"Unexpected file number in response. Expected {Data?.FileNumber}, received {typedResponse.Data?.FileNumber}.";
            throw new IOException(msg);
        }

        if (Data?.StartingAddress != typedResponse.Data?.StartingAddress)
        {
            var msg =
                $"Unexpected starting address in response. Expected {Data?.StartingAddress}, received {typedResponse.Data?.StartingAddress}.";
            throw new IOException(msg);
        }
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
            $"Write {Data?.DataBytes?.Count} bytes for file {Data?.FileNumber} starting at address {Data?.StartingAddress}.";
        return msg;
    }
}