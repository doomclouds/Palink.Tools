using System;

namespace Palink.Tools.Freebus.Interface;

/// <summary>
/// 信息存储类
/// </summary>
[Obsolete("Master与Message不再使用，请使用Freebus相关方法")]
public interface IMessage
{
    public byte[] Data { get; set; }
    public byte[] Buffer { get; set; }
    public int ReadBytes { get; set; }
    public int SendBytes { get; set; }
}