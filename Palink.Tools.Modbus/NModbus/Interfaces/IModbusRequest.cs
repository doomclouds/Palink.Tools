﻿namespace Palink.Tools.NModbus.Interfaces;

/// <summary>
///     Methods specific to a modbus request message.
/// </summary>
public interface IModbusRequest : IModbusMessage
{
    /// <summary>
    ///     Validate the specified response against the current request.
    /// </summary>
    void ValidateResponse(IModbusMessage response);
}