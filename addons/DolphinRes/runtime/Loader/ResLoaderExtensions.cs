using System;
using Godot;

namespace GODolphin.Res;

public static class ResLoaderExtensions
{
    public static T LoadSync<T>(this IResLoader self, string assetName)
        where T : GodotObject
    {
        var key = ResSearchKeys.Create(assetName, typeof(T));
        var ret = self.LoadSync(key) as T;
        key.Free();
        return ret;
    }

    public static void EnqueueLoad(
        this IResLoader self,
        string assetName,
        Action<bool, IRes> listener = null,
        bool lastOrder = true
    )
    {
        var key = ResSearchKeys.Create(assetName);
        self.EnqueueLoad(key, listener, lastOrder);
        key.Free();
    }

    public static void EnqueueLoad<T>(
        this IResLoader self,
        string assetName,
        Action<bool, IRes> listener = null,
        bool lastOrder = true
    )
    {
        var key = ResSearchKeys.Create(assetName, typeof(T));
        self.EnqueueLoad(key, listener, lastOrder);
        key.Free();
    }
}
