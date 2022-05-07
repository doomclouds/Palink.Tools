using System;
using System.IO.Ports;
using Palink.Tools.IO;
using Palink.Tools.Utility;

namespace Palink.Tools;

/// <summary>
///     Concrete Implementor - http://en.wikipedia.org/wiki/Bridge_Pattern
/// </summary>
public class SerialPortAdapter : IStreamResource
{
    private const string NewLine = "\r\n";

    private readonly SerialPort _serialPort;

    /// <summary>
    /// SerialPortAdapter
    /// </summary>
    /// <param name="serialPort"></param>
    public SerialPortAdapter(SerialPort serialPort)
    {
        _serialPort = serialPort;
        _serialPort.NewLine = NewLine;
    }

    /// <summary>
    /// InfiniteTimeout
    /// </summary>
    public int InfiniteTimeout => SerialPort.InfiniteTimeout;

    /// <summary>
    /// ReadTimeout
    /// </summary>
    public int ReadTimeout
    {
        get => _serialPort.ReadTimeout;
        set => _serialPort.ReadTimeout = value;
    }

    /// <summary>
    /// WriteTimeout
    /// </summary>
    public int WriteTimeout
    {
        get => _serialPort.WriteTimeout;
        set => _serialPort.WriteTimeout = value;
    }

    /// <summary>
    /// DiscardInBuffer
    /// </summary>
    public void DiscardInBuffer()
    {
        _serialPort.DiscardInBuffer();
    }

    /// <summary>
    /// Read
    /// </summary>
    /// <param name="buffer"></param>
    /// <param name="offset"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    public int Read(byte[] buffer, int offset, int count)
    {
        return _serialPort.Read(buffer, offset, count);
    }

    /// <summary>
    /// Write
    /// </summary>
    /// <param name="buffer"></param>
    /// <param name="offset"></param>
    /// <param name="count"></param>
    public void Write(byte[] buffer, int offset, int count)
    {
        _serialPort.Write(buffer, offset, count);
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
            CoreTool.Dispose(_serialPort);
        }
    }
}