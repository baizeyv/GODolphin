using System.Reflection;
using Godot;

namespace GODolphin.AOP;

/// <summary>
/// NOTE: For Example -> Proxy.Instance.SetTarget(foobarObject).SetMethod("TestMethod").SetArguments(new objects[]{}).SetInvocationHandler(new FoobarInvocationHandler()).Invoke();
/// </summary>

public class Proxy
{
    public static Proxy Instance = new();

    private IInvocationHandler _invocationHandler;

    /// <summary>
    /// * 目标对象
    /// </summary>
    private object _target;

    /// <summary>
    /// * 方法名称
    /// </summary>
    private string _method;

    /// <summary>
    /// * 方法参数数组
    /// </summary>
    private object[] _arguments;

    private Proxy() { }

    public Proxy SetInvocationHandler(IInvocationHandler invocationHandler)
    {
        _invocationHandler = invocationHandler;
        return this;
    }

    public Proxy SetTarget(object target)
    {
        _target = target;
        return this;
    }

    public Proxy SetMethod(string method)
    {
        _method = method;
        return this;
    }

    public Proxy SetArguments(object[] arguments)
    {
        _arguments = arguments;
        return this;
    }

    public object Invoke()
    {
        var methodInfo = _target.GetType().GetMethod(_method);
        return _invocationHandler.Invoke(_target, methodInfo, _arguments);
    }
}

public abstract class AbstractInvocationHandler : IInvocationHandler
{
    public virtual void Preprocess() { }

    public object Invoke(object proxy, MethodInfo method, object[] args)
    {
        GD.Print("----------------");
        Preprocess();
        GD.Print($"PROXY {method.DeclaringType} / {method.Name}");
        var result = method.Invoke(proxy, args);
        PostProcess();
        GD.Print("----------------");
        return result;
    }

    public virtual void PostProcess() { }
}
