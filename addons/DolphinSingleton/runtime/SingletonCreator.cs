using System;
using System.Reflection;
using Godot;

namespace GODolphin.Singleton;

internal static class SingletonCreator
{
    internal static T CreateNonPublicConstructorObject<T>()
        where T : class
    {
        var type = typeof(T);
        var constructorInfos = type.GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic);

        var ctor = Array.Find(constructorInfos, c => c.GetParameters().Length == 0);

        if (ctor == null)
        {
            throw new Exception("Non-Public Constructor() not found! in " + type);
        }
        return ctor.Invoke(null) as T;
    }

    public static T CreateSingleton<T>()
        where T : class, ISingleton
    {
        var instance = CreateNonPublicConstructorObject<T>();
        instance.OnSingletonInitialize();
        return instance;
    }
}
