namespace Palink.Tools.NModbus.Interfaces;

public interface IModbusSerialTransport : IModbusTransport
{
    void DiscardInBuffer();

    bool CheckFrame { get; set; }

    bool CheckSumsMatch(IModbusMessage message, byte[] messageFrame);

    void IgnoreResponse();
}