using System;
using System.IO;
using System.Linq;
using System.Net;
using Palink.Tools.Extensions.ArrayExt;
using Palink.Tools.IO;
using Palink.Tools.Logging;
using Palink.Tools.NModbus.Interfaces;

namespace Palink.Tools.NModbus.IO;

/// <summary>
///     Transport for Internet protocols.
///     Refined Abstraction - http://en.wikipedia.org/wiki/Bridge_Pattern
/// </summary>
internal class ModbusIpTransport : ModbusTransport
{
    private static readonly object TransactionIdLock = new();
    private ushort _transactionId;

    internal ModbusIpTransport(IStreamResource streamResource,
        IModbusFactory modbusFactory, IFreebusLogger logger)
        : base(streamResource, modbusFactory, logger)
    {
        if (streamResource == null)
            throw new ArgumentNullException(nameof(streamResource));
    }

    internal static byte[] ReadRequestResponse(IStreamResource streamResource,
        IFreebusLogger logger)
    {
        if (streamResource == null)
            throw new ArgumentNullException(nameof(streamResource));
        if (logger == null) throw new ArgumentNullException(nameof(logger));

        // read header
        var header = new byte[6];
        var numBytesRead = 0;

        while (numBytesRead != 6)
        {
            var bRead = streamResource.Read(header, numBytesRead, 6 - numBytesRead);

            if (bRead == 0)
            {
                throw new IOException("Read resulted in 0 bytes returned.");
            }

            numBytesRead += bRead;
        }

        logger.Debug($"MBAP header: {string.Join(", ", header)}");
        var frameLength =
            (ushort)IPAddress.HostToNetworkOrder(BitConverter.ToInt16(header, 4));
        logger.Debug($"{frameLength} bytes in PDU.");

        // read message
        var messageFrame = new byte[frameLength];
        numBytesRead = 0;

        while (numBytesRead != frameLength)
        {
            var bRead = streamResource.Read(messageFrame, numBytesRead,
                frameLength - numBytesRead);

            if (bRead == 0)
            {
                throw new IOException("Read resulted in 0 bytes returned.");
            }

            numBytesRead += bRead;
        }

        logger.Debug($"PDU: {frameLength}");
        var frame = header.Concat(messageFrame).ToArray();
        logger.LogFrameRx(frame);

        return frame;
    }

    internal static byte[] GetMbapHeader(IModbusMessage message)
    {
        var transactionId =
            BitConverter.GetBytes(
                IPAddress.HostToNetworkOrder((short)message.TransactionId));
        var length = BitConverter.GetBytes(
            IPAddress.HostToNetworkOrder((short)(message.ProtocolDataUnit.Length + 1)));

        var stream = new MemoryStream(7);
        stream.Write(transactionId, 0, transactionId.Length);
        stream.WriteByte(0);
        stream.WriteByte(0);
        stream.Write(length, 0, length.Length);
        stream.WriteByte(message.SlaveAddress);

        return stream.ToArray();
    }

    /// <summary>
    ///     Create a new transaction ID.
    /// </summary>
    internal virtual ushort GetNewTransactionId()
    {
        lock (TransactionIdLock)
        {
            _transactionId = _transactionId == ushort.MaxValue ? (ushort)1
                : ++_transactionId;
        }

        return _transactionId;
    }

    internal IModbusMessage CreateMessageAndInitializeTransactionId<T>(byte[] fullFrame)
        where T : IModbusMessage, new()
    {
        var mbapHeader = fullFrame.Slice(0, 6).ToArray();
        var messageFrame = fullFrame.Slice(6, fullFrame.Length - 6).ToArray();

        var response = CreateResponse<T>(messageFrame);
        response.TransactionId =
            (ushort)IPAddress.NetworkToHostOrder(BitConverter.ToInt16(mbapHeader, 0));

        return response;
    }

    public override byte[] BuildMessageFrame(IModbusMessage message)
    {
        var header = GetMbapHeader(message);
        var pdu = message.ProtocolDataUnit;
        var messageBody = new MemoryStream(header.Length + pdu.Length);

        messageBody.Write(header, 0, header.Length);
        messageBody.Write(pdu, 0, pdu.Length);

        return messageBody.ToArray();
    }

    public override void Write(IModbusMessage message)
    {
        message.TransactionId = GetNewTransactionId();
        var frame = BuildMessageFrame(message);

        Logger.LogFrameTx(frame);

        StreamResource.Write(frame, 0, frame.Length);
    }

    public override byte[] ReadRequest()
    {
        return ReadRequestResponse(StreamResource, Logger);
    }

    public override IModbusMessage ReadResponse<T>()
    {
        return CreateMessageAndInitializeTransactionId<T>(
            ReadRequestResponse(StreamResource, Logger));
    }

    internal override void OnValidateResponse(IModbusMessage request,
        IModbusMessage response)
    {
        if (request.TransactionId != response.TransactionId)
        {
            var msg =
                $"Response was not of expected transaction ID. Expected {request.TransactionId}, " +
                $"received {response.TransactionId}.";
            throw new IOException(msg);
        }
    }

    public override bool OnShouldRetryResponse(IModbusMessage request,
        IModbusMessage response)
    {
        if (request.TransactionId > response.TransactionId &&
            request.TransactionId - response.TransactionId < RetryOnOldResponseThreshold)
        {
            // This response was from a previous request
            return true;
        }

        return base.OnShouldRetryResponse(request, response);
    }
}