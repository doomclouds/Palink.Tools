using System;
using System.Text;
using Palink.Tools.Freebus.Interface;

namespace Palink.Tools.Freebus.Message;

public class FreebusContext : IFreebusContext
{
    /// <summary>
    /// Protocol Data Unit
    /// 协议数据单元
    /// </summary>
    public byte[] Pdu { get; set; } = Array.Empty<byte>();

    /// <summary>
    /// Data Receiving Unit
    /// 数据接收单元
    /// </summary>
    public byte[] Dru { get; set; } = Array.Empty<byte>();

    /// <summary>
    /// 数据接收单元长度
    /// </summary>
    public int? DruLength { get; set; }

    /// <summary>
    /// 数据接收单元结束符
    /// </summary>
    public string? NewLine { get; set; }

    public bool Succeed { get; set; }

    public string GetPduString()
    {
        return Encoding.UTF8.GetString(Pdu);
    }

    public string GetDruString()
    {
        return Encoding.UTF8.GetString(Dru);
    }

    public void SetPduString(string cmd)
    {
        Pdu = Encoding.UTF8.GetBytes(cmd);
    }
}