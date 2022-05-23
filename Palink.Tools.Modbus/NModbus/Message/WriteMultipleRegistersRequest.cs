﻿using System;
using System.IO;
using System.Linq;
using System.Net;
using Palink.Tools.Extensions.ArrayExt;
using Palink.Tools.NModbus.Contracts;
using Palink.Tools.NModbus.Data;
using Palink.Tools.NModbus.Interfaces;

namespace Palink.Tools.NModbus.Message;

internal class
    WriteMultipleRegistersRequest : AbstractModbusMessageWithData<RegisterCollection>,
        IModbusRequest
{
    public WriteMultipleRegistersRequest()
    {
    }

    public WriteMultipleRegistersRequest(byte slaveAddress, ushort startAddress,
        RegisterCollection data)
        : base(slaveAddress, ModbusFunctionCodes.WriteMultipleRegisters)
    {
        StartAddress = startAddress;
        NumberOfPoints = (ushort)data.Count;
        ByteCount = (byte)(data.Count * 2);
        Data = data;
    }

    public byte? ByteCount
    {
        get => MessageImpl.ByteCount ?? default;
        set => MessageImpl.ByteCount = value;
    }

    public ushort? NumberOfPoints
    {
        get => MessageImpl.NumberOfPoints ?? default;

        set
        {
            if (value > Modbus.MaximumRegisterRequestResponseSize)
            {
                var msg =
                    $"Maximum amount of data {Modbus.MaximumRegisterRequestResponseSize} registers.";
                throw new ArgumentOutOfRangeException(nameof(NumberOfPoints), msg);
            }

            MessageImpl.NumberOfPoints = value;
        }
    }

    public ushort? StartAddress
    {
        get => MessageImpl.StartAddress ?? default;
        set => MessageImpl.StartAddress = value;
    }

    public override int MinimumFrameSize => 7;

    public override string ToString()
    {
        var msg =
            $"Write {NumberOfPoints} holding registers starting at address {StartAddress}.";
        return msg;
    }

    public void ValidateResponse(IModbusMessage response)
    {
        var typedResponse = (WriteMultipleRegistersResponse)response;

        if (StartAddress != typedResponse.StartAddress)
        {
            var msg =
                $"Unexpected start address in response. Expected {StartAddress}, received {typedResponse.StartAddress}.";
            throw new IOException(msg);
        }

        if (NumberOfPoints != typedResponse.NumberOfPoints)
        {
            var msg =
                $"Unexpected number of points in response. Expected {NumberOfPoints}, received {typedResponse.NumberOfPoints}.";
            throw new IOException(msg);
        }
    }

    protected override void InitializeUnique(byte[] frame)
    {
        if (frame.Length < MinimumFrameSize + frame[6])
        {
            throw new FormatException("Message frame does not contain enough bytes.");
        }

        StartAddress =
            (ushort)IPAddress.NetworkToHostOrder(BitConverter.ToInt16(frame, 2));
        NumberOfPoints =
            (ushort)IPAddress.NetworkToHostOrder(BitConverter.ToInt16(frame, 4));
        ByteCount = frame[6];
        Data = new RegisterCollection(frame.Slice(7, ByteCount.Value).ToArray());
    }
}