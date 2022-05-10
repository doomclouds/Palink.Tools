using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Palink.Tools.Extensions.PLString;
using Palink.Tools.Freebus.Contracts;
using Palink.Tools.Freebus.Interface;
using Palink.Tools.IO;
using Palink.Tools.Logging;
using Palink.Tools.Utility;

namespace Palink.Tools.Freebus.IO;

public abstract class FreebusTransport : IFreebusTransport
{
    private readonly object _syncLock = new();

    internal FreebusTransport(IStreamResource streamResource, IFreebusLogger logger)
    {
        Logger = logger ?? throw new ArgumentNullException(nameof(logger));
        StreamResource = streamResource ??
                         throw new ArgumentNullException(nameof(streamResource));
        StreamResource.ReadTimeout = FreebusContracts.DefaultWaitToRetryMilliseconds;
        StreamResource.WriteTimeout = FreebusContracts.DefaultWaitToRetryMilliseconds;
    }

    public ushort Retries { get; set; } = FreebusContracts.DefaultRetries;

    public ushort WaitToRetryMilliseconds { get; set; } =
        FreebusContracts.DefaultWaitToRetryMilliseconds;

    public int ReadTimeout
    {
        get => StreamResource.ReadTimeout;
        set => StreamResource.ReadTimeout = value;
    }

    public int WriteTimeout
    {
        get => StreamResource.WriteTimeout;
        set => StreamResource.WriteTimeout = value;
    }

    public virtual IFreebusMessage UnicastMessage(IFreebusMessage message)
    {
        var attempt = 1;
        var success = false;

        do
        {
            try
            {
                if (attempt > Retries)
                    break;

                lock (_syncLock)
                {
                    Write(message);

                    bool readAgain;
                    do
                    {
                        readAgain = false;
                        ReadResponse(message);

                        if (ShouldRetryResponse(message))
                        {
                            readAgain = true;
                        }
                    } while (readAgain);
                }

                if (!ValidateResponse(message))
                {
                    attempt++;
                    Logger.Error(
                        "The response data does not match the success condition");
                    continue;
                }

                message.Succeed = true;
                success = true;
            }
            catch (Exception e)
            {
                Logger.Error(
                    $"{e.GetType().Name}, {(Retries - attempt + 1)} retries remaining - {e}");

                if (attempt++ > Retries)
                {
                    throw;
                }

                Sleep(WaitToRetryMilliseconds);
            }
        } while (!success);

        return message;
    }

    public void BroadcastMessage(IFreebusMessage message, bool shouldLog)
    {
        if (shouldLog)
        {
            Logger.LogFrameTx(message.Pdu);
        }

        StreamResource.Write(message.Pdu, 0, message.Pdu.Length);
    }

    public virtual bool ShouldRetryResponse(IFreebusMessage message)
    {
        return false;
    }

    public virtual bool ValidateResponse(IFreebusMessage message)
    {
        return true;
    }

    public IFreebusMessage ReadResponse(IFreebusMessage message)
    {
        var frame = Array.Empty<byte>();
        if (message.DruLength.HasValue)
        {
            frame = frame.Concat(Read(message.DruLength.Value)).ToArray();
        }
        else if (!message.NewLine.IsNullOrEmpty())
        {
            frame = frame.Concat(ReadLine(message.NewLine)).ToArray();
        }

        message.Dru = frame;

        if (ShouldRetryResponse(message))
        {
            if (message.NewLine.IsNullOrEmpty())
            {
                Logger.LogFrameIgnoreRx(message.Dru);
            }
            else
            {
                Logger.LogFrameIgnoreRx(message.GetDruString()
                    .Replace(message.NewLine, ""));
            }
        }
        else
        {
            if (message.NewLine.IsNullOrEmpty())
            {
                Logger.LogFrameRx(frame);
            }
            else
            {
                Logger.LogFrameRx(message.GetDruString().Replace(message.NewLine, ""));
            }
        }


        return message;
    }

    public byte[] Read(int count)
    {
        var frameBytes = new byte[count];
        var numBytesRead = 0;

        while (numBytesRead != count)
        {
            numBytesRead +=
                StreamResource.Read(frameBytes, numBytesRead, count - numBytesRead);
        }

        return frameBytes;
    }

    public byte[] ReadLine(string newLine)
    {
        var frameString = CoreTool.ReadLine(StreamResource, newLine);
        var frame = Encoding.UTF8.GetBytes(frameString);
        // Logger.LogFrameRx(frame);

        return frame;
    }

    public void Write(IFreebusMessage message)
    {
        DiscardInBuffer();

        var frame = BuildMessageFrame(message);

        if (message.NewLine.IsNullOrEmpty())
        {
            Logger.LogFrameTx(frame);
        }
        else
        {
            Logger.LogFrameTx(message.GetPduString().Replace(message.NewLine, ""));
        }


        StreamResource.Write(frame, 0, frame.Length);
    }

    public byte[] BuildMessageFrame(IFreebusMessage message)
    {
        return message.Pdu;
    }

    public void DiscardInBuffer()
    {
        StreamResource.DiscardInBuffer();
    }

    /// <summary>
    ///     Gets the stream resource.
    /// </summary>
    public IStreamResource StreamResource { get; }

    public abstract List<string> IgnoreList { get; set; }

    /// <summary>
    /// Gets the logger for this instance.
    /// </summary>
    public IFreebusLogger Logger { get; set; }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            CoreTool.Dispose(StreamResource);
        }
    }

    private static void Sleep(int millisecondsTimeout)
    {
        Task.Delay(millisecondsTimeout).Wait();
    }
}