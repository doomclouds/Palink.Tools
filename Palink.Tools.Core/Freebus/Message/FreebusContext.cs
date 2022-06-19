using System;
using System.Text;
using Palink.Tools.Extensions.StringExt;
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

    private string? _newLine;

    /// <summary>
    /// 数据接收单元结束符
    /// </summary>
    public string? NewLine
    {
        get => _newLine;
        set
        {
            IsWriteHexLog = value.IsNullOrEmpty();
            _newLine = value;
        }
    }

    /// <summary>
    /// 是否成功接收
    /// </summary>
    public bool Succeed { get; set; }

    /// <summary>
    /// 是否打印Hex通讯日志，默认为tue，否则会打印UTF8编码的字符串
    /// </summary>
    public bool IsWriteHexLog { get; set; } = true;

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