using System;
using System.Net.Sockets;
using System.Threading;
using Palink.Tools.Utility;

namespace Palink.Tools.IO;

/// <summary>
///     Concrete Implementor - http://en.wikipedia.org/wiki/Bridge_Pattern
///     This implementation is for sockets that Convert Rs485 to Ethernet.
/// </summary>
public class SocketAdapter : IStreamResource
{
    private readonly Socket _socketClient;

    /// <summary>
    /// SocketAdapter
    /// </summary>
    /// <param name="socketClient"></param>
    public SocketAdapter(Socket socketClient)
    {
        _socketClient = socketClient;
    }

    /// <summary>
    /// InfiniteTimeout
    /// </summary>
    public int InfiniteTimeout => Timeout.Infinite;

    /// <summary>
    /// ReadTimeout
    /// </summary>
    public int ReadTimeout
    {
        get => _socketClient.ReceiveTimeout;
        set => _socketClient.ReceiveTimeout = value;
    }

    /// <summary>
    /// WriteTimeout
    /// </summary>
    public int WriteTimeout
    {
        get => _socketClient.SendTimeout;
        set => _socketClient.SendTimeout = value;
    }

    /// <summary>
    /// DiscardInBuffer
    /// </summary>
    public void DiscardInBuffer()
    {
        var temp = ReadTimeout;
        ReadTimeout = 5;
        try
        {
            var maxTime = 1024;
            var buffer = new byte[1];
            while (_socketClient.Receive(buffer) != 0 && maxTime > 0)
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
    /// Read
    /// </summary>
    /// <param name="buffer"></param>
    /// <param name="offset"></param>
    /// <param name="size"></param>
    /// <returns></returns>
    public int Read(byte[] buffer, int offset, int size)
    {
        return _socketClient.Receive(buffer, offset, size, 0);
    }

    /// <summary>
    /// Write
    /// </summary>
    /// <param name="buffer"></param>
    /// <param name="offset"></param>
    /// <param name="size"></param>
    public void Write(byte[] buffer, int offset, int size)
    {
        _socketClient.Send(buffer, offset, size, 0);
    }


    /// <summary>
    /// Dispose
    /// </summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Dispose
    /// </summary>
    /// <param name="disposing"></param>
    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            CoreTool.Dispose(_socketClient);
        }
    }
}