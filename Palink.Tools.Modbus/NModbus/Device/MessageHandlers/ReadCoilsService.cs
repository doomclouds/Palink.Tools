using Palink.Tools.NModbus.Contracts;
using Palink.Tools.NModbus.Interfaces;
using Palink.Tools.NModbus.Message;

namespace Palink.Tools.NModbus.Device.MessageHandlers;

internal class ReadCoilsService : ModbusFunctionServiceBase<ReadCoilsInputsRequest>
{
    public ReadCoilsService()
        : base(ModbusFunctionCodes.ReadCoils)
    {
    }

    public override IModbusMessage CreateRequest(byte[] frame)
    {
        return CreateModbusMessage<ReadCoilsInputsRequest>(frame);
    }

    public override int GetRtuRequestBytesToRead(byte[] frameStart)
    {
        return 1;
    }

    public override int GetRtuResponseBytesToRead(byte[] frameStart)
    {
        return frameStart[2] + 1;
    }
}