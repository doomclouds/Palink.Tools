using System.IO;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Palink.Tools.Extensions.PLString;

namespace Palink.Tools.Extensions.PLSerialize;

/// <summary>
/// 实体序列化扩展
/// </summary>
public static class SerializeExtensions
{
    /// <summary>
    /// 实体对象转JSON字符串
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="dateFormatString"></param>
    /// <param name="ignoreNull"></param>
    /// <returns></returns>
    public static string ToJson(this object? obj,
        bool ignoreNull,
        string dateFormatString = "yyyy-MM-dd HH:mm:ss")
    {
        return JsonConvert.SerializeObject(obj, Formatting.None,
            new JsonSerializerSettings
            {
                DateFormatString = dateFormatString,
                NullValueHandling = ignoreNull
                    ? NullValueHandling.Ignore
                    : NullValueHandling.Include
            });
    }

    /// <summary>
    /// 实体对象转JSON字符串
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="jsonSerializerSettings"></param>
    /// <returns></returns>
    public static string ToJson(this object? obj,
        JsonSerializerSettings? jsonSerializerSettings = default)
    {
        return JsonConvert.SerializeObject(obj, Formatting.None, jsonSerializerSettings);
    }

    /// <summary>
    /// JSON字符串转实体对象
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="jsonStr"></param>
    /// <param name="jsonSerializerSettings"></param>
    /// <returns></returns>
    public static T? FromJson<T>(this string? jsonStr,
        JsonSerializerSettings? jsonSerializerSettings = default)
    {
        return jsonStr.IsNullOrEmpty()
            ? default
            : JsonConvert.DeserializeObject<T>(jsonStr, jsonSerializerSettings);
    }

    /// <summary>
    /// 字符串序列化成字节序列
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public static byte[]? SerializeUtf8(this string? str)
    {
        return str == null ? null : Encoding.UTF8.GetBytes(str);
    }

    /// <summary>
    /// 字节序列序列化成字符串
    /// </summary>
    /// <param name="stream"></param>
    /// <returns></returns>
    public static string? DeserializeUtf8(this byte[]? stream)
    {
        return stream == null ? null : Encoding.UTF8.GetString(stream);
    }

    /// <summary>
    /// 根据key将json文件内容转换为指定对象
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="filePath"></param>
    /// <param name="key"></param>
    /// <param name="jsonSerializerSettings"></param>
    /// <returns></returns>
    public static async Task<T?> FromJsonFile<T>(this string? filePath, string key = "",
        JsonSerializerSettings? jsonSerializerSettings = default)
        where T : new()
    {
        if (!File.Exists(filePath) || filePath.IsNullOrEmpty()) return new T();

        using var reader = new StreamReader(filePath);
        var json = await reader.ReadToEndAsync();

        if (string.IsNullOrEmpty(key))
            return JsonConvert.DeserializeObject<T>(json, jsonSerializerSettings);

        return JsonConvert.DeserializeObject<object>(json, jsonSerializerSettings) is not
            JObject obj
            ? new T()
            : JsonConvert.DeserializeObject<T>(obj[key]?.ToString() ?? string.Empty,
                jsonSerializerSettings);
    }
}