using System;
using System.Collections.Generic;
using Palink.Tools.IO;
using Palink.Tools.Logging;

namespace Palink.Tools.Freebus.Interface;

public interface IFreebusTransport : IDisposable
{
    ushort Retries { get; set; }

    ushort WaitToRetryMilliseconds { get; set; }

    int ReadTimeout { get; set; }

    int WriteTimeout { get; set; }

    IFreebusContext UnicastMessage(IFreebusContext context);

    void BroadcastMessage(IFreebusContext context, bool shouldLog);

    byte[] BuildMessageFrame(IFreebusContext context);

    void Write(IFreebusContext context);

    IStreamResource StreamResource { get; }

    List<string> IgnoreList { get; set; }

    string? ErrorMessage { get; set; }

    IFreebusLogger Logger { get; set; }
}