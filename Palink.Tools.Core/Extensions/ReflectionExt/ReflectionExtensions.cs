using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Palink.Tools.Extensions.ObjectExt;
using Palink.Tools.Extensions.StringExt;

namespace Palink.Tools.Extensions.ReflectionExt;

/// <summary>
/// ReflectionExtensions
/// </summary>
[SuppressMessage("ReSharper", "CoVariantArrayConversion")]
public static class ReflectionExtensions
{
    #region 属性字段设置

    public static BindingFlags Bf = BindingFlags.DeclaredOnly | BindingFlags.Public |
        BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static;

    public static T? InvokeMethod<T>(this object? obj, string? methodName,
        object[]? args = default)
    {
        if (obj.IsNull())
            throw new ArgumentNullException(nameof(obj), "can not null");
        if (methodName.IsNullOrEmpty())
            throw new ArgumentNullException(nameof(methodName), "can not null");

        if (obj.GetType().GetMethod(methodName)?.Invoke(obj, args) is T t)
        {
            return t;
        }

        return default;
    }

    public static void InvokeMethod(this object? obj, string? methodName,
        object[]? args = default)
    {
        if (obj.IsNull())
            throw new ArgumentNullException(nameof(obj), "can not null");
        if (methodName.IsNullOrEmpty())
            throw new ArgumentNullException(nameof(methodName), "can not null");
        obj.GetType().GetMethod(methodName)?.Invoke(obj, args);
    }

    public static void SetField(this object? obj, string name, object value)
    {
        if (obj.IsNull())
            throw new ArgumentNullException(nameof(obj), "can not null");
        SetProperty(obj, name, value);
    }

    public static T GetField<T>(this object? obj, string name)
    {
        if (obj.IsNull())
            throw new ArgumentNullException(nameof(obj), "can not null");
        return GetProperty<T>(obj, name);
    }

    public static FieldInfo[] GetFields(this object? obj)
    {
        if (obj.IsNull())
            throw new ArgumentNullException(nameof(obj), "can not null");
        var fieldInfos = obj.GetType().GetFields(Bf);
        return fieldInfos;
    }

    public static void SetProperty(this object? obj, string name, object value)
    {
        if (obj.IsNull())
            throw new ArgumentNullException(nameof(obj), "can not null");
        var parameter = Expression.Parameter(obj.GetType(), "e");
        var property = Expression.PropertyOrField(parameter, name);
        var before = Expression.Lambda(property, parameter).Compile().DynamicInvoke(obj);
        if (value.Equals(before))
        {
            return;
        }

        if (property.Type.IsGenericType &&
            property.Type.GetGenericTypeDefinition() == typeof(Nullable<>))
        {
            obj.GetType().GetProperty(name)?.SetValue(obj, value);
        }
        else
        {
            var valueExpression = Expression.Parameter(value.GetType(), "v");
            var assign = Expression.Assign(property, valueExpression);
            Expression.Lambda(assign, parameter, valueExpression).Compile()
                .DynamicInvoke(obj, value);
        }
    }

    public static T GetProperty<T>(this object? obj, string name)
    {
        if (obj.IsNull())
            throw new ArgumentNullException(nameof(obj), "can not null");
        var parameter = Expression.Parameter(obj.GetType(), "e");
        var property = Expression.PropertyOrField(parameter, name);
        return (T)Expression.Lambda(property, parameter).Compile().DynamicInvoke(obj);
    }

    public static PropertyInfo[] GetProperties(this object? obj)
    {
        if (obj.IsNull())
            throw new ArgumentNullException(nameof(obj), "can not null");
        var propertyInfos = obj.GetType().GetProperties(Bf);
        return propertyInfos;
    }

    #endregion 属性字段设置

    #region 创建实例

    public static object? GetInstance(this Type type)
    {
        return GetInstance<TypeToIgnore, object>(type, null);
    }

    public static T? GetInstance<T>(this Type type) where T : class, new()
    {
        return GetInstance<TypeToIgnore, T>(type, null);
    }

