using System;

namespace Palink.Tools.Utility;

/// <summary>
/// DisposableUtility
/// </summary>
public partial class CoreTool
{
    /// <summary>
    /// Dispose
    /// </summary>
    /// <param name="item"></param>
    /// <typeparam name="T"></typeparam>
    public static void Dispose<T>(T? item)
        where T : class, IDisposable
    {
        item?.Dispose();
    }
}