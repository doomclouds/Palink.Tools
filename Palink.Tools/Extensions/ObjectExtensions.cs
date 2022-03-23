using System;

namespace Palink.Tools.Extensions
{
    /// <summary>
    /// 对象判断扩展类
    /// </summary>
    public static class ObjectExtensions
    {
        /// <summary>
        /// 判断对象是否为空
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static bool IsNull(this object obj)
        {
            return obj == null;
        }

        /// <summary>
        /// 判断对象是否不为空
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static bool IsNotNull(this object obj)
        {
            return obj != null;
        }

        /// <summary>
        /// 对象为空则抛出异常
        /// </summary>
        /// <param name="obj"></param>
        public static void ThrowIfNull(this object obj)
        {
            if (obj == null)
            {
                throw new ArgumentNullException(nameof(obj));
            }
        }
    }
}