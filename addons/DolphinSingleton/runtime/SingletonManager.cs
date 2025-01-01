using System;
using System.Reflection;
using GODolphin.IOC;
using GODolphin.Pool;
using Godot;

namespace GODolphin.Singleton;

public sealed partial class SingletonManager : Node, ISingleton, IPoolable
{
    private static SingletonManager _instance;

    private readonly IOCContainer _ioc = new();

    public static SingletonManager Instance => _instance;

    private SingletonManager() { }

    public void OnSingletonInitialize()
    {
        Global.Initialize(GetTree());
        SubscribeAll();
    }

    public override void _EnterTree()
    {
        _instance = this;
        OnSingletonInitialize();
    }

    private void SubscribeAll()
    {
        var resource = GD.Load<SingletonResource>(GODolphinConst.ManagerTypeResource);
        var methodInfo = typeof(SingletonManager).GetMethod(nameof(Register), BindingFlags.NonPublic | BindingFlags.Instance);
        foreach (var type in resource.ManagerTypeArray)
        {
            var t = Type.GetType(type);
            var genericMethod = methodInfo.MakeGenericMethod(t);
            genericMethod.Invoke(this, null);
        }
    }


    private void Register<T>()
        where T : Node, ISingleton
    {
        MemberInfo info = typeof(T);
        var attributes = info.GetCustomAttributes(true);
        foreach (var atr in attributes)
        {
            var defineAtr = atr as NodePathAttribute;
            if (defineAtr == null)
                continue;
            var node = SingletonCreator.CreateNonPublicConstructorObject<T>();
            node.Name = defineAtr.NodeName;
            AddChild(node);
            _ioc.RegisterSpecial<T>(defineAtr);
            return;
        }

        {
            var node = SingletonCreator.CreateNonPublicConstructorObject<T>();
            node.Name = info.Name;
            AddChild(node);
            _ioc.RegisterSpecial<T>(new NodePathAttribute(info.Name));
        }
    }

    public string GetInsName<T>()
        where T : Node, ISingleton
    {
        var val = _ioc.GetSpecial<T>() as NodePathAttribute;
        return val?.NodeName;
    }

    public void Reset()
    {
        Instance._ioc.Clear();
    }

    protected override void Dispose(bool disposing)
    {
        Reset();
        _instance = null;
    }
}
