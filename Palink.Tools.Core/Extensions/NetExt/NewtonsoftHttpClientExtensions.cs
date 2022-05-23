using System;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Palink.Tools.Extensions.StringExt;

namespace Palink.Tools.Extensions.NetExt;

/// <summary>
/// NewtonsoftHttpClientExtensions
/// </summary>
public static class NewtonsoftHttpClientExtensions
{
    /// <summary>
    /// GetFromJsonAsync
    /// </summary>
    /// <param name="httpClient"></param>
    /// <param name="uri"></param>
    /// <param name="settings"></param>
    /// <param name="cancellationToken"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static async Task<T?> GetFromJsonAsync<T>(this HttpClient httpClient,
        string uri, JsonSerializerSettings? settings = null,
        CancellationToken cancellationToken = default)
    {
        ThrowIfInvalidParams(uri);

        var response = await httpClient.GetAsync(uri, cancellationToken);

        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync();

        return JsonConvert.DeserializeObject<T>(json, settings);
    }

    /// <summary>
    /// PostAsJsonAsync
    /// </summary>
    /// <param name="httpClient"></param>
    /// <param name="uri"></param>
    /// <param name="value"></param>
    /// <param name="settings"></param>
    /// <param name="cancellationToken"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    public static async Task<HttpResponseMessage> PostAsJsonAsync<T>(
        this HttpClient httpClient, string uri, T value,
        JsonSerializerSettings? settings = null,
        CancellationToken cancellationToken = default)
    {
        ThrowIfInvalidParams(uri);

        if (value is null)
        {
            throw new ArgumentNullException(nameof(value));
        }

        var json = JsonConvert.SerializeObject(value, settings);

        var response = await httpClient.PostAsync(uri,
            new StringContent(json, Encoding.UTF8, "application/json"),
            cancellationToken);

        response.EnsureSuccessStatusCode();

        return response;
    }

    /// <summary>
    /// PutAsJsonAsync
    /// </summary>
    /// <param name="httpClient"></param>
    /// <param name="uri"></param>
    /// <param name="value"></param>
    /// <param name="settings"></param>
    /// <param name="cancellationToken"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    public static async Task<HttpResponseMessage> PutAsJsonAsync<T>(
        this HttpClient httpClient, string uri, T value,
        JsonSerializerSettings? settings = null,
        CancellationToken cancellationToken = default)
    {
        ThrowIfInvalidParams(uri);

        if (value is null)
        {
            throw new ArgumentNullException(nameof(value));
        }

        var json = JsonConvert.SerializeObject(value, settings);

        var response = await httpClient.PutAsync(uri,
            new StringContent(json, Encoding.UTF8, "application/json"),
            cancellationToken);

        response.EnsureSuccessStatusCode();

        return response;
    }

    private static void ThrowIfInvalidParams(string uri)
    {
        if (uri.IsNullOrWhiteSpace())
        {
            throw new ArgumentException("Can't be null or empty", nameof(uri));
        }
    }

    /// <summary>
    /// ReadFromJsonAsync
    /// </summary>
    /// <param name="content"></param>
    /// <param name="settings"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    public static Task<T?> ReadFromJsonAsync<T>(this HttpContent? content,
        JsonSerializerSettings? settings = null)
    {
        if (content == null)
            throw new ArgumentNullException(nameof(content));
        return ReadFromJsonAsyncCore<T>(content, settings);
    }

    private static async Task<T?> ReadFromJsonAsyncCore<T>(HttpContent content,
        JsonSerializerSettings? settings)
    {
        var json = await content.ReadAsStringAsync();
        if (json.IsNullOrEmpty()) return default;

        var t = JsonConvert.DeserializeObject<T>(json, settings);
        return t;
    }
}