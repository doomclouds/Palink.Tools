using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using SQLite;

namespace Palink.Tools.System.Caching.Local;

/// <summary>
/// Key/Value data store for any data object.
/// Allows for saving data along with expiration dates and ETags.
/// </summary>
public class IceStorage : IIceStorage
{
    /// <summary>
    /// Path identifier, combined with the base path to synthesize the full path
    /// </summary>
    public static string ApplicationId { get; set; } = string.Empty;

    private static readonly Lazy<string> BaseCacheDir = new(() =>
        Path.Combine(IceStorageUtils.GetBasePath(ApplicationId), "PalinkStorage"));

    private readonly SQLiteConnection _db;
    private readonly object _dbLock = new();

    /// <summary>
    /// AutoExpire
    /// </summary>
    public bool AutoExpire { get; set; }

    private static IceStorage? _instance;

    /// <summary>
    /// Gets the instance of the IceStorage
    /// </summary>
    public static IIceStorage Current => _instance ??= new IceStorage();

    /// <summary>
    /// Specify a path to create the cache database
    /// </summary>
    /// <param name="cacheDirectory"></param>
    /// <returns></returns>
    public static IIceStorage Create(string cacheDirectory) =>
        new IceStorage(cacheDirectory);

    private readonly JsonSerializerSettings _jsonSettings;

    private IceStorage(string? cacheDirectory = null)
    {
        var directory = string.IsNullOrEmpty(cacheDirectory)
            ? BaseCacheDir.Value
            : cacheDirectory;
        var path = Path.Combine(directory, "IceStorage.db");
        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

        _db = new SQLiteConnection(path,
            SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create |
            SQLiteOpenFlags.FullMutex);
        _db.CreateTable<Ice>();

        _jsonSettings = new JsonSerializerSettings
        {
            ObjectCreationHandling = ObjectCreationHandling.Replace,
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            TypeNameHandling = TypeNameHandling.All,
        };
    }

    #region Exist and Expiration Methods

    /// <summary>
    /// Checks to see if the key exists in the IceStorage.
    /// </summary>
    /// <param name="key">Unique identifier for the entry to check</param>
    /// <returns>If the key exists</returns>
    public bool Exists(string key)
    {
        if (string.IsNullOrWhiteSpace(key))
            throw new ArgumentException("Key can not be null or empty.", nameof(key));

        Ice ent;
        lock (_dbLock)
        {
            ent = _db.Find<Ice>(key);
        }

        return ent != null;
    }

    /// <summary>
    /// Checks to see if the entry for the key is expired.
    /// </summary>
    /// <param name="key">Key to check</param>
    /// <returns>If the expiration data has been met</returns>
    public bool IsExpired(string key)
    {
        if (string.IsNullOrWhiteSpace(key))
            throw new ArgumentException("Key can not be null or empty.", nameof(key));

        Ice ent;
        lock (_dbLock)
        {
            ent = _db.Find<Ice>(key);
        }

        if (ent == null)
            return true;

        return DateTime.UtcNow > ent.ExpirationDate;
    }

    #endregion

    #region Get Methods

    /// <summary>
    /// Gets all the keys that are saved in the cache
    /// </summary>
    /// <returns>The IEnumerable of keys</returns>
    public IEnumerable<string> GetKeys(CacheState state = CacheState.Active)
    {
        IEnumerable<Ice> allIces;
        lock (_dbLock)
        {
            allIces = _db.Query<Ice>($"SELECT Id FROM {nameof(Ice)}");
        }

        if (allIces == null) return Array.Empty<string>();
        var ices = new List<Ice>();

        if (state.HasFlag(CacheState.Active))
        {
            ices = allIces
                .Where(x => GetExpiration(x.Id) >= DateTime.UtcNow)
                .ToList();
        }

        if (state.HasFlag(CacheState.Expired))
        {
            ices.AddRange(allIces.Where(x =>
                GetExpiration(x.Id) < DateTime.UtcNow));
        }

        return ices.Select(x => x.Id);
    }

    /// <summary>
    /// Get keys with specified eTag
    /// </summary>
    /// <param name="eTag">标签名称</param>
    /// <param name="state">State to get: Multiple with flags: CacheState.Active | CacheState.Expired</param>
    /// <returns>The keys</returns>
    public IEnumerable<string> GetKeys(string? eTag, CacheState state = CacheState.Active)
    {
        IEnumerable<Ice> allIces;
        lock (_dbLock)
        {
            allIces = _db.Query<Ice>($"SELECT Id,ETag FROM {nameof(Ice)}");
        }

        if (allIces == null) return Array.Empty<string>();
        var ices = new List<Ice>();

        if (state.HasFlag(CacheState.Active))
        {
            ices = allIces
                .Where(x =>
                    x.ETag != null && GetExpiration(x.Id) >= DateTime.UtcNow &&
                    x.ETag.Equals(eTag))
                .ToList();
        }

        if (state.HasFlag(CacheState.Expired))
        {
            ices.AddRange(allIces.Where(x =>
                x.ETag != null && GetExpiration(x.Id) < DateTime.UtcNow &&
                x.ETag.Equals(eTag)));
        }

        return ices.Select(x => x.Id);
    }

