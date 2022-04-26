using Palink.Tools.NModbus.Interfaces;

namespace Palink.Tools.NModbus.Device;

/// <summary>
/// Base class for FunctionService
/// </summary>
/// <typeparam name="TRequest">The type of request to handle.</typeparam>
internal abstract class ModbusFunctionServiceBase<TRequest> : IModbusFunctionService
    where TRequest : class
{
    protected ModbusFunctionServiceBase(byte functionCode)
    {
        FunctionCode = functionCode;
    }

    public byte FunctionCode { get; }

    public abstract IModbusMessage CreateRequest(byte[] frame);

    public abstract int GetRtuRequestBytesToRead(byte[] frameStart);

    public abstract int GetRtuResponseBytesToRead(byte[] frameStart);

    protected static T CreateModbusMessage<T>(byte[] frame)
        where T : IModbusMessage, new()
    {
        IModbusMessage message = new T();
        message.Initialize(frame);

        return (T)message;
    }
}