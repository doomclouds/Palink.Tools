﻿using Palink.Tools.NModbus.Contracts;
using Palink.Tools.NModbus.Interfaces;
using Palink.Tools.NModbus.IO.Message;

namespace Palink.Tools.NModbus.Device.MessageHandlers;

internal class
    ReadInputRegistersService : ModbusFunctionServiceBase<
        ReadHoldingInputRegistersRequest>
{
    public ReadInputRegistersService()
        : base(ModbusFunctionCodes.ReadInputRegisters)
    {
    }

    public override IModbusMessage CreateRequest(byte[] frame)
    {
        return CreateModbusMessage<ReadHoldingInputRegistersRequest>(frame);
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