using System.Collections.Generic;
using System.Linq;
using GODolphin.Singleton;
using Godot;

namespace GODolphin.Res;

[NodePath("[Resource-Manager]")]
public partial class ResourceManager : NodeSingleton<ResourceManager>
{
    internal ResTable _resTable = new();

    private bool _isResMapDirty;

    private LinkedList<IAsyncTask> _asyncTasks = new();

    private int _currentAsyncThreadCount;

    private const int MaxAsyncThreadCount = 8;

    private ResourceManager() { }

    public override void OnSingletonInitialize()
    {
        ResFactory.AddCreator(new InternalResCreator());
    }

    public IRes GetRes(ResSearchKeys resSearchKeys, bool createNew = false)
    {
        var res = _resTable.GetResBySearchKeys(resSearchKeys);
        if (res != null)
            return res;

        if (!createNew)
        {
            GD.Print($"Create New Res FALSE");
            return null;
        }

        res = ResFactory.Create(resSearchKeys);
        if (res != null)
        {
            _resTable.Add(res);
        }

        ResSharedBuffer.Instance.SendTable(_resTable);
        return res;
    }

    public T GetRes<T>(ResSearchKeys key)
        where T : class, IRes
    {
        return GetRes(key) as T;
    }

    public override void _Process(double delta)
    {
        if (_isResMapDirty)
            RemoveUnusedRes();
    }

    private void RemoveUnusedRes()
    {
        if (!_isResMapDirty)
            return;

        _isResMapDirty = false;

        foreach (var res in _resTable.ToArray())
        {
            if (res.RefCount <= 0 && res.State != ResState.Loading)
            {
                if (res.ReleaseRes())
                {
                    _resTable.Remove(res);
                    res.Free();
                }
            }
        }
        ResSharedBuffer.Instance.SendTable(_resTable);
    }

    public void ClearOnUpdate()
    {
        _isResMapDirty = true;
    }

    public void PushAsyncTask(IAsyncTask task)
    {
        if (task == null)
            return;

        _asyncTasks.AddLast(task);
        TryStartNextAsyncTask();
    }

    private void TryStartNextAsyncTask()
    {
        if (_asyncTasks.Count == 0)
            return;

        if (_currentAsyncThreadCount >= MaxAsyncThreadCount)
            return;

        var task = _asyncTasks.First.Value;
        _asyncTasks.RemoveFirst();

        ++_currentAsyncThreadCount;
        task.DoLoadAsync(OnAsyncTaskFinish);
    }

    private void OnAsyncTaskFinish()
    {
        --_currentAsyncThreadCount;
        TryStartNextAsyncTask();
    }
}
