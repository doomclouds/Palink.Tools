using System.Collections.Generic;

namespace Palink.Tools.NModbus.Extensions;

internal static class DictionaryExtensions
{
    /// <summary>
    /// Gets the specified value in the dictionary. If not found, returns default for TValue.
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    /// <param name="dictionary"></param>
    /// <param name="key"></param>
    /// <returns></returns>
    internal static TValue? GetValueOrDefault<TKey, TValue>(
        this IDictionary<TKey, TValue> dictionary, TKey key)
    {
        return dictionary.TryGetValue(key, out var value) ? value : default;
    }
}