using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using Palink.Tools.Communication.Interface;
using RJCP.IO.Ports;

namespace Palink.Tools.Communication.Adapter;

/// <summary>
/// SerialPortAdapter
/// </summary>
public class SerialPortAdapter : IStreamResource
{
    private const string NewLine = "\r\n";

    [NotNull]
    private SerialPortStream? _serialPortStream;

    /// <summary>
    /// SerialPortAdapter
    /// </summary>
    /// <param name="serialPortStream"></param>
    public SerialPortAdapter(SerialPortStream serialPortStream)
    {
        _serialPortStream = serialPortStream;
        _serialPortStream.NewLine = NewLine;
    }

    /// <summary>
    /// InfiniteTimeout
    /// </summary>
    public int InfiniteTimeout => Timeout.Infinite;

    /// <summary>
    /// 读数据超时时间
    /// </summary>
    public int ReadTimeout
    {
        get => _serialPortStream.ReadTimeout;
        set => _serialPortStream.ReadTimeout = value;
    }

    /// <summary>
    /// 写数据超时时间
    /// </summary>
    public int WriteTimeout
    {
        get => _serialPortStream.WriteTimeout;
        set => _serialPortStream.WriteTimeout = value;
    }

    /// <summary>
    /// 清空缓存
    /// </summary>
    public void DiscardInBuffer()
    {
        _serialPortStream.DiscardInBuffer();
    }

    /// <summary>
    /// 读取数据
    /// </summary>
    /// <param name="buffer"></param>
    /// <param name="offset"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    public int Read(byte[] buffer, int offset, int count)
    {
        return _serialPortStream.Read(buffer, offset, count);
    }

    /// <summary>
    /// 写数据
    /// </summary>
    /// <param name="buffer"></param>
    /// <param name="offset"></param>
    /// <param name="count"></param>
    public void Write(byte[] buffer, int offset, int count)
    {
        _serialPortStream.Write(buffer, offset, count);
    }

    /// <summary>
    /// 释放对象
    /// </summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// 释放对象
    /// </summary>
    /// <param name="disposing"></param>
    protected virtual void Dispose(bool disposing)
    {
        if (!disposing)
        {
            return;
        }

        _serialPortStream.Dispose();
        _serialPortStream = default;
    }
}