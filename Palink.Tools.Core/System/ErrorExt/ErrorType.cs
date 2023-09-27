namespace Palink.Tools.System.ErrorExt;

public enum ErrorType
{
    /// <summary>
    /// 预期之外的异常
    /// </summary>
    Unexpected,

    /// <summary>
    /// 严重异常，程序无法运行
    /// </summary>
    Serious,

    /// <summary>
    /// 可处理的异常
    /// </summary>
    Handleable
}