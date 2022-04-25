using System;
using System.Collections.Generic;
using Palink.Tools.IO;
using Palink.Tools.Logging;

namespace Palink.Tools.Freebus.Interface;

public interface IFreebusTransport : IDisposable
{
    int Retries { get; set; }

    int WaitToRetryMilliseconds { get; set; }

    int ReadTimeout { get; set; }

    int WriteTimeout { get; set; }

    IFreebusMessage UnicastMessage(IFreebusMessage message);

    byte[] BuildMessageFrame(IFreebusMessage message);

    void Write(IFreebusMessage message);

    IStreamResource StreamResource { get; }

    List<string> IgnoreList { get; set; }

    IFreebusLogger Logger { get; set; }
}