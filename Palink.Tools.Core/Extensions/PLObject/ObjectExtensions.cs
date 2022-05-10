using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Palink.Tools.Extensions.PLArray;

namespace Palink.Tools.Extensions.PLObject;

/// <summary>
/// 对象判断扩展类
/// </summary>
public static class ObjectExtensions
{
    [NotNull] private static readonly MethodInfo? CloneMethod =
        typeof(object).GetMethod("MemberwiseClone",
            BindingFlags.NonPublic | BindingFlags.Instance);

    /// <summary>
    /// IsPrimitive
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public static bool IsPrimitive(this Type type)
    {
        if (type == typeof(string))
        {
            return true;
        }

        return type.IsValueType & type.IsPrimitive;
    }

    /// <summary>
    /// DeepClone
    /// </summary>
    /// <param name="originalObject"></param>
    /// <returns></returns>
    public static object? DeepClone(this object? originalObject)
    {
        return InternalCopy(originalObject,
            new Dictionary<object, object>(new ReferenceEqualityComparer()));
    }

    /// <summary>
    /// DeepClone
    /// </summary>
    /// <param name="original"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static T? DeepClone<T>(this T? original)
    {
        return (T?)DeepClone(original as object);
    }

    private static object? InternalCopy(object? originalObject,
        IDictionary<object, object> visited)
    {
        if (originalObject.IsNull())
        {
            return null;
        }

        var typeToReflect = originalObject.GetType();
        if (IsPrimitive(typeToReflect))
        {
            return originalObject;
        }

        if (visited.ContainsKey(originalObject))
        {
            return visited[originalObject];
        }

        if (typeof(Delegate).IsAssignableFrom(typeToReflect))
        {
            return null;
        }

        var cloneObject = CloneMethod.Invoke(originalObject, null);
        if (typeToReflect.IsArray)
        {
            var arrayType = typeToReflect.GetElementType();
            if (arrayType != null && !IsPrimitive(arrayType))
            {
                var clonedArray = (Array)cloneObject;
                clonedArray.ForEach((array, indices) =>
                    array.SetValue(InternalCopy(clonedArray.GetValue(indices), visited),
                        indices));
            }
        }

        visited.Add(originalObject, cloneObject);
        CopyFields(originalObject, visited, cloneObject, typeToReflect);
        RecursiveCopyBaseTypePrivateFields(originalObject, visited, cloneObject,
            typeToReflect);
        return cloneObject;
    }

    private static void RecursiveCopyBaseTypePrivateFields(object originalObject,
        IDictionary<object, object> visited, object cloneObject, Type typeToReflect)
    {
        if (typeToReflect.BaseType == null)
        {
            return;
        }

        RecursiveCopyBaseTypePrivateFields(originalObject, visited, cloneObject,
            typeToReflect.BaseType);
        CopyFields(originalObject, visited, cloneObject, typeToReflect.BaseType,
            BindingFlags.Instance | BindingFlags.NonPublic,
            info => info.IsPrivate);
    }

    private static void CopyFields(object originalObject,
        IDictionary<object, object> visited, object cloneObject, Type typeToReflect,
        BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.NonPublic |
                                    BindingFlags.Public | BindingFlags.FlattenHierarchy,
        Func<FieldInfo, bool>? filter = null)
    {
        foreach (var fieldInfo in typeToReflect.GetFields(bindingFlags))
        {
            if (filter != null && !filter(fieldInfo))
            {
                continue;
            }

            if (IsPrimitive(fieldInfo.FieldType))
            {
                continue;
            }

            var originalFieldValue = fieldInfo.GetValue(originalObject);
            var clonedFieldValue = InternalCopy(originalFieldValue, visited);
            fieldInfo.SetValue(cloneObject, clonedFieldValue);
        }
    }

    /// <summary>
    /// ReferenceEquals
    /// </summary>
    /// <param name="this"></param>
    /// <param name="o"></param>
    public new static bool ReferenceEquals(this object? @this, object? o)
    {
        return object.ReferenceEquals(@this, o);
    }

