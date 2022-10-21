using System;
using System.Collections.Generic;
using System.Linq;

namespace Palink.Tools.Messaging;

public class Messenger : IMessenger
{
    private static readonly object CreationLock = new();
    private static IMessenger? _defaultInstance;
    private readonly object _registerLock = new();

    private Dictionary<Type, List<WeakActionAndToken>>? _recipientsAction;

    public static IMessenger Default
    {
        get
        {
            if (_defaultInstance != null) return _defaultInstance;
            
            lock (CreationLock)
            {
                _defaultInstance ??= new Messenger();
            }

            return _defaultInstance;
        }
    }

    public void Register<TMessage>(object recipient, Action<TMessage> action
        , object? token = default)
    {
        lock (_registerLock)
        {
            var messageType = typeof(TMessage);

            _recipientsAction ??= new Dictionary<Type, List<WeakActionAndToken>>();

            var recipients = _recipientsAction;

            lock (recipients)
            {
                List<WeakActionAndToken> list;

                if (!recipients.ContainsKey(messageType))
                {
                    list = new List<WeakActionAndToken>();
                    recipients.Add(messageType, list);
                }
                else
                {
                    list = recipients[messageType];
                }

                var weakAction = new WeakAction<TMessage>(recipient, action);

                var item = new WeakActionAndToken
                {
                    Action = weakAction,
                    Token = token
                };

                list.Add(item);
            }
        }

        Cleanup();
    }

    public void Send<TMessage>(TMessage message)
    {
        SendToTargetOrType(message, null, null);
    }

    public void Send<TMessage, TTarget>(TMessage message)
    {
        SendToTargetOrType(message, typeof(TTarget), null);
    }

    public void Send<TMessage>(TMessage message, object token)
    {
        SendToTargetOrType(message, null, token);
    }

    public void Unregister<TMessage>(object recipient, object? token = default,
        Action<TMessage>? action = default)
    {
        UnregisterFromLists(recipient, token, action, _recipientsAction);
        Cleanup();
    }

    private void SendToTargetOrType<TMessage>(TMessage message, Type? messageTargetType,
        object? token)
    {
        if (_recipientsAction == null) return;

        var messageType = typeof(TMessage);

        var listClone =
            _recipientsAction.Keys.Take(_recipientsAction.Count).ToList();

        foreach (var type in listClone)
        {
            List<WeakActionAndToken>? list = null;

            if (messageType == type)
            {
                lock (_recipientsAction)
                {
                    list = _recipientsAction[type]
                        .Take(_recipientsAction[type].Count)
                        .ToList();
                }
            }

            SendToList(message, list, messageTargetType, token);
        }

        Cleanup();
    }

    private static void SendToList<TMessage>(
        TMessage message, IEnumerable<WeakActionAndToken>? weakActionsAndTokens,
        Type? messageTargetType, object? token)
    {
        if (weakActionsAndTokens == null || message == null) return;

        var list = weakActionsAndTokens.ToList();
        var listClone = list.Take(list.Count).ToList();

        foreach (var item in listClone)
        {
            if (item.Action is IExecuteWithObject executeAction
                && (messageTargetType == null
                    || item.Action.Target.GetType() == messageTargetType
                    || messageTargetType.IsInstanceOfType(item.Action.Target))
                && ((item.Token == null && token == null)
                    || item.Token != null && item.Token.Equals(token)))
            {
                executeAction.ExecuteWithObject(message);
            }
        }
    }

    private static void UnregisterFromLists<TMessage>(
        object recipient, object? token, Action<TMessage>? action,
        Dictionary<Type, List<WeakActionAndToken>>? lists)
    {
        var messageType = typeof(TMessage);

        if (lists == null || !lists.Any() || !lists.ContainsKey(messageType))
        {
            return;
        }

        lock (lists)
        {
            foreach (var item in lists[messageType])
            {
                if (item.Action is WeakAction<TMessage> weakActionCasted
                    && recipient == weakActionCasted.Target
                    && (action == null || action.Method.Name == weakActionCasted.MethodName)
                    && (token == null || token.Equals(item.Token)))
                {
                    item.Action?.MarkForDeletion();
                }
            }
        }
    }

    public void Cleanup()
    {
        CleanupList(_recipientsAction);
    }

    private static void CleanupList(IDictionary<Type, List<WeakActionAndToken>>? lists)
    {
        if (lists == null)
        {
            return;
        }

        lock (lists)
        {
            var listsToRemove = new List<Type>();
            foreach (var list in lists)
            {
                var recipientsToRemove = list.Value
                    .Where(item => item.Action == null)
                    .ToList();

                foreach (var recipient in recipientsToRemove)
                {
                    list.Value.Remove(recipient);
                }

                if (list.Value.Count == 0)
                {
                    listsToRemove.Add(list.Key);
                }
            }

            foreach (var key in listsToRemove)
            {
                lists.Remove(key);
            }
        }
    }

    public void Reset()
    {
        _recipientsAction?.Clear();
    }

    private struct WeakActionAndToken
    {
        public WeakAction? Action;

        public object? Token;
    }
}