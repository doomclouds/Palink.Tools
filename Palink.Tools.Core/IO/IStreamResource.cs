using System;

namespace Palink.Tools.IO;

/// <summary>
/// StreamResource
/// </summary>
public interface IStreamResource : IDisposable
{
    /// <summary>
    /// InfiniteTimeout
    /// </summary>
    int InfiniteTimeout { get; }

    /// <summary>
    /// 读数据超时时间
    /// </summary>
    int ReadTimeout { get; set; }

    /// <summary>
    /// 写数据超时时间
    /// </summary>
    int WriteTimeout { get; set; }

    /// <summary>
    /// 清空缓存
    /// </summary>
    void DiscardInBuffer();

    /// <summary>
    /// 读取数据
    /// </summary>
    /// <param name="buffer"></param>
    /// <param name="offset"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    int Read(byte[] buffer, int offset, int count);

    /// <summary>
    /// 写数据
    /// </summary>
    /// <param name="buffer"></param>
    /// <param name="offset"></param>
    /// <param name="count"></param>
    void Write(byte[] buffer, int offset, int count);
}