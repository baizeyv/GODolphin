using System;
using System.Threading.Tasks;
using GODolphin.Pool;
using GODolphin.Singleton;
using Godot;
using Array = Godot.Collections.Array;

namespace GODolphin.Res;

public class InternalRes : Res
{
    private string _path;

    public static InternalRes Create(string name)
    {
        var res = SafeObjectPool<InternalRes>.Instance.Obtain();
        if (res != null)
        {
            res.AssetName = name;
        }

        res._path = name.Substring(ResFlag.InternalResProtocol.Length);
        return res;
    }

    public override void LoadSync()
    {
        if (!CheckLoadable())
        {
            // * 不可加载
            return;
        }

        if (string.IsNullOrEmpty(AssetName))
        {
            return;
        }

        State = ResState.Loading;

        if (AssetType != null)
        {
            Asset = ResourceLoader.Load(_path, AssetType.ToString());
        }
        else
        {
            Asset = ResourceLoader.Load(_path);
        }

        if (Asset == null)
        {
            GD.PrintErr($"Failed to load asset from Resource: {_path}");
            OnResLoadFailed();
            return;
        }

        State = ResState.Ready;
    }

    public override void LoadAsync()
    {
        if (!CheckLoadable())
        {
            // * 不可加载
            return;
        }

        if (string.IsNullOrEmpty(AssetName))
        {
            return;
        }

        State = ResState.Loading;
        ResourceManager.Instance.PushAsyncTask(this);
    }

    public override async Task DoLoadAsync(System.Action callback)
    {
        if (RefCount <= 0)
        {
            OnResLoadFailed();
            callback?.Invoke();
            return;
        }

        Error requestError;
        if (AssetType != null)
        {
            requestError = ResourceLoader.LoadThreadedRequest(_path, AssetType.ToString());
        }
        else
        {
            requestError = ResourceLoader.LoadThreadedRequest(_path);
        }

        if (requestError is not Error.Ok)
        {
            OnResLoadFailed();
            callback?.Invoke();
            GD.PrintErr(
                $"Failed to load resource at path: {_path} with error {requestError}, returning null."
            );
            return;
        }

        var status = ResourceLoader.ThreadLoadStatus.Failed;

        var sceneTree = Global.Tree;

        do
        {
            if (status == ResourceLoader.ThreadLoadStatus.InProgress)
            {
                // * 等待帧更新信号
                await sceneTree.ToSignal(sceneTree, SceneTree.SignalName.ProcessFrame);
            }

            status = ResourceLoader.LoadThreadedGetStatus(_path);
        } while (status == ResourceLoader.ThreadLoadStatus.InProgress);

        if (status != ResourceLoader.ThreadLoadStatus.Loaded)
        {
            OnResLoadFailed();
            callback?.Invoke();
            GD.PrintErr(
                $"Failed to load resource at path: {_path} with status {status}, returning null."
            );
            return;
        }
        Asset = ResourceLoader.LoadThreadedGet(_path);
        State = ResState.Ready;
        callback?.Invoke();
    }

    public override string[] GetDependenciesList()
    {
        return ResourceLoader.GetDependencies(_path);
    }

    protected override float CalculateProgress()
    {
        var array = new Array();
        ResourceLoader.LoadThreadedGetStatus(_path, array);
        return (float)array[0];
    }

    public override void Free()
    {
        SafeObjectPool<InternalRes>.Instance.Free(this);
    }

    public override string ToString()
    {
        return $"Type: InternalRes {AssetName}";
    }
}
