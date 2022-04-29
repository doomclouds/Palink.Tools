using Palink.Tools.NModbus.Contracts;
using Palink.Tools.NModbus.Interfaces;
using Palink.Tools.NModbus.Message;

namespace Palink.Tools.NModbus.Device.MessageHandlers;

internal class WriteMultipleRegistersService
    : ModbusFunctionServiceBase<WriteMultipleRegistersRequest>
{
    public WriteMultipleRegistersService()
        : base(ModbusFunctionCodes.WriteMultipleRegisters)
    {
    }

    public override IModbusMessage CreateRequest(byte[] frame)
    {
        return CreateModbusMessage<WriteMultipleRegistersRequest>(frame);
    }

    public override int GetRtuRequestBytesToRead(byte[] frameStart)
    {
        return frameStart[6] + 2;
    }

    public override int GetRtuResponseBytesToRead(byte[] frameStart)
    {
        return 4;
    }
}