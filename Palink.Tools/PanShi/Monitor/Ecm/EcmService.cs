using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Timers;
using Palink.Tools.Extensions;
using Palink.Tools.PLSystems.Caching.MonkeyCache.SQLite;
using Task = System.Threading.Tasks.Task;

namespace Palink.Tools.PanShi.Monitor.Ecm;

/// <summary>
/// 云监控服务
/// </summary>
public class EcmService
{
    private Timer BeatsTimer { get; set; }
    private Timer MessageTimer { get; set; }

    /// <summary>
    /// 展品编号
    /// </summary>
    public string ExhibitNo { get; private set; }

    /// <summary>
    /// 服务器URL
    /// </summary>
    public string Url { get; private set; }

    /// <summary>
    /// 队列消息数量
    /// </summary>
    public int MessageCount => ConcurrentQueue.Count;

    private ConcurrentQueue<EcmMessage> ConcurrentQueue { get; } = new();

    /// <summary>
    /// EcmService构造器
    /// </summary>
    /// <param name="minDelay">心跳间隔，单位分钟</param>
    /// <param name="exhibitNo">展品编号</param>
    /// <param name="url">服务器地址</param>
    public EcmService(double minDelay, string exhibitNo, string url)
    {
        Barrel.ApplicationId = "palink_tools_ecm_message_caching";
        BeatsTimer = new Timer(minDelay * 60 * 1000);
        BeatsTimer.Elapsed += BeatsTimer_Elapsed;
        BeatsTimer.Start();
        MessageTimer = new Timer(100);
        MessageTimer.Elapsed += MessageTimer_Elapsed;
        MessageTimer.Start();

        if (exhibitNo.IsNullOrEmpty() || url.IsNullOrEmpty())
        {
            return;
        }

        ExhibitNo = exhibitNo;
        Url = url;

        Barrel.Current.EmptyExpired();
        var keys = Barrel.Current.GetKeys();
        foreach (var key in keys)
        {
            var message = Barrel.Current.Get<EcmMessage>(key);
            if (message.MessageType == MessageType.Needed)
            {
                ConcurrentQueue.Enqueue(message);
            }
        }

        Task.Run(() =>
        {
            this.BeatsInstance(MessageType.Normal).SendDataToEcm();
        });
    }

    private void MessageTimer_Elapsed(object sender, ElapsedEventArgs e)
    {
        MessageTimer.Stop();

        if (ConcurrentQueue.Any())
        {
            if (GetMessage(out var message))
            {
                switch (message.MessageType)
                {
                    case MessageType.Needed:
                        if (!message.SendDataToEcm(true))
                        {
                            AddMessage(message);
                        }

                        break;
                    case MessageType.FiveMinOnce:
                    case MessageType.TenMinOnce:
                    case MessageType.HalfHourOnce:
                    case MessageType.OneHourOnce:
                        if (!message.SendDataToEcm(true))
                        {
                            AddMessage(message);
                        }
                        else
                        {
                            //更新缓存为发送成功
                            message.SendSucceed = true;
                            Barrel.Current.Empty($"Unrepeated:{message.InfoContent}");
                            Barrel.Current.Add($"Unrepeated:{message.InfoContent}",
                                message,
                                TimeSpan.FromMinutes((double)message.MessageType));
                        }

                        break;
                    case MessageType.ForeverOnce:
                        if (!message.SendDataToEcm(true))
                        {
                            AddMessage(message);
                        }
                        else
                        {
                            //更新缓存为发送成功
                            message.SendSucceed = true;
                            Barrel.Current.Empty($"ForeverOnce:{message.InfoContent}");
                            Barrel.Current.Add($"ForeverOnce:{message.InfoContent}",
                                message, TimeSpan.FromDays(30));
                        }

                        break;
                    case MessageType.Normal:
                    default:
                        message.SendDataToEcm();
                        break;
                }
            }
        }

        MessageTimer.Start();
    }

    private void BeatsTimer_Elapsed(object sender, ElapsedEventArgs e)
    {
        BeatsTimer.Stop();

        this.BeatsInstance(MessageType.Normal).SendDataToEcm();

        BeatsTimer.Start();
    }

    /// <summary>
    /// 向消息队列添加新消息
    /// </summary>
    /// <param name="message"></param>
    public void AddMessage(EcmMessage message)
    {
        Barrel.Current.EmptyExpired();
        var min = 60 * 12;
        if (message.MessageType == MessageType.Needed)
        {
            var cachingMsg =
                Barrel.Current.Get<EcmMessage>(message.Id);
            var keys = Barrel.Current.GetKeys();
            if (cachingMsg == null)
            {
                Barrel.Current.Add(message.Id, message,
                    TimeSpan.FromDays(30));
            }
        }
        else if (message.MessageType != MessageType.Normal &&
                 message.MessageType != MessageType.ForeverOnce &&
                 message.MessageType != MessageType.Needed)
        {
            min = message.MessageType switch
            {
                MessageType.FiveMinOnce => 5,
                MessageType.TenMinOnce => 10,
                MessageType.HalfHourOnce => 30,
                MessageType.OneHourOnce => 60,
                _ => min
            };

            var cachingMsg =
                Barrel.Current.Get<EcmMessage>($"Unrepeated:{message.InfoContent}");
            switch (cachingMsg)
            {
                case { SendSucceed: true }:
                    return;
                case null:
                    Barrel.Current.Add($"Unrepeated:{message.InfoContent}", message,
                        TimeSpan.FromMinutes(min));
                    break;
            }
        }
        else if (message.MessageType == MessageType.ForeverOnce)
        {
            var cachingMsg =
                Barrel.Current.Get<EcmMessage>($"ForeverOnce:{message.InfoContent}");

            switch (cachingMsg)
            {
                case { SendSucceed: true }:
                    return;
                case null:
                    Barrel.Current.Add($"ForeverOnce:{message.InfoContent}", message,
                        TimeSpan.FromDays(30));
                    break;
            }
        }

        ConcurrentQueue.Enqueue(message);
    }

    private bool GetMessage(out EcmMessage msg)
    {
        Barrel.Current.EmptyExpired();
        if (ConcurrentQueue.TryDequeue(out var message))
        {
            if (message.MessageType == MessageType.Needed)
            {
                Barrel.Current.Empty(message.Id);
            }

            msg = message;
            return true;
        }

        msg = null;
        return false;
    }
}