﻿using System;

namespace Palink.Tools.Messaging;

public interface IMessenger
{
    void Register<TMessage>(object recipient, Action<TMessage> action, object? token = default);

    void Register(object recipient, Action action, object? token = default);

    void Send(object? token = default);

    void Send<TMessage>(object? token, TMessage message);

    void Unregister(object recipient, Action action, object? token = default);

    void Unregister<TMessage>(object recipient, Action<TMessage> action, object? token = default);

    void Reset();
}