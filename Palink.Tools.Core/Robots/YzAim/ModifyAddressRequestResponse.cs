using System.IO;
using Palink.Tools.NModbus.Data;
using Palink.Tools.NModbus.Interfaces;
using Palink.Tools.NModbus.Message;

namespace Palink.Tools.Robots.YzAim;

internal class
    ModifyAddressRequestResponse : AbstractModbusMessageWithData<RegisterCollection>,
        IModbusRequest
{
    public ModifyAddressRequestResponse()
    {
    }

    public ModifyAddressRequestResponse(byte slaveAddress, ushort startAddress,
        RegisterCollection data)
        : base(slaveAddress, 0x7a)
    {
        Data = data;
    }

    private byte _targetId;

    public override int MinimumFrameSize => 6;

    protected override void InitializeUnique(byte[] frame)
    {
        _targetId = frame[0];
    }

    public void ValidateResponse(IModbusMessage response)
    {
        if (response.MessageFrame[0] != _targetId || response.MessageFrame[1] != 0x7a)
        {
            throw new IOException("修改数据失败");
        }
    }
}