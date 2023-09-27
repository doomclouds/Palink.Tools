namespace Palink.Tools.System.ErrorExt;

public readonly record struct Error
{
    private Error(string code, string description, ErrorType type)
    {
        Code = code;
        Description = description;
        Type = type;
        NumericType = (int)type;
    }

    /// <summary>
    /// 错误码
    /// </summary>
    public string Code { get; } = string.Empty;

    /// <summary>
    /// 错误描述
    /// </summary>
    public string Description { get; } = string.Empty;

    /// <summary>
    /// 错误级别
    /// </summary>
    public ErrorType Type { get; }

    /// <summary>
    /// 错误级别的值
    /// </summary>
    public int NumericType { get; }

    public static Error Unexpected(
        string code = "General.Unexpected",
        string description = "预期之外的异常") =>
        new(code, description, ErrorType.Unexpected);

    public static Error Serious(
        string code = "General.Serious",
        string description = "严重异常，程序无法运行") =>
        new(code, description, ErrorType.Serious);

    public static Error Handleable(
        string code = "General.Handleable",
        string description = "可处理的异常") =>
        new(code, description, ErrorType.Handleable);

    /// <summary>
    /// 自定义异常
    /// </summary>
    /// <param name="type"></param>
    /// <param name="code"></param>
    /// <param name="description"></param>
    /// <returns></returns>
    public static Error Custom(
        int type,
        string code,
        string description) =>
        new(code, description, (ErrorType)type);
}