    public static T? GetInstance<T>(string type) where T : class, new()
    {
        return GetInstance<TypeToIgnore, T>(Type.GetType(type), null);
    }

    public static object? GetInstance(string type)
    {
        return GetInstance<TypeToIgnore, object>(Type.GetType(type), null);
    }

    public static T? GetInstance<TArg, T>(this Type? type, TArg? argument)
        where T : class, new()
    {
        return GetInstance<TArg, TypeToIgnore, T>(type, argument, null);
    }

    public static T? GetInstance<TArg, T>(string type, TArg? argument)
        where T : class, new()
    {
        return GetInstance<TArg, TypeToIgnore, T>(Type.GetType(type), argument, null);
    }

    public static T? GetInstance<TArg1, TArg2, T>(this Type? type, TArg1? argument1,
        TArg2? argument2) where T : class, new()
    {
        return GetInstance<TArg1, TArg2, TypeToIgnore, T>(type, argument1, argument2,
            null);
    }

    public static T? GetInstance<TArg1, TArg2, T>(string type, TArg1? argument1,
        TArg2? argument2) where T : class, new()
    {
        return GetInstance<TArg1, TArg2, TypeToIgnore, T>(Type.GetType(type), argument1,
            argument2, null);
    }

    public static T? GetInstance<TArg1, TArg2, TArg3, T>(this Type? type,
        TArg1? argument1,
        TArg2? argument2, TArg3? argument3) where T : class, new()
    {
        return InstanceCreationFactory<TArg1, TArg2, TArg3, T>.CreateInstanceOf(type,
            argument1, argument2, argument3);
    }

    public static T? GetInstance<TArg1, TArg2, TArg3, T>(string type, TArg1? argument1,
        TArg2? argument2, TArg3? argument3) where T : class, new()
    {
        return InstanceCreationFactory<TArg1, TArg2, TArg3, T>.CreateInstanceOf(
            Type.GetType(type), argument1, argument2, argument3);
    }

    private class TypeToIgnore
    {
    }

    private static class InstanceCreationFactory<TArg1, TArg2, TArg3, TObject>
        where TObject : class, new()
    {
        private static readonly Dictionary<Type?, Func<TArg1?, TArg2?, TArg3?, TObject?>>
            InstanceCreationMethods = new();

        public static TObject? CreateInstanceOf(Type? type, TArg1? arg1, TArg2? arg2,
            TArg3? arg3)
        {
            CacheInstanceCreationMethodIfRequired(type);

            return InstanceCreationMethods[type](arg1, arg2, arg3);
        }

        private static void CacheInstanceCreationMethodIfRequired(Type? type)
        {
            if (InstanceCreationMethods.ContainsKey(type))
            {
                return;
            }

            var argumentTypes = new[]
            {
                typeof(TArg1),
                typeof(TArg2),
                typeof(TArg3)
            };

            var constructorArgumentTypes =
                argumentTypes.Where(t => t != typeof(TypeToIgnore)).ToArray();
            var constructor = type?.GetConstructor(
                BindingFlags.Instance | BindingFlags.Public, null,
                CallingConventions.HasThis, constructorArgumentTypes,
                Array.Empty<ParameterModifier>());

            var lamdaParameterExpressions = new[]
            {
                Expression.Parameter(typeof(TArg1), "param1"),
                Expression.Parameter(typeof(TArg2), "param2"),
                Expression.Parameter(typeof(TArg3), "param3")
            };

            var constructorParameterExpressions = lamdaParameterExpressions
                .Take(constructorArgumentTypes.Length).ToArray();
            if (constructor == null)
            {
                return;
            }

            var constructorCallExpression =
                Expression.New(constructor, constructorParameterExpressions);
            var constructorCallingLambda = Expression
                .Lambda<Func<TArg1?, TArg2?, TArg3?, TObject?>>(constructorCallExpression,
                    lamdaParameterExpressions).Compile();
            InstanceCreationMethods[type] = constructorCallingLambda;
        }
    }

    #endregion 创建实例
}