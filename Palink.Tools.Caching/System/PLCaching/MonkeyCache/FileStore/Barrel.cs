using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using Newtonsoft.Json;
using Palink.Tools.Extensions.PLString;

namespace Palink.Tools.System.PLCaching.MonkeyCache.FileStore;

/// <summary>
/// 文件缓存
/// </summary>
public class Barrel : IBarrel
{
    private readonly ReaderWriterLockSlim _indexLocker;
    private readonly JsonSerializerSettings _jsonSettings;
    private readonly Lazy<string> _baseDirectory;
    private readonly HashAlgorithm _hashAlgorithm;

    /// <summary>
    /// FileStore Barrel constructor
    /// </summary>
    /// <param name="cacheDirectory">Optionally specify directory where cache will live</param>
    /// <param name="hash">Optionally specify hash algorithm</param>
    private Barrel(string? cacheDirectory = null, HashAlgorithm? hash = null)
    {
        if (cacheDirectory.IsNullOrEmpty())
        {
            _baseDirectory = new Lazy<string>(() =>
                Path.Combine(BarrelUtils.GetBasePath(ApplicationId)));
        }
        else
        {
            _baseDirectory = new Lazy<string>(() => cacheDirectory);
        }


        _hashAlgorithm = hash ?? MD5.Create();

        _indexLocker = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);

        _jsonSettings = new JsonSerializerSettings
        {
            ObjectCreationHandling = ObjectCreationHandling.Replace,
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            TypeNameHandling = TypeNameHandling.All,
        };

        _index = new Dictionary<string, Tuple<string, DateTime>>();

