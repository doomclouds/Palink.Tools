using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

namespace Palink.Tools.Extensions.ObjectExt;

/// <summary>
/// 对象判断扩展类
/// </summary>
public static class ObjectExtensions
{
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
    /// <param name="propName"></param>
    /// <returns></returns>
    public static string PropertyDescription(this object @object, string propName)
    {
        var value = @object.ToString();
        var prop = @object.GetType().GetProperty(propName);
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
    /// <param name="propName"></param>
    /// <returns></returns>
    public static string PropertyDisplayName(this object @object, string propName)
    {
        var value = @object.ToString();
        var prop = @object.GetType().GetProperty(propName);
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