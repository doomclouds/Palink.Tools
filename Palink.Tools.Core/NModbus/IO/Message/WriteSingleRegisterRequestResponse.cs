﻿using System;
using System.IO;
using System.Linq;
using System.Net;
using Palink.Tools.NModbus.Contracts;
using Palink.Tools.NModbus.Data;
using Palink.Tools.NModbus.Interfaces;

namespace Palink.Tools.NModbus.IO.Message;

internal class
    WriteSingleRegisterRequestResponse :
        AbstractModbusMessageWithData<RegisterCollection>, IModbusRequest
{
    public WriteSingleRegisterRequestResponse()
    {
    }

    public WriteSingleRegisterRequestResponse(byte slaveAddress, ushort startAddress,
        ushort registerValue)
        : base(slaveAddress, ModbusFunctionCodes.WriteSingleRegister)
    {
        StartAddress = startAddress;
        Data = new RegisterCollection(registerValue);
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
            $"Write single holding register {Data?[0]} at address {StartAddress}.";
        return msg;
    }

    public void ValidateResponse(IModbusMessage response)
    {
        var typedResponse = (WriteSingleRegisterRequestResponse)response;

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
        Data = new RegisterCollection(
            (ushort)IPAddress.NetworkToHostOrder(BitConverter.ToInt16(frame, 4)));
    }
}