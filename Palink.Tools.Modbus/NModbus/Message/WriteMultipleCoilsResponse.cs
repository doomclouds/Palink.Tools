using System;
using System.Net;
using Palink.Tools.NModbus.Contracts;
using Palink.Tools.NModbus.Interfaces;

namespace Palink.Tools.NModbus.Message;

internal class WriteMultipleCoilsResponse : AbstractModbusMessage, IModbusMessage
{
    public WriteMultipleCoilsResponse()
    {
    }

    public WriteMultipleCoilsResponse(byte slaveAddress, ushort startAddress,
        ushort numberOfPoints)
        : base(slaveAddress, ModbusFunctionCodes.WriteMultipleCoils)
    {
        StartAddress = startAddress;
        NumberOfPoints = numberOfPoints;
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

    public override int MinimumFrameSize => 6;

    public override string ToString()
    {
        var msg = $"Wrote {NumberOfPoints} coils starting at address {StartAddress}.";
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