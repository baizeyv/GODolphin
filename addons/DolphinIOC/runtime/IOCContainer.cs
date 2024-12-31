using System;
using System.Collections.Generic;
using System.Linq;
using Godot.NativeInterop;

namespace GODolphin.IOC;

/// <summary>
/// * 控制反转容器
/// </summary>
public class IOCContainer
{
    private Dictionary<Type, object> _instances = new();

    public void Register<T>(T instance)
    {
        var key = typeof(T);
        if (_instances.ContainsKey(key))
        {
            _instances[key] = instance;
        }
        else
        {
            _instances.Add(key, instance);
        }
    }

    public void RegisterSpecial<T>(object obj)
    {
        var key = typeof(T);
        if (_instances.ContainsKey(key))
        {
            _instances[key] = obj;
        }
        else
        {
            _instances.Add(key, obj);
        }
    }

    public object GetSpecial<T>()
        where T : class
    {
        var key = typeof(T);
        return _instances.GetValueOrDefault(key);
    }

    public T Get<T>()
        where T : class
    {
        var key = typeof(T);
        if (_instances.TryGetValue(key, out var retInstance))
        {
            return retInstance as T;
        }

        return null;
    }

    public IEnumerable<T> GetInstancesByType<T>()
    {
        var type = typeof(T);
        return _instances.Values.Where(item => type.IsInstanceOfType(item)).Cast<T>();
    }

    public Dictionary<Type, object> GetMap()
    {
        return _instances;
    }

    public void Clear() => _instances.Clear();
}
