using System.Reflection;
using System;
using System.Diagnostics.CodeAnalysis;
using Palink.Tools.Extensions.ObjectExt;

namespace Palink.Tools.Messaging;

public class WeakAction
{
    [NotNull] protected MethodInfo? Method { get; set; }

    public virtual string MethodName => Method.Name;

    [NotNull] protected WeakReference? ActionReference { get; set; }


    [NotNull] protected WeakReference? Reference { get; set; }


    public WeakAction(object target, Action action)
    {
        Method = action.Method;
        ActionReference = new WeakReference(action.Target);
        Reference = new WeakReference(target);
    }

    public WeakAction()
    {
        
    }

    public object Target => Reference.Target;

    protected object ActionTarget => ActionReference.Target;

    public void Execute()
    {
        var actionTarget = ActionTarget;

        Method.Invoke(actionTarget, null);
    }

    public void MarkForDeletion()
    {
        Reference = null;
        ActionReference = null;
        Method = null;
    }
}

public class WeakAction<TMessage> : WeakAction, IExecuteWithObject
{
    public WeakAction(object target, Action<TMessage> action)
    {
        Method = action.Method;
        ActionReference = new WeakReference(action.Target);
        Reference = new WeakReference(target);
    }

    public void ExecuteWithObject(object parameter)
    {
        var parameterCasted = (TMessage)parameter;
        Execute(parameterCasted);
    }

    public void Execute(TMessage parameter)
    {
        if (parameter.IsNull()) return;

        var actionTarget = ActionTarget;

        Method.Invoke(actionTarget, new object[] { parameter });
    }
}