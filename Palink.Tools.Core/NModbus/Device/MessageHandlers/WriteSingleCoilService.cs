using Palink.Tools.NModbus.Contracts;
using Palink.Tools.NModbus.Interfaces;
using Palink.Tools.NModbus.Message;

namespace Palink.Tools.NModbus.Device.MessageHandlers;

internal class
    WriteSingleCoilService : ModbusFunctionServiceBase<WriteSingleCoilRequestResponse>
{
    public WriteSingleCoilService()
        : base(ModbusFunctionCodes.WriteSingleCoil)
    {
    }

    public override IModbusMessage CreateRequest(byte[] frame)
    {
        return CreateModbusMessage<WriteSingleCoilRequestResponse>(frame);
    }

    public override int GetRtuRequestBytesToRead(byte[] frameStart)
    {
        return 1;
    }

    public override int GetRtuResponseBytesToRead(byte[] frameStart)
    {
        return 4;
    }
}