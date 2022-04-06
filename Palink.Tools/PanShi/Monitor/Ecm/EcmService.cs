using System;
using System.Linq;
using System.Timers;
using Palink.Tools.Extensions.PLString;
using Palink.Tools.PLSystems.Caching.MonkeyCache;
using Palink.Tools.PLSystems.Caching.MonkeyCache.SQLite;
using Task = System.Threading.Tasks.Task;

namespace Palink.Tools.PanShi.Monitor.Ecm;

/// <summary>
/// 云监控服务
/// </summary>
public class EcmService
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

    private readonly IBarrel _barrel;

    /// <summary>
    /// EcmService构造器
    /// </summary>
    /// <param name="minDelay">心跳间隔，单位分钟</param>
    /// <param name="exhibitNo">展品编号</param>
    /// <param name="url">服务器地址</param>
    /// <param name="cacheAppId">缓存位置唯一标识</param>
    public EcmService(double minDelay, string exhibitNo, string url, string cacheAppId)
    {
        Barrel.ApplicationId = cacheAppId;
        _barrel =
            Barrel.Create(
                $"{AppDomain.CurrentDomain.BaseDirectory}{Barrel.ApplicationId}");

        BeatsTimer = new Timer(minDelay * 60 * 1000);
        BeatsTimer.Elapsed += BeatsTimer_Elapsed;
        BeatsTimer.Start();
        MessageTimer = new Timer(1000);
        MessageTimer.Elapsed += MessageTimer_Elapsed;
        MessageTimer.Start();

        if (exhibitNo.IsNullOrEmpty() || url.IsNullOrEmpty())
        {
            return;
        }

        ExhibitNo = exhibitNo;
        Url = url;

        Task.Run(() =>
        {
            EcmMessage.BeatsInstance(ExhibitNo).SendDataToEcm(Url);
        });
    }

    private void MessageTimer_Elapsed(object sender, ElapsedEventArgs e)
    {
        MessageTimer.Stop();

        _barrel.EmptyExpired();

        var keys = _barrel.GetKeys();

        foreach (var key in keys)
        {
            var msg = _barrel.Get<EcmMessage>(key);

            if (!msg.SendSucceed && msg.Tag != MessageTag.Needed &&
                msg.Tag != MessageTag.AutoExpireNeeded)
            {
                msg.SendDataToEcm(Url);
                msg.SendSucceed = true;
                // _barrel.Empty(msg.Id);
                _barrel.Add(msg.Id, msg, msg.ETime, msg.GetTag());
            }
            else if (!msg.SendSucceed)
            {
                if (msg.SendDataToEcm(Url, true))
                {
                    msg.SendSucceed = true;
                    // _barrel.Empty(msg.Id);
                    _barrel.Add(msg.Id, msg, msg.ETime, msg.GetTag());

                    if (msg.Tag == MessageTag.Needed)
                    {
                        _barrel.Empty(msg.Id);
                    }
                }
            }
        }

        MessageTimer.Start();
    }

    private void BeatsTimer_Elapsed(object sender, ElapsedEventArgs e)
    {
        BeatsTimer.Stop();

        EcmMessage.BeatsInstance(ExhibitNo).SendDataToEcm(Url);

        BeatsTimer.Start();
    }

    /// <summary>
    /// 向消息队列添加新消息
    /// </summary>
    /// <param name="message"></param>
    public void AddMessage(EcmMessage message)
    {
        _barrel.EmptyExpired();

        switch (message.Tag)
        {
            default:
            case MessageTag.Once:
            case MessageTag.Needed:
                break;
            case MessageTag.AutoExpire:
            case MessageTag.AutoExpireNeeded:
                //判断该消息是否存在
                var keys = _barrel.GetKeys(message.GetTag());
                if (keys.Any())
                {
                    return;
                }

                break;
        }

        _barrel.Add(message.Id, message, message.ETime, message.GetTag());
    }
}