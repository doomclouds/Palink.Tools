namespace Palink.Tools.Extensions.ArrayExt;

/// <summary>
/// 对象相似，并不一定是同一对象
/// </summary>
public interface IObjectLike
{
    /// <summary>
    /// 判断对象是否相似
    /// 判断依据是其属性是否符合某种规则
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    bool Likes(IObjectLike other);
}