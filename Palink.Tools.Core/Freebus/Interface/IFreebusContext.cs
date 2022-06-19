namespace Palink.Tools.Freebus.Interface;

public interface IFreebusContext
{
    /// <summary>
    /// Protocol Data Unit
    /// 协议数据单元
    /// </summary>
    byte[] Pdu { get; set; }

    /// <summary>
    /// Data Receiving Unit
    /// 数据接收单元
    /// </summary>
    byte[] Dru { get; set; }

    /// <summary>
    /// 数据接收单元长度
    /// </summary>
    int? DruLength { get; set; }

    /// <summary>
    /// 数据接收单元结束符
    /// </summary>
    string? NewLine { set; get; }

    /// <summary>
    /// 是否成功接收
    /// </summary>
    public bool Succeed { get; set; }

    /// <summary>
    /// 是否打印Hex通讯日志，默认为tue，否则会打印UTF8编码的字符串
    /// </summary>
    public bool IsWriteHexLog { get; set; }

    /// <summary>
    /// 获取数据协议单元
    /// </summary>
    /// <returns></returns>
    string GetPduString();

    /// <summary>
    /// 获取数据接收单元
    /// </summary>
    /// <returns></returns>
    string GetDruString();

    /// <summary>
    /// 设置数据协议单元
    /// </summary>
    /// <returns></returns>
    void SetPduString(string cmd);
}