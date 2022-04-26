using System;
using System.Net;
using Palink.Tools.NModbus.Contracts;
using Palink.Tools.NModbus.Interfaces;

namespace Palink.Tools.NModbus.Message;

internal class WriteMultipleRegistersResponse : AbstractModbusMessage, IModbusMessage
{
    public WriteMultipleRegistersResponse()
    {
    }

    public WriteMultipleRegistersResponse(byte slaveAddress, ushort startAddress,
        ushort numberOfPoints)
        : base(slaveAddress, ModbusFunctionCodes.WriteMultipleRegisters)
    {
        StartAddress = startAddress;
        NumberOfPoints = numberOfPoints;
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

    public override int MinimumFrameSize => 6;

    public override string ToString()
    {
        var msg =
            $"Wrote {NumberOfPoints} holding registers starting at address {StartAddress}.";
        return msg;
    }

    protected override void InitializeUnique(byte[] frame)
    {
        StartAddress =
            (ushort)IPAddress.NetworkToHostOrder(BitConverter.ToInt16(frame, 2));
        NumberOfPoints =
            (ushort)IPAddress.NetworkToHostOrder(BitConverter.ToInt16(frame, 4));
    }
}