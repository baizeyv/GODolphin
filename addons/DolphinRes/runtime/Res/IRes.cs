using System;
using Godot;

namespace GODolphin.Res;

public interface IRes : IRefCounter, IAsyncTask
{
    /// <summary>
    /// * 资源名称
    /// </summary>
    string AssetName { get; }

    /// <summary>
    /// * 资源类型
    /// </summary>
    Type AssetType { get; set; }

    /// <summary>
    /// * 资源本体
    /// </summary>
    GodotObject Asset { get; }

    /// <summary>
    /// * 资源状态
    /// </summary>
    ResState State { get; }

    /// <summary>
    /// * 加载进度
    /// </summary>
    float Progress { get; }

    /// <summary>
    /// * 同步加载
    /// </summary>
    void LoadSync();

    /// <summary>
    /// * 异步加载
    /// </summary>
    void LoadAsync();

    /// <summary>
    /// * 依赖资源列表
    /// </summary>
    /// <returns></returns>
    string[] GetDependenciesList();

    /// <summary>
    /// * 所有依赖资源是否加载完毕了
    /// </summary>
    /// <returns></returns>
    bool IsDependenciesLoadFinish();

    void SubscribeOnResLoadDoneEvent(Action<bool, IRes> listener);

    void UnsubscribeOnResLoadDoneEvent(Action<bool, IRes> listener);

    /// <summary>
    /// * 释放资源
    /// </summary>
    /// <returns></returns>
    bool ReleaseRes();

    /// <summary>
    /// * 回收
    /// </summary>
    void Free();
}
