using System;
using Palink.Tools.Freebus.Interface;

namespace Palink.Tools.Freebus.Message;

/// <summary>
/// 基本消息体
/// </summary>
[Obsolete("Master与Message不再使用，请使用Freebus相关方法")]
public class BaseMessage : IMessage
{
    public byte[] Data { get; set; } = Array.Empty<byte>();
    public byte[] Buffer { get; set; } = Array.Empty<byte>();
    public int ReadBytes { get; set; } = 13;
    public int SendBytes { get; set; } = 13;
}