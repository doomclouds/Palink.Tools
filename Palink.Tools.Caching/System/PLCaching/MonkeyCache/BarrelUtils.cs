using System;
using System.IO;

namespace Palink.Tools.System.PLCaching.MonkeyCache;

/// <summary>
/// Barrel Utils
/// </summary>
public static class BarrelUtils
{
    internal static string? BasePath;

    /// <summary>
    /// Sets the base path to use. This can only be set once and before using the Barrel
    /// </summary>
    public static void SetBaseCachePath(string path)
    {
        if (!string.IsNullOrWhiteSpace(BasePath))
            throw new InvalidOperationException(
                "You can only set the base cache path once before using the Barrel.");

        BasePath = path;
    }

    internal static bool IsString<T>(T item)
    {
        var typeOf = typeof(T);
        if (typeOf.IsGenericType &&
            typeOf.GetGenericTypeDefinition() == typeof(Nullable<>))
        {
            typeOf = Nullable.GetUnderlyingType(typeOf);
        }

        var typeCode = Type.GetTypeCode(typeOf);
        return typeCode == TypeCode.String;
    }

    internal static string GetBasePath(string applicationId)
    {
        if (string.IsNullOrWhiteSpace(applicationId))
            throw new ArgumentException(
                "You must set a ApplicationId for MonkeyCache by using Barrel.ApplicationId.");

        if (applicationId.IndexOfAny(Path.GetInvalidPathChars()) != -1)
            throw new ArgumentException("ApplicationId has invalid characters");

        if (string.IsNullOrWhiteSpace(BasePath))
        {
            // Gets full path based on device type.
            BasePath =
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        }

        return Path.Combine(BasePath, applicationId);
    }

    /// <summary>
    /// Gets the expiration from a timespan
    /// </summary>
    /// <param name="timeSpan"></param>
    /// <returns></returns>
    internal static DateTime GetExpiration(TimeSpan timeSpan)
    {
        try
        {
            return DateTime.UtcNow.Add(timeSpan);
        }
        catch
        {
            return timeSpan.Milliseconds < 0 ? DateTime.MinValue : DateTime.MaxValue;
        }
    }
}