using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
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
    /// 判断对象是否是原始对象
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
    /// 深度Clone
    /// </summary>
    /// <param name="originalObject"></param>
    /// <returns></returns>
    public static object? DeepClone(this object? originalObject)
    {
        return InternalCopy(originalObject,
            new Dictionary<object, object>(new ReferenceEqualityComparer()));
    }

    /// <summary>
    /// 深度Clone
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
    /// 严格比较两个对象是否是同一对象(判断引用)
    /// </summary>
    /// <param name="this">自己</param>
    /// <param name="o">需要比较的对象</param>
    /// <returns>是否同一对象</returns>
    public new static bool ReferenceEquals(this object? @this, object? o)
    {
        return object.ReferenceEquals(@this, o);
    }

    /// <summary>
    /// 是否是默认值
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
    /// 判断对象是否为空
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public static bool IsNull([NotNullWhen(false)] this object? obj)
    {
        return obj == null;
    }

    /// <summary>
    /// 判断对象是否不为空
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    [Obsolete]
    public static bool IsNotNull([NotNullWhen(true)] this object? obj)
    {
        return obj != null;
    }

    /// <summary>
    /// 判断对象是否不为空
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public static bool NotNull([NotNullWhen(true)] this object? obj)
    {
        return obj != null;
    }

    /// <summary>
    /// 对象为空则抛出异常
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
    /// 获取属性描述信息
    /// </summary>
    /// <param name="object"></param>
    /// <param name="name">属性名称</param>
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
    /// 获取属性显示名称
    /// </summary>
    /// <param name="object"></param>
    /// <param name="name">属性名称</param>
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
    /// Used to simplify and beautify casting an object to a type.
    /// </summary>
    /// <typeparam name="T">Type to be casted</typeparam>
    /// <param name="obj">Object to cast</param>
    /// <returns>Casted object</returns>
    public static T As<T>(this object obj) where T : class => (T)obj;

    /// <summary>
    /// Converts given object to a value type using <see cref="M:System.Convert.ChangeType(System.Object,System.Type)" /> method.
    /// </summary>
    /// <param name="obj">Object to be converted</param>
    /// <typeparam name="T">Type of the target object</typeparam>
    /// <returns>Converted object</returns>
    public static T To<T>(this object obj) where T : struct => typeof(T) == typeof(Guid)
        ? (T)TypeDescriptor.GetConverter(typeof(T))
            .ConvertFromInvariantString(obj.ToString())
        : (T)Convert.ChangeType(obj, typeof(T),
            CultureInfo.InvariantCulture);

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