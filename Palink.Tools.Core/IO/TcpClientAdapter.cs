﻿using System;
using System.Net.Sockets;
using System.Threading;
using Palink.Tools.Utility;

namespace Palink.Tools.IO;

/// <summary>
/// TcpClientAdapter
/// </summary>
public class TcpClientAdapter : IStreamResource
{
    private readonly TcpClient _tcpClient;

    /// <summary>
    /// TcpClientAdapter
    /// </summary>
    /// <param name="tcpClient"></param>
    public TcpClientAdapter(TcpClient tcpClient)
    {
        _tcpClient = tcpClient;
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
        get => _tcpClient.GetStream().ReadTimeout;
        set => _tcpClient.GetStream().ReadTimeout = value;
    }

    /// <summary>
    /// 写数据超时时间
    /// </summary>
    public int WriteTimeout
    {
        get => _tcpClient.GetStream().WriteTimeout;
        set => _tcpClient.GetStream().WriteTimeout = value;
    }

    /// <summary>
    /// 写数据
    /// </summary>
    /// <param name="buffer"></param>
    /// <param name="offset"></param>
    /// <param name="size"></param>
    public void Write(byte[] buffer, int offset, int size)
    {
        _tcpClient.GetStream().Write(buffer, offset, size);
    }

    /// <summary>
    /// 读取数据
    /// </summary>
    /// <param name="buffer"></param>
    /// <param name="offset"></param>
    /// <param name="size"></param>
    /// <returns></returns>
    public int Read(byte[] buffer, int offset, int size)
    {
        return _tcpClient.GetStream().Read(buffer, offset, size);
    }

    /// <summary>
    /// 清空缓存
    /// </summary>
    public void DiscardInBuffer()
    {
        var temp = ReadTimeout;
        ReadTimeout = 5;
        try
        {
            var maxTime = 1024;
            var buffer = new byte[1];
            while (_tcpClient.GetStream().Read(buffer, 0, 1) != 0 && maxTime > 0)
            {
                maxTime--;
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }

        ReadTimeout = temp;
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
    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            CoreTool.Dispose(_tcpClient);
        }
    }
}