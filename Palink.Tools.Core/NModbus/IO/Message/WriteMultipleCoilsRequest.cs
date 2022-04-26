﻿using System;
using System.IO;
using System.Linq;
using System.Net;
using Palink.Tools.Extensions.PLArray;
using Palink.Tools.NModbus.Contracts;
using Palink.Tools.NModbus.Data;
using Palink.Tools.NModbus.Interfaces;

namespace Palink.Tools.NModbus.IO.Message;

/// <summary>
///     Write Multiple Coils request.
/// </summary>
internal class
    WriteMultipleCoilsRequest : AbstractModbusMessageWithData<DiscreteCollection>,
        IModbusRequest
{
    /// <summary>
    ///     Write Multiple Coils request.
    /// </summary>
    public WriteMultipleCoilsRequest()
    {
    }

    /// <summary>
    ///     Write Multiple Coils request.
    /// </summary>
    public WriteMultipleCoilsRequest(byte slaveAddress, ushort startAddress,
        DiscreteCollection data)
        : base(slaveAddress, ModbusFunctionCodes.WriteMultipleCoils)
    {
        StartAddress = startAddress;
        NumberOfPoints = (ushort)data.Count;
        ByteCount = (byte)((data.Count + 7) / 8);
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
            if (value > Modbus.MaximumDiscreteRequestResponseSize)
            {
                var msg =
                    $"Maximum amount of data {Modbus.MaximumDiscreteRequestResponseSize} coils.";
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
        var msg = $"Write {NumberOfPoints} coils starting at address {StartAddress}.";
        return msg;
    }

    public void ValidateResponse(IModbusMessage response)
    {
        var typedResponse = (WriteMultipleCoilsResponse)response;

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
        Data = new DiscreteCollection(frame.Slice(7, ByteCount.Value).ToArray());
    }
}