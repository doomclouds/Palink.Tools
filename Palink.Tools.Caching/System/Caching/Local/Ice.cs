using System;
using SQLite;

namespace Palink.Tools.System.Caching.Local;

/// <summary>
/// Data object for IceStorage
/// </summary>
internal class Ice
{
    /// <summary>
    /// Unique Identifier
    /// </summary>
    [PrimaryKey]
    [global::System.Diagnostics.CodeAnalysis.NotNull]
    public string? Id { get; set; }


    /// <summary>
    /// Additional ETag to set for Http Caching
    /// </summary>
    public string? ETag { get; set; }

    /// <summary>
    /// Main Contents.
    /// </summary>
    [global::System.Diagnostics.CodeAnalysis.NotNull]
    public string? Contents { get; set; }

    /// <summary>
    /// Expiration data of the object, stored in UTC
    /// </summary>
    public DateTime ExpirationDate { get; set; }
}