using System;
using System.Collections.Generic;
using System.Linq;

namespace Palink.Tools.Messaging;

public class Messenger : IMessenger
{
    private static readonly object CreationLock = new();
    private static IMessenger? _defaultInstance;
    private readonly object _registerLock = new();

    private readonly Dictionary<Type, List<WeakActionAndToken>> _recipientsAction = new();

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

    public void Register<TMessage>(object recipient, Action<TMessage> action,
        object? token = default)
    {
        lock (_registerLock)
        {
            var messageType = typeof(TMessage);

            lock (_recipientsAction)
            {
                List<WeakActionAndToken> list;

                if (!_recipientsAction.ContainsKey(messageType))
                {
                    list = new List<WeakActionAndToken>();
                    _recipientsAction.Add(messageType, list);
                }
                else
                {
                    list = _recipientsAction[messageType];
                }

                var weakAction = new WeakAction<TMessage>(recipient, action);

                var item = new WeakActionAndToken(weakAction, token);

                list.Add(item);
            }
        }

        Cleanup();
    }

    public void Register(object recipient, Action action, object? token = default)
    {
        lock (_registerLock)
        {
            var messageType = typeof(Messenger);

            lock (_recipientsAction)
            {
                List<WeakActionAndToken> list;

                if (!_recipientsAction.ContainsKey(messageType))
                {
                    list = new List<WeakActionAndToken>();
                    _recipientsAction.Add(messageType, list);
                }
                else
                {
                    list = _recipientsAction[messageType];
                }

                var weakAction = new WeakAction(recipient, action);

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

    public void Send(object? token = default)
    {
        SendToTargetOrType(this, token);
    }

    public void Send<TMessage>(object? token, TMessage message)
    {
        SendToTargetOrType(message, token);
    }

    public void Unregister<TMessage>(object recipient, Action<TMessage> action,
        object? token = default)
    {
        UnregisterFromLists(recipient, action, token, _recipientsAction);
        Cleanup();
    }

    public void Unregister(object recipient, Action action, object? token = default)
    {
        UnregisterFromLists(recipient, action, token, _recipientsAction);
        Cleanup();
    }

    private void SendToTargetOrType<TMessage>(TMessage message, object? token)
    {
        var messageType = typeof(TMessage);

        var listClone =
            _recipientsAction.Keys.Take(_recipientsAction.Count).ToList();

        foreach (var type in listClone)
        {
            var list = new List<WeakActionAndToken>();

            if (messageType == type)
            {
                lock (_recipientsAction)
                {
                    list = _recipientsAction[type]
                        .Take(_recipientsAction[type].Count)
                        .ToList();
                }
            }

            SendToList(message, list, token);
        }

        Cleanup();
    }

    private static void SendToList<TMessage>(
        TMessage message, IEnumerable<WeakActionAndToken> weakActionsAndTokens
        , object? token)
    {
        if (message == null) return;

        var list = weakActionsAndTokens.ToList();
        var listClone = list.Take(list.Count).ToList();

        foreach (var item in listClone)
        {
            switch (item.Action)
            {
                case IExecuteWithObject executeAction
                    when ((item.Token == null && token == null)
                          || item.Token != null && item.Token.Equals(token)):
                    executeAction.ExecuteWithObject(message);
                    continue;
                case { } weakAction
                    when ((item.Token == null && token == null)
                          || item.Token != null && item.Token.Equals(token)):
                    weakAction.Execute();
                    break;
            }
        }
    }

    private static void UnregisterFromLists<TMessage>(
        object recipient, Action<TMessage> action, object? token,
        Dictionary<Type, List<WeakActionAndToken>> lists)
    {
        var messageType = typeof(TMessage);

        if (!lists.Any() || !lists.ContainsKey(messageType))
        {
            return;
        }

        lock (lists)
        {
            foreach (var item in lists[messageType])
            {
                if (item.Action is WeakAction<TMessage> weakActionCasted
                    && recipient == weakActionCasted.Target
                    && action.Method.Name == weakActionCasted.MethodName
                    && ((token == null && item.Token == null)
                        || token?.Equals(item.Token) == true))
                {
                    item.Action.MarkForDeletion();
                }
            }
        }
    }

    private static void UnregisterFromLists(object recipient, Action action, object? token,
        Dictionary<Type, List<WeakActionAndToken>> lists)
    {
        var messageType = typeof(Messenger);

        if (!lists.Any() || !lists.ContainsKey(messageType))
        {
            return;
        }

        lock (lists)
        {
            foreach (var item in lists[messageType])
            {
                if (item.Action is { } weakActionCasted
                    && recipient == weakActionCasted.Target
                    && action.Method.Name == weakActionCasted.MethodName
                    && ((token == null && item.Token == null)
                        || token?.Equals(item.Token) == true))
                {
                    item.Action.MarkForDeletion();
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
                    .Where(item => item.Action.ShouldDelete)
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
        _recipientsAction.Clear();
    }

    private struct WeakActionAndToken
    {
        public WeakActionAndToken(WeakAction action, object? token)
        {
            Action = action;
            Token = token;
        }

        public WeakAction Action;

        public object? Token;
    }
}