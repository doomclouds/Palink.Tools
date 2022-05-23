using System;
using System.Collections.Generic;
using System.Linq;

namespace Palink.Tools.Extensions.ArrayExt;

/// <summary>
/// ArrayExtensions
/// </summary>
public static class ArrayExtensions
{
    /// <summary>
    /// Iterate Array
    /// </summary>
    /// <param name="array"></param>
    /// <param name="action"></param>
    public static void ForEach(this Array array, Action<Array, int[]> action)
    {
        if (array.LongLength == 0)
        {
            return;
        }

        var walker = new ArrayTraverse(array);
        do action(array, walker.Position);
        while (walker.MoveNext());
    }

    /// <summary>
    /// Slice
    /// </summary>
    /// <param name="source"></param>
    /// <param name="startIndex"></param>
    /// <param name="size"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public static IEnumerable<T> Slice<T>(this IEnumerable<T> source, int startIndex,
        int size)
    {
        if (source == null)
        {
            throw new ArgumentNullException(nameof(source));
        }

        var enumerable = source as T[] ?? source.ToArray();
        var num = enumerable.Length;

        if (startIndex < 0 || num < startIndex)
        {
            throw new ArgumentOutOfRangeException(nameof(startIndex));
        }

        if (size < 0 || startIndex + size > num)
        {
            throw new ArgumentOutOfRangeException(nameof(size));
        }

        return enumerable.Skip(startIndex).Take(size);
    }

    /// <summary>Check if an item is in a list.</summary>
    /// <param name="item">Item to check</param>
    /// <param name="list">List of items</param>
    /// <typeparam name="T">Type of the items</typeparam>
    public static bool IsIn<T>(this T item, params T[] list) =>
        list.Contains(item);

    /// <summary>Check if an item is in the given enumerable.</summary>
    /// <param name="item">Item to check</param>
    /// <param name="items">Items</param>
    /// <typeparam name="T">Type of the items</typeparam>
    public static bool IsIn<T>(this T item, IEnumerable<T> items) =>
        items.Contains(item);

    /// <summary>
    /// Concatenates the members of a constructed <see cref="T:System.Collections.Generic.IEnumerable`1" /> collection of type System.String, using the specified separator between each member.
    /// This is a shortcut for string.Join(...)
    /// </summary>
    /// <param name="source">A collection that contains the strings to concatenate.</param>
    /// <param name="separator">The string to use as a separator. separator is included in the returned string only if values has more than one element.</param>
    /// <returns>A string that consists of the members of values delimited by the separator string. If values has no members, the method returns System.String.Empty.</returns>
    public static string
        JoinAsString(this IEnumerable<string> source, string separator) =>
        string.Join(separator, source);

    /// <summary>
    /// Concatenates the members of a collection, using the specified separator between each member.
    /// This is a shortcut for string.Join(...)
    /// </summary>
    /// <param name="source">A collection that contains the objects to concatenate.</param>
    /// <param name="separator">The string to use as a separator. separator is included in the returned string only if values has more than one element.</param>
    /// <typeparam name="T">The type of the members of values.</typeparam>
    /// <returns>A string that consists of the members of values delimited by the separator string. If values has no members, the method returns System.String.Empty.</returns>
    public static string JoinAsString<T>(this IEnumerable<T> source, string separator) =>
        string.Join(separator, source);

    /// <summary>
    /// Filters a <see cref="T:System.Collections.Generic.IEnumerable`1" /> by given predicate if given condition is true.
    /// </summary>
    /// <param name="source">Enumerable to apply filtering</param>
    /// <param name="condition">A boolean value</param>
    /// <param name="predicate">Predicate to filter the enumerable</param>
    /// <returns>Filtered or not filtered enumerable based on <paramref name="condition" /></returns>
    public static IEnumerable<T> WhereIf<T>(
        this IEnumerable<T> source,
        bool condition,
        Func<T, bool> predicate)
    {
        return !condition ? source : source.Where(predicate);
    }

    /// <summary>
    /// Filters a <see cref="T:System.Collections.Generic.IEnumerable`1" /> by given predicate if given condition is true.
    /// </summary>
    /// <param name="source">Enumerable to apply filtering</param>
    /// <param name="condition">A boolean value</param>
    /// <param name="predicate">Predicate to filter the enumerable</param>
    /// <returns>Filtered or not filtered enumerable based on <paramref name="condition" /></returns>
    public static IEnumerable<T> WhereIf<T>(
        this IEnumerable<T> source,
        bool condition,
        Func<T, int, bool> predicate)
    {
        return !condition ? source : source.Where(predicate);
    }

    internal class ArrayTraverse
    {
        public int[] Position;
        private readonly int[] _maxLengths;

        public ArrayTraverse(Array array)
        {
            _maxLengths = new int[array.Rank];
            for (var i = 0; i < array.Rank; ++i)
            {
                _maxLengths[i] = array.GetLength(i) - 1;
            }

            Position = new int[array.Rank];
        }

        public bool MoveNext()
        {
            for (var i = 0; i < Position.Length; ++i)
            {
                if (Position[i] >= _maxLengths[i])
                {
                    continue;
                }

                Position[i]++;
                for (var j = 0; j < i; j++)
                {
                    Position[j] = 0;
                }

                return true;
            }

            return false;
        }
    }
}