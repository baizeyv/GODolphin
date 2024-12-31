using System;
using Godot;

namespace GODolphin.Res;

/// <summary>
/// * 资源加载器
/// </summary>
public interface IResLoader
{
    /// <summary>
    /// * 同步加载
    /// </summary>
    /// <param name="resSearchKeys"></param>
    /// <returns></returns>
    GodotObject LoadSync(ResSearchKeys resSearchKeys);

    /// <summary>
    /// * 入队加载
    /// </summary>
    /// <param name="resSearchKeys"></param>
    /// <param name="listener"></param>
    /// <param name="lastOrder"></param>
    void EnqueueLoad(
        ResSearchKeys resSearchKeys,
        Action<bool, IRes> listener = null,
        bool lastOrder = true
    );

    /// <summary>
    /// * 异步加载
    /// </summary>
    /// <param name="listener"></param>
    void LoadAsync(System.Action listener = null);

    /// <summary>
    /// * 释放当前资源加载器所持有的所有资源
    /// </summary>
    void ReleaseAllRes();
}
