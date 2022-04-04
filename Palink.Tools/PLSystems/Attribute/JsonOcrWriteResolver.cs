using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Palink.Tools.PLSystems.Attribute;

/// <summary>
/// 使用WriteName进行Json序列化
/// </summary>
public class JsonOcrWriteResolver : DefaultContractResolver
{
    /// <summary>
    /// CreateProperty
    /// </summary>
    /// <param name="member"></param>
    /// <param name="memberSerialization"></param>
    /// <returns></returns>
    protected override JsonProperty CreateProperty(MemberInfo member,
        MemberSerialization memberSerialization)
    {
        var property = base.CreateProperty(member, memberSerialization);
        if (member.GetCustomAttribute(typeof(JsonOcrAttribute)) is JsonOcrAttribute attr)
        {
            property.PropertyName = attr.WriteName ?? property.PropertyName;
            property.Writable = attr.Writable;
        }

        return property;
    }
}

/// <summary>
/// 使用ReadName进行Json序列化
/// </summary>
public class JsonOcrReadResolver : DefaultContractResolver
{
    /// <summary>
    /// CreateProperty
    /// </summary>
    /// <param name="member"></param>
    /// <param name="memberSerialization"></param>
    /// <returns></returns>
    protected override JsonProperty CreateProperty(MemberInfo member,
        MemberSerialization memberSerialization)
    {
        var property = base.CreateProperty(member, memberSerialization);
        if (member.GetCustomAttribute(typeof(JsonOcrAttribute)) is JsonOcrAttribute attr)
        {
            property.PropertyName = attr.ReadName ?? property.PropertyName;
            property.Readable = attr.Readable;
        }

        return property;
    }
}