using System;
using System.IO;
using System.Linq;
using System.Net;
using Palink.Tools.Extensions.ArrayExt;
using Palink.Tools.NModbus.Contracts;
using Palink.Tools.NModbus.Data;
using Palink.Tools.NModbus.Interfaces;

namespace Palink.Tools.NModbus.Message;

internal class
    WriteSingleCoilRequestResponse : AbstractModbusMessageWithData<RegisterCollection>,
        IModbusRequest
{
    public WriteSingleCoilRequestResponse()
    {
    }

    public WriteSingleCoilRequestResponse(byte slaveAddress, ushort startAddress,
        bool coilState)
        : base(slaveAddress, ModbusFunctionCodes.WriteSingleCoil)
    {
        StartAddress = startAddress;
        Data = new RegisterCollection(coilState ? Modbus.CoilOn : Modbus.CoilOff);
    }

    public override int MinimumFrameSize => 6;

    public ushort? StartAddress
    {
        get => MessageImpl.StartAddress ?? default;
        set => MessageImpl.StartAddress = value;
    }

    public override string ToString()
    {
        var msg =
            $"Write single coil {(Data?.First() == Modbus.CoilOn ? 1 : 0)} at address {StartAddress}.";
        return msg;
    }

    public void ValidateResponse(IModbusMessage response)
    {
        var typedResponse = (WriteSingleCoilRequestResponse)response;

        if (StartAddress != typedResponse.StartAddress)
        {
            var msg =
                $"Unexpected start address in response. Expected {StartAddress}, received {typedResponse.StartAddress}.";
            throw new IOException(msg);
        }

        if (Data?.First() != typedResponse.Data?.First())
        {
            var msg =
                $"Unexpected data in response. Expected {Data?.First()}, received {typedResponse.Data?.First()}.";
            throw new IOException(msg);
        }
    }

    protected override void InitializeUnique(byte[] frame)
    {
        StartAddress =
            (ushort)IPAddress.NetworkToHostOrder(BitConverter.ToInt16(frame, 2));
        Data = new RegisterCollection(frame.Slice(4, 2).ToArray());
    }
}