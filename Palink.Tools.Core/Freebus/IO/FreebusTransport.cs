using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Palink.Tools.Extensions.StringExt;
using Palink.Tools.Freebus.Contracts;
using Palink.Tools.Freebus.Interface;
using Palink.Tools.IO;
using Palink.Tools.Logging;
using Palink.Tools.Utility;

namespace Palink.Tools.Freebus.IO;

public abstract class FreebusTransport : IFreebusTransport
{
    private readonly object _syncLock = new();

    protected FreebusTransport(IStreamResource streamResource, IFreebusLogger logger)
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

    public virtual IFreebusContext UnicastMessage(IFreebusContext context)
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
                    Write(context);

                    bool readAgain;
                    do
                    {
                        readAgain = false;
                        ReadResponse(context);

                        if (ShouldRetryResponse(context))
                        {
                            readAgain = true;
                        }
                    } while (readAgain);
                }

                if (!ValidateResponse(context))
                {
                    attempt++;
                    Logger.Error(
                        "The response data does not match the success condition");
                    continue;
                }

                context.Succeed = true;
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

        return context;
    }

    public void BroadcastMessage(IFreebusContext context, bool shouldLog)
    {
        if (shouldLog)
        {
            Logger.LogFrameTx(context.Pdu);
        }

        StreamResource.Write(context.Pdu, 0, context.Pdu.Length);
    }

    public virtual bool ShouldRetryResponse(IFreebusContext context)
    {
        return false;
    }

    public virtual bool ValidateResponse(IFreebusContext context)
    {
        return true;
    }

    public IFreebusContext ReadResponse(IFreebusContext context)
    {
        var frame = Array.Empty<byte>();
        if (context.DruLength.HasValue)
        {
            frame = frame.Concat(Read(context.DruLength.Value)).ToArray();
        }
        else if (!context.NewLine.IsNullOrEmpty())
        {
            frame = frame.Concat(ReadLine(context.NewLine)).ToArray();
        }

        context.Dru = frame;

        if (ShouldRetryResponse(context))
        {
            if (context.NewLine.IsNullOrEmpty() || context.IsWriteHexLog)
            {
                Logger.LogFrameIgnoreRx(context.Dru);
            }
            else
            {
                Logger.LogFrameIgnoreRx(context.GetDruString()
                    .Replace(context.NewLine, ""));
            }
        }
        else
        {
            if (context.NewLine.IsNullOrEmpty() || context.IsWriteHexLog)
            {
                Logger.LogFrameRx(frame);
            }
            else
            {
                Logger.LogFrameRx(context.GetDruString().Replace(context.NewLine, ""));
            }
        }

        return context;
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

    public void Write(IFreebusContext context)
    {
        DiscardInBuffer();

        var frame = BuildMessageFrame(context);

        if (context.NewLine.IsNullOrEmpty())
        {
            Logger.LogFrameTx(frame);
        }
        else
        {
            Logger.LogFrameTx(context.GetPduString().Replace(context.NewLine, ""));
        }

        StreamResource.Write(frame, 0, frame.Length);
    }

    public byte[] BuildMessageFrame(IFreebusContext context)
    {
        return context.Pdu;
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

    public abstract string? ErrorMessage { get; set; }

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