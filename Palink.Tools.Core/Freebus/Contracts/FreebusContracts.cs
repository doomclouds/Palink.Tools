namespace Palink.Tools.Freebus.Contracts;

public static class FreebusContracts
{
    // default setting for number of retries for IO operations
    public const int DefaultRetries = 3;

    // default number of milliseconds to wait after encountering an ACKNOWLEDGE or SLAVE DEVICE BUSY slave exception response.
    public const int DefaultWaitToRetryMilliseconds = 250;
}