        LoadIndex();
        WriteIndex();
    }

    /// <summary>
    /// 路径标识，与基础路径一起合成完整路径
    /// </summary>
    public static string ApplicationId { get; set; } = string.Empty;

    /// <summary>
    /// AutoExpire
    /// </summary>
    public bool AutoExpire { get; set; }

    private static Barrel? _instance;

    /// <summary>
    /// Gets the instance of the Barrel
    /// </summary>
    public static IBarrel Current => (_instance ??= new Barrel());

    /// <summary>
    /// FileStore Barrel
    /// </summary>
    /// <param name="cacheDirectory">Optionally specify directory where cache will live</param>
    /// <param name="hash">Optionally specify hash algorithm</param>
    public static IBarrel Create(string cacheDirectory, HashAlgorithm? hash = null) =>
        new Barrel(cacheDirectory, hash);

    /// <summary>
    /// Adds an entry to the barrel
    /// </summary>
    /// <param name="key">Unique identifier for the entry</param>
    /// <param name="data">Data object to store</param>
    /// <param name="expireIn">Time from UtcNow to expire entry in</param>
    /// <param name="eTag">Optional eTag information</param>
    private void Add(string key, string? data, TimeSpan expireIn, string? eTag = null)
    {
        _indexLocker.EnterWriteLock();

        try
        {
            var hash = Hash(key);
            var path = Path.Combine(_baseDirectory.Value, hash);

            if (!Directory.Exists(_baseDirectory.Value))
                Directory.CreateDirectory(_baseDirectory.Value);

            File.WriteAllText(path, data);

            _index[key] = new Tuple<string, DateTime>(eTag ?? string.Empty,
                BarrelUtils.GetExpiration(expireIn));

            WriteIndex();
        }
        finally
        {
            _indexLocker.ExitWriteLock();
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
    public void Add<T>(string key,
        T data,
        TimeSpan expireIn,
        string? eTag = null,
        JsonSerializerSettings? jsonSerializationSettings = null)
    {
        if (string.IsNullOrWhiteSpace(key))
            throw new ArgumentException("Key can not be null or empty.", nameof(key));

        const string err = "Data can not be null.";
        if (data == null)
            throw new ArgumentNullException(err, nameof(data));

        string? dataJson;

        if (BarrelUtils.IsString(data))
        {
            dataJson = data as string;
        }
        else
        {
            dataJson = JsonConvert.SerializeObject(data,
                jsonSerializationSettings ?? _jsonSettings);
        }

        Add(key, dataJson, expireIn, eTag);
    }

    /// <summary>
    /// Empties all specified entries regardless if they are expired.
    /// Throws an exception if any deletions fail and rolls back changes.
    /// </summary>
    /// <param name="key">keys to empty</param>
    public void Empty(params string[] key)
    {
        _indexLocker.EnterWriteLock();

        try
        {
            foreach (var k in key)
            {
                if (string.IsNullOrWhiteSpace(k))
                    continue;

                var file = Path.Combine(_baseDirectory.Value, Hash(k));
                if (File.Exists(file))
                    File.Delete(file);

                _index.Remove(k);
            }

            WriteIndex();
        }
        finally
        {
            _indexLocker.ExitWriteLock();
        }
    }

    /// <summary>
    /// Empties all expired entries that are in the Barrel.
    /// Throws an exception if any deletions fail and rolls back changes.
    /// </summary>
    public void EmptyAll()
    {
        _indexLocker.EnterWriteLock();

        try
        {
            foreach (var file in _index.Select(item => Hash(item.Key))
                         .Select(hash => Path.Combine(_baseDirectory.Value, hash))
                         .Where(File.Exists))
            {
                File.Delete(file);
            }

            _index.Clear();

            WriteIndex();
        }
        finally
        {
            _indexLocker.ExitWriteLock();
        }
    }

    /// <summary>
    /// Empties all expired entries that are in the Barrel.
    /// Throws an exception if any deletions fail and rolls back changes.
    /// </summary>
    public void EmptyExpired()
    {
        _indexLocker.EnterWriteLock();

        try
        {
            var expired = _index.Where(k => k.Value.Item2 < DateTime.UtcNow);

            var toRem = new List<string>();

            foreach (var item in expired)
            {
                var hash = Hash(item.Key);
                var file = Path.Combine(_baseDirectory.Value, hash);
                if (File.Exists(file))
                    File.Delete(file);
                toRem.Add(item.Key);
            }

            foreach (var key in toRem)
                _index.Remove(key);

            WriteIndex();
        }
        finally
        {
            _indexLocker.ExitWriteLock();
        }
    }

    /// <summary>
    /// Checks to see if the key exists in the Barrel.
    /// </summary>
    /// <param name="key">Unique identifier for the entry to check</param>
    /// <returns>If the key exists</returns>
    public bool Exists(string key)
    {
        if (string.IsNullOrWhiteSpace(key))
            throw new ArgumentException("Key can not be null or empty.", nameof(key));

        bool exists;

        _indexLocker.EnterReadLock();

        try
        {
            exists = _index.ContainsKey(key);
        }
        finally
        {
            _indexLocker.ExitReadLock();
        }

        return exists;
    }

    /// <summary>
    /// Gets all the keys that are saved in the cache
    /// </summary>
    /// <returns>The IEnumerable of keys</returns>
    public IEnumerable<string> GetKeys(CacheState state = CacheState.Active)
    {
        _indexLocker.EnterReadLock();

        try
        {
            var bananas = new List<KeyValuePair<string, Tuple<string, DateTime>>>();

            if (state.HasFlag(CacheState.Active))
            {
                bananas = _index
                    .Where(x => x.Value.Item2 >= DateTime.UtcNow)
                    .ToList();
            }

            if (state.HasFlag(CacheState.Expired))
            {
                bananas.AddRange(_index.Where(x => x.Value.Item2 < DateTime.UtcNow));
            }

            return bananas.Select(x => x.Key);
        }
        catch (Exception)
        {
            return Array.Empty<string>();
        }
        finally
        {
            _indexLocker.ExitReadLock();
        }
    }

    /// <summary>
    /// 根据缓存状态和标签值获取缓存
    /// </summary>
    /// <param name="eTag">标签名称</param>
    /// <param name="state">State to get: Multiple with flags: CacheState.Active | CacheState.Expired</param>
    /// <returns>The keys</returns>
    public IEnumerable<string> GetKeys(string? eTag, CacheState state = CacheState.Active)
    {
        _indexLocker.EnterReadLock();

        try
        {
            var bananas = new List<KeyValuePair<string, Tuple<string, DateTime>>>();

            if (state.HasFlag(CacheState.Active))
            {
                bananas = _index
                    .Where(x =>
                        x.Value.Item2 >= DateTime.UtcNow &&
                        x.Value.Item1.Equals(eTag))
                    .ToList();
            }

            if (state.HasFlag(CacheState.Expired))
            {
                bananas.AddRange(_index.Where(x => x.Value.Item2 < DateTime.UtcNow &&
                                                   x.Value.Item1.Equals(eTag)));
            }

            return bananas.Select(x => x.Key);
        }
        catch (Exception)
        {
            return Array.Empty<string>();
        }
        finally
        {
            _indexLocker.ExitReadLock();
        }
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

        var result = default(T);

        _indexLocker.EnterReadLock();

        try
        {
            var hash = Hash(key);
            var path = Path.Combine(_baseDirectory.Value, hash);

            if (_index.ContainsKey(key) && File.Exists(path) &&
                (!AutoExpire || (AutoExpire && !IsExpired(key))))
            {
                var contents = File.ReadAllText(path);
                if (BarrelUtils.IsString(result))
                {
                    object final = contents;
                    return (T)final;
                }

                result = JsonConvert.DeserializeObject<T>(contents,
                    jsonSerializationSettings ?? _jsonSettings);
            }
        }
        finally
        {
            _indexLocker.ExitReadLock();
        }

        return result;
    }

    /// <summary>
    /// Gets the DateTime that the item will expire for the specified key.
    /// </summary>
    /// <param name="key">Unique identifier for entry to get</param>
    /// <returns>The expiration date if the key is found, else null</returns>
    public DateTime? GetExpiration(string key)
    {
        if (string.IsNullOrWhiteSpace(key))
            throw new ArgumentException("Key can not be null or empty.", nameof(key));

        DateTime? date = null;

        _indexLocker.EnterReadLock();

        try
        {
            if (_index.ContainsKey(key))
                date = _index[key]?.Item2;
        }
        finally
        {
            _indexLocker.ExitReadLock();
        }

        return date;
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

        string? etag = null;

        _indexLocker.EnterReadLock();

        try
        {
            if (_index.ContainsKey(key))
                etag = _index[key]?.Item1;
        }
        finally
        {
            _indexLocker.ExitReadLock();
        }

        return etag;
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

        var expired = true;

        _indexLocker.EnterReadLock();

        try
        {
            if (_index.ContainsKey(key))
                expired = _index[key].Item2 < DateTime.UtcNow;
        }
        finally
        {
            _indexLocker.ExitReadLock();
        }

        return expired;
    }

    private readonly Dictionary<string, Tuple<string, DateTime>> _index;

    private const string IndexFilename = "idx.dat";

    private string? _indexFile;

    private void WriteIndex()
    {
        if (string.IsNullOrEmpty(_indexFile))
            _indexFile = Path.Combine(_baseDirectory.Value, IndexFilename);
        if (!Directory.Exists(_baseDirectory.Value))
            Directory.CreateDirectory(_baseDirectory.Value);

        if (_indexFile.IsNullOrEmpty())
            throw new ArgumentNullException(nameof(_indexFile), "indexFile不能为空");
        using var f = File.Open(_indexFile, FileMode.Create);
        using var sw = new StreamWriter(f);
        foreach (var kvp in _index)
        {
            var dtEpoch = DateTimeToEpochSeconds(kvp.Value.Item2);
            sw.WriteLine($"{kvp.Key}\t{kvp.Value.Item1}\t{dtEpoch.ToString()}");
        }
    }

    private void LoadIndex()
    {
        if (string.IsNullOrEmpty(_indexFile))
            _indexFile = Path.Combine(_baseDirectory.Value, IndexFilename);

        if (!File.Exists(_indexFile))
            return;

        _index.Clear();

        if (_indexFile.IsNullOrEmpty())
            throw new ArgumentNullException(nameof(_indexFile), "indexFile不能为空");
        using var f = File.OpenRead(_indexFile);
        using var sw = new StreamReader(f);
        string line;
        while ((line = sw.ReadLine() ?? string.Empty) != null)
        {
            var parts = line.Split('\t');
            if (parts.Length == 3)
            {
                var key = parts[0];
                var etag = parts[1];
                var dt = parts[2];

                if (!string.IsNullOrEmpty(key) &&
                    int.TryParse(dt, out var secondsSinceEpoch) &&
                    !_index.ContainsKey(key))
                    _index.Add(key,
                        new Tuple<string, DateTime>(etag,
                            EpochSecondsToDateTime(secondsSinceEpoch)));
            }
        }
    }

    private string Hash(string input)
    {
        var data = _hashAlgorithm.ComputeHash(Encoding.Default.GetBytes(input));
        return BitConverter.ToString(data);
    }

    private static readonly DateTime
        Epoch = new(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);

    private static int DateTimeToEpochSeconds(DateTime date)
    {
        var diff = date - Epoch;
        return (int)diff.TotalSeconds;
    }

    private static DateTime EpochSecondsToDateTime(int seconds) =>
        Epoch + TimeSpan.FromSeconds(seconds);
}