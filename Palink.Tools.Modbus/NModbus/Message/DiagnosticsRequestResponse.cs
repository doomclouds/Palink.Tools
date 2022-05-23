using System;
using System.Linq;
using System.Net;
using Palink.Tools.Extensions.ArrayExt;
using Palink.Tools.NModbus.Contracts;
using Palink.Tools.NModbus.Data;
using Palink.Tools.NModbus.Interfaces;

namespace Palink.Tools.NModbus.Message;

internal class
    DiagnosticsRequestResponse : AbstractModbusMessageWithData<RegisterCollection>,
        IModbusMessage
{
    public DiagnosticsRequestResponse()
    {
    }

    public DiagnosticsRequestResponse(ushort subFunctionCode, byte slaveAddress,
        RegisterCollection data)
        : base(slaveAddress, ModbusFunctionCodes.Diagnostics)
    {
        SubFunctionCode = subFunctionCode;
        Data = data;
    }

    public override int MinimumFrameSize => 6;

    public ushort? SubFunctionCode
    {
        get => MessageImpl.SubFunctionCode ?? default;
        set => MessageImpl.SubFunctionCode = value;
    }

    public override string ToString()
    {
        return $"Diagnostics message, sub-function return query data - {Data}.";
    }

    protected override void InitializeUnique(byte[] frame)
    {
        SubFunctionCode =
            (ushort)IPAddress.NetworkToHostOrder(BitConverter.ToInt16(frame, 2));
        Data = new RegisterCollection(frame.Slice(4, 2).ToArray());
    }
}