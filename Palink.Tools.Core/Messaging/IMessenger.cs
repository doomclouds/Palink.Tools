using System;

namespace Palink.Tools.Messaging;

public interface IMessenger
{
    void Register<TMessage>(object recipient, Action<TMessage> action, object? token = default);
   
    void Send<TMessage>(TMessage message);

    void Send<TMessage>(TMessage message, object token);

    void Unregister<TMessage>(object recipient, object? token = default,
        Action<TMessage>? action = default);

    void Reset();
}