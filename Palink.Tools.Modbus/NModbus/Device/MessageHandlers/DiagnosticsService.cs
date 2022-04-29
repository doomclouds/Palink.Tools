using Palink.Tools.NModbus.Contracts;
using Palink.Tools.NModbus.Interfaces;
using Palink.Tools.NModbus.Message;

namespace Palink.Tools.NModbus.Device.MessageHandlers;

internal class DiagnosticsService : ModbusFunctionServiceBase<IModbusMessage>
{
    public DiagnosticsService()
        : base(ModbusFunctionCodes.Diagnostics)
    {
    }

    public override IModbusMessage CreateRequest(byte[] frame)
    {
        return CreateModbusMessage<DiagnosticsRequestResponse>(frame);
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