    /// <summary>
    /// Gets the data entry for the specified key.
    /// </summary>
    /// <param name="key">Unique identifier for the entry to get</param>
    /// <param name="jsonSerializationSettings">Custom json serialization settings to use</param>
    /// <returns>The data object that was stored if found, else default(T)</returns>
    public T? Get<T>(string key, JsonSerializerSettings? jsonSerializationSettings = null)
    {
        if (string.IsNullOrWhiteSpace(key))
            throw new ArgumentException("Key can not be null or empty.", nameof(key));

        Ice? ent;
        lock (_dbLock)
        {
            ent = _db.Query<Ice>(
                $"SELECT {nameof(ent.Contents)} FROM {nameof(Ice)} WHERE {nameof(ent.Id)} = ?",
                key).FirstOrDefault();
        }

        var result = default(T);

        if (ent == null || AutoExpire && IsExpired(key))
            return result;

        if (IceStorageUtils.IsString(result))
        {
            object final = ent.Contents;
            return (T)final;
        }


        return JsonConvert.DeserializeObject<T>(ent.Contents,
            jsonSerializationSettings ?? _jsonSettings);
    }

    /// <summary>
    /// Gets the ETag for the specified key.
    /// </summary>
    /// <param name="key">Unique identifier for entry to get</param>
    /// <returns>The ETag if the key is found, else null</returns>
    public string? GetETag(string key)
    {
        if (string.IsNullOrWhiteSpace(key))
            throw new ArgumentException("Key can not be null or empty.", nameof(key));

        Ice? ent;
        lock (_dbLock)
        {
            ent = _db.Query<Ice>(
                $"SELECT {nameof(ent.ETag)} FROM {nameof(Ice)} WHERE {nameof(ent.Id)} = ?",
                key).FirstOrDefault();
        }

        return ent?.ETag;
    }

    /// <summary>
    /// Gets the DateTime that the item will expire for the specified key.
    /// </summary>
    /// <param name="key">Unique identifier for entry to get</param>
    /// <returns>The expiration date if the key is found, else null</returns>
    public DateTime? GetExpiration(string key)
    {
        Ice? ent;
        lock (_dbLock)
        {
            ent = _db.Query<Ice>(
                $"SELECT {nameof(ent.ExpirationDate)} FROM {nameof(Ice)} WHERE {nameof(ent.Id)} = ?",
                key).FirstOrDefault();
        }

        return ent?.ExpirationDate;
    }

    #endregion

    #region Add Methods

    /// <summary>
    /// Adds a string to the barrel
    /// </summary>
    /// <param name="key">Unique identifier for the entry</param>
    /// <param name="data">Data string to store</param>
    /// <param name="expireIn">Time from UtcNow to expire entry in</param>
    /// <param name="eTag">Optional eTag information</param>
    private void Add(string key, string? data, TimeSpan expireIn, string? eTag = null)
    {
        var ent = new Ice
        {
            Id = key,
            ExpirationDate = IceStorageUtils.GetExpiration(expireIn),
            ETag = eTag,
            Contents = data
        };

        lock (_dbLock)
        {
            _db.InsertOrReplace(ent);
        }
    }

    /// <summary>
    /// Adds an entry to the barrel
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="key">Unique identifier for the entry</param>
    /// <param name="data">Data object to store</param>
    /// <param name="expireIn">Time from UtcNow to expire entry in</param>
    /// <param name="eTag">Optional eTag information</param>
    /// <param name="jsonSerializationSettings">Custom json serialization settings to use</param>
    public void Add<T>(string key, T data, TimeSpan expireIn, string? eTag = null,
        JsonSerializerSettings? jsonSerializationSettings = null)
    {
        if (string.IsNullOrWhiteSpace(key))
            throw new ArgumentException("Key can not be null or empty.", nameof(key));

        const string err = "Data can not be null.";
        if (data == null)
            throw new ArgumentNullException(err, nameof(data));

        string? dataJson;

        if (IceStorageUtils.IsString(data))
        {
            dataJson = data as string;
        }
        else
        {
            dataJson =
                JsonConvert.SerializeObject(data,
                    jsonSerializationSettings ?? _jsonSettings);
        }

        Add(key, dataJson, expireIn, eTag);
    }

    #endregion

    #region Empty Methods

    /// <summary>
    /// Empties all expired entries that are in the IceStorage.
    /// Throws an exception if any deletions fail and rolls back changes.
    /// </summary>
    public void EmptyExpired()
    {
        lock (_dbLock)
        {
            var entries = _db.Query<Ice>(
                "SELECT * FROM Ice WHERE ExpirationDate < ?", DateTime.UtcNow.Ticks);
            _db.RunInTransaction(() =>
            {
                foreach (var k in entries)
                    _db.Delete<Ice>(k.Id);
            });
        }
    }

    /// <summary>
    /// Empties all expired entries that are in the IceStorage.
    /// Throws an exception if any deletions fail and rolls back changes.
    /// </summary>
    public void EmptyAll()
    {
        lock (_dbLock)
        {
            _db.DeleteAll<Ice>();
        }
    }

    /// <summary>
    /// Empties all specified entries regardless if they are expired.
    /// Throws an exception if any deletions fail and rolls back changes.
    /// </summary>
    /// <param name="key">keys to empty</param>
    public void Empty(params string[] key)
    {
        lock (_dbLock)
        {
            _db.RunInTransaction(() =>
            {
                foreach (var k in key)
                {
                    if (string.IsNullOrWhiteSpace(k))
                        continue;

                    _db.Delete<Ice>(primaryKey: k);
                }
            });
        }
    }

    #endregion
}