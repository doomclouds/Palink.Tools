using System;
using System.Collections;
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
    [NotNull]private static readonly MethodInfo? CloneMethod =
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
    /// 判断是否为null，null或0长度都返回true
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="value"></param>
    /// <returns></returns>
    public static bool IsNullOrEmpty<T>([NotNullWhen(false)]this T? value)
        where T : class
    {
        #region 1.对象级别

        //引用为null
        var isObjectNull = value is null;
        if (isObjectNull)
        {
            return true;
        }

        //判断是否为集合
        var tempEnumerator = (value as IEnumerable)?.GetEnumerator();
        if (tempEnumerator == null) return false; //这里出去代表是对象 且 引用不为null.所以为false

        #endregion 1.对象级别

        #region 2.集合级别

        //到这里就代表是集合且引用不为空，判断长度
        //MoveNext方法返回tue代表集合中至少有一个数据,返回false就代表0长度
        var isZeroLength = tempEnumerator.MoveNext() == false;
        return isZeroLength;

        #endregion 2.集合级别
    }

    /// <summary>
    /// 判断是否不为null
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="value"></param>
    /// <returns></returns>
    public static bool IsNotNullOrEmpty<T>([NotNullWhen(true)]this T? value)
        where T : class
    {
        return !value.IsNullOrEmpty();
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
    public static bool IsNull([NotNullWhen(false)]this object? obj)
    {
        return obj == null;
    }

    /// <summary>
    /// 判断对象是否不为空
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public static bool IsNotNull([NotNullWhen(true)]this object? obj)
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