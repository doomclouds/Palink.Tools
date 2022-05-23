using System;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using Palink.Tools.Extensions.StringExt;
using Palink.Tools.System.Caching.Local;

namespace Palink.Tools.PanShi.Monitor.ECM;

/// <summary>
/// 云监控服务
/// </summary>
public class ECMService
{
    private Timer BeatsTimer { get; }
    private Timer MessageTimer { get; }

    /// <summary>
    /// 展品编号
    /// </summary>
    public string ExhibitNo { get; }

    /// <summary>
    /// 服务器URL
    /// </summary>
    public string Url { get; }

    private readonly IIceStorage _iceStorage;

    /// <summary>
    /// EcmService构造器
    /// </summary>
    /// <param name="minDelay">心跳间隔，单位分钟</param>
    /// <param name="exhibitNo">展品编号</param>
    /// <param name="url">服务器地址</param>
    /// <param name="cacheAppId">缓存位置唯一标识</param>
    /// <param name="remainCache">是否保留缓存</param>
    public ECMService(double minDelay, string exhibitNo, string url, string cacheAppId)
    {
        IceStorage.ApplicationId = cacheAppId;
        _iceStorage =
            IceStorage.Create(
                $"{AppDomain.CurrentDomain.BaseDirectory}{IceStorage.ApplicationId}");

        BeatsTimer = new Timer(minDelay * 60 * 1000);
        BeatsTimer.Elapsed += BeatsTimer_Elapsed;
        BeatsTimer.Start();
        MessageTimer = new Timer(1000);
        MessageTimer.Elapsed += MessageTimer_Elapsed;
        MessageTimer.Start();

        if (exhibitNo.IsNullOrEmpty())
        {
            throw new ArgumentException("不能为空", nameof(exhibitNo));
        }

        if (url.IsNullOrEmpty())
        {
            throw new ArgumentException("不能为空", nameof(url));
        }

        ExhibitNo = exhibitNo;
        Url = url;

        Task.Run(() => { this.CreateBeats().SendDataToECM(); });
    }

    private void MessageTimer_Elapsed(object? sender, ElapsedEventArgs e)
    {
        MessageTimer.Stop();

        _iceStorage.EmptyExpired();

        var keys = _iceStorage.GetKeys();

        foreach (var key in keys)
        {
            var msg = _iceStorage.Get<ECMMessage>(key);

            switch (msg?.SendSucceed)
            {
                case false when msg.Tag != MessageTag.Needed &&
                                msg.Tag != MessageTag.AutoExpireNeeded:
                    msg.SendDataToECM();
                    msg.SendSucceed = true;
                    // _iceStorage.Empty(msg.Id);
                    _iceStorage.Add(msg.Id, msg, msg.ETime, msg.GetTag());
                    break;
                case false:
                {
                    if (msg.SendDataToECM(true))
                    {
                        msg.SendSucceed = true;
                        // _iceStorage.Empty(msg.Id);
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

    private void BeatsTimer_Elapsed(object? sender, ElapsedEventArgs e)
    {
        BeatsTimer.Stop();

        this.CreateBeats().SendDataToECM();

        BeatsTimer.Start();
    }

    /// <summary>
    /// 向消息队列添加新消息
    /// </summary>
    /// <param name="message"></param>
    public void AddMessage(ECMMessage message)
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