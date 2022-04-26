using System;
using System.Collections.Generic;
using System.Globalization;
using Palink.Tools.NModbus.Contracts;
using Palink.Tools.NModbus.Interfaces;

namespace Palink.Tools.NModbus.Message;

internal class SlaveExceptionResponse : AbstractModbusMessage, IModbusMessage
{
    private static readonly Dictionary<byte, string> ExceptionMessages =
        CreateExceptionMessages();

    public SlaveExceptionResponse()
    {
    }

    public SlaveExceptionResponse(byte slaveAddress, byte functionCode,
        byte exceptionCode)
        : base(slaveAddress, functionCode)
    {
        SlaveExceptionCode = exceptionCode;
    }

    public override int MinimumFrameSize => 3;

    public byte? SlaveExceptionCode
    {
        get => MessageImpl.ExceptionCode ?? default;
        set => MessageImpl.ExceptionCode = value;
    }

    /// <summary>
    ///     Returns a <see cref="T:System.String"></see> that represents the current <see cref="T:System.Object"></see>.
    /// </summary>
    /// <returns>
    ///     A <see cref="T:System.String"></see> that represents the current <see cref="T:System.Object"></see>.
    /// </returns>
    public override string ToString()
    {
        if (!SlaveExceptionCode.HasValue)
            throw new ArgumentNullException(nameof(SlaveExceptionCode),
                "ByteCount dose not null");

        var msg = ExceptionMessages.ContainsKey(SlaveExceptionCode.Value)
            ? ExceptionMessages[SlaveExceptionCode.Value]
            : Resources.Unknown;

        return string.Format(CultureInfo.InvariantCulture,
            Resources.SlaveExceptionResponseFormat,
            Environment.NewLine,
            FunctionCode,
            SlaveExceptionCode,
            msg);
    }

    internal static Dictionary<byte, string> CreateExceptionMessages()
    {
        return new Dictionary<byte, string>(9)
        {
            { 1, Resources.IllegalFunction },
            { 2, Resources.IllegalDataAddress },
            { 3, Resources.IllegalDataValue },
            { 4, Resources.SlaveDeviceFailure },
            { 5, Resources.Acknowledge },
            { 6, Resources.SlaveDeviceBusy },
            { 8, Resources.MemoryParityError },
            { 10, Resources.GatewayPathUnavailable },
            { 11, Resources.GatewayTargetDeviceFailedToRespond }
        };
    }

    protected override void InitializeUnique(byte[] frame)
    {
        if (FunctionCode <= Modbus.ExceptionOffset)
        {
            throw new FormatException(Resources
                .SlaveExceptionResponseInvalidFunctionCode);
        }

        SlaveExceptionCode = frame[2];
    }
}