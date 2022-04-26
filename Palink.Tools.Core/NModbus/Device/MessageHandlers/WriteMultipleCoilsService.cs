using Palink.Tools.NModbus.Contracts;
using Palink.Tools.NModbus.Interfaces;
using Palink.Tools.NModbus.Message;

namespace Palink.Tools.NModbus.Device.MessageHandlers;

internal class
    WriteMultipleCoilsService : ModbusFunctionServiceBase<WriteMultipleCoilsRequest>
{
    public WriteMultipleCoilsService()
        : base(ModbusFunctionCodes.WriteMultipleCoils)
    {
    }

    public override IModbusMessage CreateRequest(byte[] frame)
    {
        return CreateModbusMessage<WriteMultipleCoilsRequest>(frame);
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