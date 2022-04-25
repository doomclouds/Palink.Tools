namespace Palink.Tools.Utility;

/// <summary>
/// 鼠标状态
/// </summary>
internal enum MouseStatus
{
    /// <summary>
    /// 移动
    /// </summary>
    Move = 0x200,

    /// <summary>
    /// 左键按下
    /// </summary>
    LeftDown = 0x201,

    /// <summary>
    /// 左键抬起
    /// </summary>
    LeftUp = 0x202,

    /// <summary>
    /// 右键按下
    /// </summary>
    RightDown = 0x204,

    /// <summary>
    /// 右键抬起
    /// </summary>
    RightUp = 0x205,
}