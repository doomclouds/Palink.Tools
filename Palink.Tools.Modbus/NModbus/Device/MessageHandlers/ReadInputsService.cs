using Palink.Tools.NModbus.Contracts;
using Palink.Tools.NModbus.Interfaces;
using Palink.Tools.NModbus.Message;

namespace Palink.Tools.NModbus.Device.MessageHandlers;

internal class ReadInputsService : ModbusFunctionServiceBase<ReadCoilsInputsRequest>
{
    public ReadInputsService() : base(ModbusFunctionCodes.ReadInputs)
    {
    }

    /// <summary>Creates a message that wraps the request frame.</summary>
    /// <param name="frame"></param>
    /// <returns></returns>
    public override IModbusMessage CreateRequest(byte[] frame)
    {
        return CreateModbusMessage<ReadCoilsInputsRequest>(frame);
    }

    /// <summary>Gets the number of bytes to read for a request</summary>
    /// <param name="frameStart"></param>
    /// <returns></returns>
    public override int GetRtuRequestBytesToRead(byte[] frameStart)
    {
        return 1;
    }

    /// <summary>Gets the number of bytes to read for a response.</summary>
    /// <param name="frameStart"></param>
    /// <returns></returns>
    public override int GetRtuResponseBytesToRead(byte[] frameStart)
    {
        return frameStart[2] + 1;
    }
}