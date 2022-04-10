using System;

namespace Palink.Tools.System.PLAttribute;

/// <summary>
/// json序列化属性，区分与读取的数据与发送的数据序列化时的属性名称
/// </summary>
public class JsonOcrAttribute : Attribute
{
    /// <summary>
    /// 读取名称
    /// </summary>
    public string ReadName { get; set; }

    /// <summary>
    /// 写入名称
    /// </summary>
    public string WriteName { get; set; }

    /// <summary>
    /// 是否可读
    /// </summary>
    public bool Readable { get; set; }

    /// <summary>
    /// 是否可写
    /// </summary>
    public bool Writable { get; set; }

    /// <summary>
    /// 构造器
    /// </summary>
    public JsonOcrAttribute(string readName, string writeName)
    {
        ReadName = readName;
        WriteName = writeName;
    }
}