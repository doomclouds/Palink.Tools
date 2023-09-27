using System.Collections.Generic;
using System.Linq;

namespace Palink.Tools.System.ErrorExt;

public readonly record struct ErrorOr<TValue> : IErrorOr
{
    private readonly TValue? _value = default;
    private readonly List<Error> _errors = new();

    /// <summary>
    /// 是否发生异常
    /// </summary>
    public bool IsError { get; }

    /// <summary>
    /// 异常列表
    /// </summary>
    public List<Error> Errors => IsError ? _errors : new List<Error>();

    /// <summary>
    /// 通过异常列表获取ErrorOr
    /// </summary>
    public static ErrorOr<TValue> From(List<Error> errors)
    {
        return errors;
    }

    public TValue Value => _value!;

    /// <summary>
    /// 获取第一个异常
    /// </summary>
    public Error? FirstError => _errors.Any() ? _errors.First() : default;

    private ErrorOr(Error error)
    {
        _errors = new List<Error> { error };
        IsError = true;
    }

    private ErrorOr(List<Error> errors)
    {
        _errors = errors;
        IsError = true;
    }

    private ErrorOr(TValue value)
    {
        _value = value;
        IsError = false;
    }

    /// <summary>
    /// 通过实体创建ErrorOr
    /// </summary>
    /// <param name="value"></param>
    public static implicit operator ErrorOr<TValue>(TValue value)
    {
        return new ErrorOr<TValue>(value);
    }

    /// <summary>
    /// 通过Error创建ErrorOr
    /// </summary>
    /// <param name="error"></param>
    public static implicit operator ErrorOr<TValue>(Error error)
    {
        return new ErrorOr<TValue>(error);
    }

    /// <summary>
    /// 通过Error集合创建ErrorOr
    /// </summary>
    public static implicit operator ErrorOr<TValue>(List<Error> errors)
    {
        return new ErrorOr<TValue>(errors);
    }

    /// <summary>
    /// 通过Error数组创建ErrorOr
    /// </summary>
    public static implicit operator ErrorOr<TValue>(Error[] errors)
    {
        return new ErrorOr<TValue>(errors.ToList());
    }
}

/// <summary>
/// Provides utility methods for creating instances of <see ref="ErrorOr{T}"/>.
/// </summary>
public static class ErrorOr
{
    /// <summary>
    /// Creates an <see ref="ErrorOr{TValue}"/> instance from a value.
    /// </summary>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <param name="value">The value from which to create an ErrorOr instance.</param>
    /// <returns>An <see ref="ErrorOr{TValue}"/> instance containing the specified value.</returns>
    public static ErrorOr<TValue> From<TValue>(TValue value)
    {
        return value;
    }
}