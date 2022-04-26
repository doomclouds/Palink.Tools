using Palink.Tools.NModbus.Data;

namespace Palink.Tools.NModbus.Message;

internal abstract class AbstractModbusMessageWithData<TData> : AbstractModbusMessage
    where TData : IModbusMessageDataCollection
{
    internal AbstractModbusMessageWithData()
    {
    }

    internal AbstractModbusMessageWithData(byte slaveAddress, byte functionCode)
        : base(slaveAddress, functionCode)
    {
    }

    public TData? Data
    {
        get => MessageImpl.Data == null ? default : (TData)MessageImpl.Data;
        set => MessageImpl.Data = value;
    }
}