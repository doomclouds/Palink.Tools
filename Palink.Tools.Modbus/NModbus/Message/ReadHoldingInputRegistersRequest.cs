using System;
using System.IO;
using System.Net;
using Palink.Tools.NModbus.Contracts;
using Palink.Tools.NModbus.Interfaces;

namespace Palink.Tools.NModbus.Message;

internal class ReadHoldingInputRegistersRequest : AbstractModbusMessage, IModbusRequest
{
    public ReadHoldingInputRegistersRequest()
    {
    }

    public ReadHoldingInputRegistersRequest(byte functionCode, byte slaveAddress,
        ushort startAddress, ushort numberOfPoints)
        : base(slaveAddress, functionCode)
    {
        StartAddress = startAddress;
        NumberOfPoints = numberOfPoints;
    }

    public ushort? StartAddress
    {
        get => MessageImpl.StartAddress ?? default;
        set => MessageImpl.StartAddress = value;
    }

    public override int MinimumFrameSize => 6;

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

    public override string ToString()
    {
        var msg =
            $"Read {NumberOfPoints} {(FunctionCode == ModbusFunctionCodes.ReadHoldingRegisters ? "holding" : "input")} " +
            $"registers starting at address {StartAddress}.";
        return msg;
    }

    public void ValidateResponse(IModbusMessage response)
    {
        var typedResponse = response as ReadHoldingInputRegistersResponse;

        var expectedByteCount = NumberOfPoints * 2;

        if (expectedByteCount != typedResponse?.ByteCount)
        {
            var msg =
                $"Unexpected byte count. Expected {expectedByteCount}, received {typedResponse?.ByteCount}.";
            throw new IOException(msg);
        }
    }

    protected override void InitializeUnique(byte[] frame)
    {
        StartAddress =
            (ushort)IPAddress.NetworkToHostOrder(BitConverter.ToInt16(frame, 2));
        NumberOfPoints =
            (ushort)IPAddress.NetworkToHostOrder(BitConverter.ToInt16(frame, 4));
    }
}