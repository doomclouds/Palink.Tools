﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace Palink.Tools.Extensions.PLArray;

/// <summary>
/// ArrayExtensions
/// </summary>
public static class ArrayExtensions
{
    /// <summary>
    /// 数组遍历
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
        while (walker.Step());
    }

    /// <summary>
    /// 切片
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

        public bool Step()
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