    /// <summary>
    /// IsDefaultValue
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static bool IsDefaultValue(this object? value)
    {
        if (value == null)
        {
            return true;
        }

        var type = value.GetType();
        if (type == typeof(bool))
        {
            return (bool)value == false;
        }

        if (type.IsEnum)
        {
            return (int)value == 0;
        }

        if (type == typeof(DateTime))
        {
            return (DateTime)value == default;
        }

        return double.Parse(value.ToString()) == 0;
    }

    /// <summary>
    /// IsNull
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public static bool IsNull([NotNullWhen(false)] this object? obj)
    {
        return obj == null;
    }

    /// <summary>
    /// NotNull
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public static bool NotNull([NotNullWhen(true)] this object? obj)
    {
        return obj != null;
    }

    /// <summary>
    /// ThrowIfNull
    /// </summary>
    /// <param name="obj"></param>
    public static void ThrowIfNull(this object? obj)
    {
        if (obj.IsNull())
        {
            throw new ArgumentNullException(nameof(obj));
        }
    }

    /// <summary>
    /// PropertyDescription
    /// </summary>
    /// <param name="object"></param>
    /// <param name="name"></param>
    /// <returns></returns>
    public static string PropertyDescription(this object @object, string name)
    {
        var value = @object.ToString();
        var prop = @object.GetType().GetProperty(name);
        var obj =
            prop?.GetCustomAttributes(typeof(DescriptionAttribute), false); //获取描述属性
        if (obj == null || obj.Length == 0) //当描述属性没有时，直接返回名称
            return value;
        var descriptionAttribute = (DescriptionAttribute)obj[0];
        return descriptionAttribute.Description;
    }

    /// <summary>
    /// PropertyDisplayName
    /// </summary>
    /// <param name="object"></param>
    /// <param name="name"></param>
    /// <returns></returns>
    public static string PropertyDisplayName(this object @object, string name)
    {
        var value = @object.ToString();
        var prop = @object.GetType().GetProperty(value);
        var obj =
            prop?.GetCustomAttributes(typeof(DisplayNameAttribute), false); //获取显示名称
        if (obj == null || obj.Length == 0) //当描述属性没有时，直接返回名称
            return value;
        var displayNameAttribute = (DisplayNameAttribute)obj[0];
        return displayNameAttribute.DisplayName;
    }

    /// <summary>
    /// Can be used to conditionally perform a function
    /// on an object and return the modified or the original object.
    /// It is useful for chained calls.
    /// </summary>
    /// <param name="obj">An object</param>
    /// <param name="condition">A condition</param>
    /// <param name="func">A function that is executed only if the condition is <code>true</code></param>
    /// <typeparam name="T">Type of the object</typeparam>
    /// <returns>
    /// Returns the modified object (by the <paramref name="func" /> if the <paramref name="condition" /> is <code>true</code>)
    /// or the original object if the <paramref name="condition" /> is <code>false</code>
    /// </returns>
    public static T If<T>(this T obj, bool condition, Func<T, T> func) =>
        condition ? func(obj) : obj;

    /// <summary>
    /// Can be used to conditionally perform an action
    /// on an object and return the original object.
    /// It is useful for chained calls on the object.
    /// </summary>
    /// <param name="obj">An object</param>
    /// <param name="condition">A condition</param>
    /// <param name="action">An action that is executed only if the condition is <code>true</code></param>
    /// <typeparam name="T">Type of the object</typeparam>
    /// <returns>Returns the original object.</returns>
    public static T If<T>(this T obj, bool condition, Action<T> action)
    {
        if (condition)
            action(obj);
        return obj;
    }
}

internal class ReferenceEqualityComparer : EqualityComparer<object>
{
    public override bool Equals(object x, object y)
    {
        return ReferenceEquals(x, y);
    }

    public override int GetHashCode(object obj)
    {
        return obj.GetHashCode();
    }
}