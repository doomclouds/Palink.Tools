using System;
using System.ComponentModel;

namespace Palink.Tools.Extensions.PLAttribute;

/// <summary>
/// 特性扩展方法
/// </summary>
public static class AttributeExtensions
{
    /// <summary>
    /// 获取枚举描述信息
    /// </summary>
    /// <param name="enumValue"></param>
    /// <returns></returns>
    public static string EnumDescription(this Enum enumValue)
    {
        var value = enumValue.ToString();
        var field = enumValue.GetType().GetField(value);
        var obj =
            field?.GetCustomAttributes(typeof(DescriptionAttribute), false); //获取描述属性
        if (obj == null || obj.Length == 0) //当描述属性没有时，直接返回名称
            return value;
        var descriptionAttribute = (DescriptionAttribute)obj[0];
        return descriptionAttribute.Description;
    }
}