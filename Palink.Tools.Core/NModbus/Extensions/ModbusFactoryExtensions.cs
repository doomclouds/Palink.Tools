using System;
using Palink.Tools.IO;
using Palink.Tools.Logging;
using Palink.Tools.NModbus.Device;
using Palink.Tools.NModbus.Interfaces;

namespace Palink.Tools.NModbus.Extensions;

public static class ModbusFactoryExtensions
{
    /// <summary>
    /// Creates an RTU master.
    /// </summary>
    /// <param name="factory"></param>
    /// <param name="streamResource"></param>
    /// <returns></returns>
    public static IModbusSerialMaster CreateRtuMaster(this IModbusFactory factory,
        IStreamResource streamResource)
    {
        var transport = factory.CreateRtuTransport(streamResource);

        return new ModbusSerialMaster(transport);
    }

    /// <summary>
    /// Creates an ASCII master.
    /// </summary>
    /// <param name="factory"></param>
    /// <param name="streamResource"></param>
    /// <returns></returns>
    public static IModbusSerialMaster CreateAsciiMaster(this IModbusFactory factory,
        IStreamResource streamResource)
    {
        IModbusAsciiTransport transport = factory.CreateAsciiTransport(streamResource);

        return new ModbusSerialMaster(transport);
    }

    private const int MinRequestFrameLength = 3;

    internal static IModbusMessage CreateModbusRequest(this IModbusFactory factory,
        byte[] frame)
    {
        if (frame.Length < MinRequestFrameLength)
        {
            var msg =
                $"Argument 'frame' must have a length of at least {MinRequestFrameLength} bytes.";
            throw new FormatException(msg);
        }

        var functionCode = frame[1];

        var functionService = factory.GetFunctionService(functionCode);
        if (functionService != null)
        {
            return functionService.CreateRequest(frame);
        }

        {
            var msg = $"Function code {functionCode} not supported.";
            factory.Logger.Warning(msg);

            throw new NotImplementedException(msg);
        }
    }

    internal static IModbusFunctionService GetFunctionServiceOrThrow(
        this IModbusFactory factory, byte functionCode)
    {
        var service = factory.GetFunctionService(functionCode);

        if (service != null)
        {
            return service;
        }

        var msg = $"Function code {functionCode} not supported.";
        factory.Logger.Warning(msg);

        throw new NotImplementedException(msg);
    }
}