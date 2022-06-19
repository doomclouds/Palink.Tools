using System;
using System.Linq;
using System.Timers;
using Palink.Tools.System.Caching.Local;

namespace Palink.Tools.Monitor;

/// <summary>
/// CachePostService
/// </summary>
///todo 这里并不完美，每次发送不同消息都要一个PostCacheService，增加更多后台线程处理任务，加大系统负担
public class PostCacheService<T> where T : Message
{
    private Timer MessageTimer { get; }

    private readonly IIceStorage _iceStorage;

    /// <summary>
    /// EcmService构造器
    /// </summary>
    /// <param name="cacheAppId">缓存位置唯一标识</param>
    public PostCacheService(string cacheAppId)
    {
        IceStorage.ApplicationId = cacheAppId;
        _iceStorage =
            IceStorage.Create(
                $"{AppDomain.CurrentDomain.BaseDirectory}{IceStorage.ApplicationId}");

        MessageTimer = new Timer(1000);
        MessageTimer.Elapsed += MessageTimer_Elapsed;
        MessageTimer.Start();
    }

    private async void MessageTimer_Elapsed(object? sender, ElapsedEventArgs e)
    {
        MessageTimer.Stop();

        _iceStorage.EmptyExpired();

        var keys = _iceStorage.GetKeys();

        foreach (var key in keys)
        {
            var msg = _iceStorage.Get<T>(key);

            switch (msg?.SendSucceed)
            {
                case false when msg.Tag != MessageTag.Needed &&
                    msg.Tag != MessageTag.AutoExpireNeeded:
                    await msg.PostJsonToServer();
                    msg.SendSucceed = true;
                    _iceStorage.Add(msg.Id, msg, msg.ETime, msg.GetTag());
                    break;
                case false:
                {
                    if (await msg.PostJsonToServer(true))
                    {
                        msg.SendSucceed = true;
                        _iceStorage.Add(msg.Id, msg, msg.ETime, msg.GetTag());

                        if (msg.Tag == MessageTag.Needed)
                        {
                            _iceStorage.Empty(msg.Id);
                        }
                    }

                    break;
                }
            }
        }

        MessageTimer.Start();
    }

    /// <summary>
    /// 向消息队列添加新消息
    /// </summary>
    /// <param name="message"></param>
    public void AddMessage(T message)
    {
        _iceStorage.EmptyExpired();

        switch (message.Tag)
        {
            default:
            case MessageTag.Once:
            case MessageTag.Needed:
                break;
            case MessageTag.AutoExpire:
            case MessageTag.AutoExpireNeeded:
                //判断该消息是否存在
                var keys = _iceStorage.GetKeys(message.GetTag());
                if (keys.Any())
                {
                    return;
                }

                break;
        }

        _iceStorage.Add(message.Id, message, message.ETime, message.GetTag());
    }
}