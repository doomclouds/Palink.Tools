using Palink.Tools.NModbus.Contracts;
using Palink.Tools.NModbus.Interfaces;
using Palink.Tools.NModbus.Message;

namespace Palink.Tools.NModbus.Device.MessageHandlers;

internal class WriteFileRecordService
    : ModbusFunctionServiceBase<WriteFileRecordRequest>
{
    public WriteFileRecordService()
        : base(ModbusFunctionCodes.WriteFileRecord)
    {
    }

    public override IModbusMessage CreateRequest(byte[] frame)
    {
        return CreateModbusMessage<WriteFileRecordRequest>(frame);
    }

    public override int GetRtuRequestBytesToRead(byte[] frameStart)
    {
        return frameStart[2] + 1;
    }

    public override int GetRtuResponseBytesToRead(byte[] frameStart)
    {
        return frameStart[2] + 1;
    }
}