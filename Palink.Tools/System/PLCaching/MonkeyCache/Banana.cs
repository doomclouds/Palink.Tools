using System;
using SQLite;

#nullable disable
namespace Palink.Tools.System.PLCaching.MonkeyCache;

/// <summary>
/// Data object for Barrel
/// </summary>
internal class Banana
{
    /// <summary>
    /// Unique Identifier
    /// </summary>
    [PrimaryKey]
    public string Id { get; set; }


    /// <summary>
    /// Additional ETag to set for Http Caching
    /// </summary>
    public string ETag { get; set; }

    /// <summary>
    /// Main Contents.
    /// </summary>
    public string Contents { get; set; }

    /// <summary>
    /// Expiration data of the object, stored in UTC
    /// </summary>
    public DateTime ExpirationDate { get; set; }
}