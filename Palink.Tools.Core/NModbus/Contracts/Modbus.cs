﻿namespace Palink.Tools.NModbus.Contracts;

internal static class Modbus
{
    public const int MaximumDiscreteRequestResponseSize = 2040;
    public const int MaximumRegisterRequestResponseSize = 127;

    // modbus slave exception offset that is added to the function code, to flag an exception
    public const byte ExceptionOffset = 128;

    // default setting for number of retries for IO operations
    public const int DefaultRetries = 3;

    // default number of milliseconds to wait after encountering an ACKNOWLEDGE or SLAVE DEVICE BUSY slave exception response.
    public const int DefaultWaitToRetryMilliseconds = 250;

    // default setting for IO timeouts in milliseconds
    //public const int DefaultTimeout = 1000;


    public const ushort CoilOn = 0xFF00;
    public const ushort CoilOff = 0x0000;

    // IP slaves should be addressed by IP
    public const byte DefaultIpSlaveUnitId = 0;

    //// An existing connection was forcibly closed by the remote host
    //public const int ConnectionResetByPeer = 10054;

    // Existing socket connection is being closed
    //public const int WSACancelBlockingCall = 10004;

    // used by the ASCII transport to indicate end of message
    public const string NewLine = "\r\n";
}