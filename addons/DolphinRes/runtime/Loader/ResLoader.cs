using System;
using System.Collections.Generic;
using GODolphin.Pool;
using Godot;

namespace GODolphin.Res;

public class ResLoader : IPoolable, IResLoader
{
    private readonly List<IRes> _resList = new();

    private int _loadingCount;

    private readonly LinkedList<IRes> _waitLoadList = new();

    private List<Node> _node2Unload;

    private LinkedList<CallbackWrap> _callbackRecordList;

    private System.Action _listener;

    [Obsolete("请使用 ResLoader.Create() 获取 ResLoader 对象", true)]
    public ResLoader() { }

    public static ResLoader Create()
    {
        return SafeObjectPool<ResLoader>.Instance.Obtain();
    }

    public void Free()
    {
        if (_node2Unload != null)
        {
            foreach (var o in _node2Unload)
            {
                if (o != null)
                {
                    o.QueueFree();
                }
            }

            _node2Unload.Clear();
            _node2Unload.Free();
            _node2Unload = null;
        }

        SafeObjectPool<ResLoader>.Instance.Free(this);
    }

    public void Reset()
    {
        ReleaseAllRes();
    }

    /// <summary>
    /// * 在回收 ResLoader 的同时 QueueFree 指定节点
    /// </summary>
    /// <param name="node"></param>
    public void AddNodeForQueueFreeWhenFree(Node node)
    {
        if (_node2Unload == null)
            _node2Unload = ListPool<Node>.Obtain();

        _node2Unload.Add(node);
    }

    public GodotObject LoadSync(ResSearchKeys resSearchKeys)
    {
        EnqueueLoad(resSearchKeys);
        LoadSync();

        var res = ResourceManager.Instance.GetRes(resSearchKeys);
        if (res == null)
        {
            GD.PrintErr("Failed to load res: " + resSearchKeys);
            return null;
        }
        resSearchKeys.Free();
        return res.Asset;
    }

    private void LoadSync()
    {
        while (_waitLoadList.Count > 0)
        {
            var first = _waitLoadList.First.Value;
            --_loadingCount;
            _waitLoadList.RemoveFirst();
            if (first == null)
                return;

            first.LoadSync();
        }
    }

    public void EnqueueLoad(
        ResSearchKeys resSearchKeys,
        Action<bool, IRes> listener = null,
        bool lastOrder = true
    )
    {
        var res = FindResInArray(_resList, resSearchKeys);
        if (res != null)
        {
            if (listener != null)
            {
                AddResListenerRecord(res, listener);
                res.SubscribeOnResLoadDoneEvent(listener);
            }
            return;
        }

        res = ResourceManager.Instance.GetRes(resSearchKeys, true);

        if (res == null)
            return;
        if (listener != null)
        {
            AddResListenerRecord(res, listener);
            res.SubscribeOnResLoadDoneEvent(listener);
        }

        // * 无论该资源是否加载完成,都需要添加对该资源依赖的引用
        var depends = res.GetDependenciesList();
        if (depends != null)
        {
            foreach (var depend in depends)
            {
                var searchKey = ResSearchKeys.Create(depend);
                EnqueueLoad(searchKey);
                searchKey.Free();
            }
        }

        AddRes2Array(res, lastOrder);
    }

    public void LoadAsync(System.Action listener = null)
    {
        _listener = listener;
        DoLoadAsync();
    }

    private void DoLoadAsync()
    {
        if (_loadingCount == 0)
        {
            if (_listener != null)
            {
                var callback = _listener;
                _listener = null;
                callback?.Invoke();
            }

            return;
        }

        var nextNode = _waitLoadList.First;
        LinkedListNode<IRes> currentNode;
        while (nextNode != null)
        {
            currentNode = nextNode;
            var res = currentNode.Value;
            nextNode = currentNode.Next;
            if (res.IsDependenciesLoadFinish())
            {
                _waitLoadList.Remove(currentNode);
                if (res.State != ResState.Ready)
                {
                    res.SubscribeOnResLoadDoneEvent(OnResLoadFinish);
                    res.LoadAsync();
                }
                else
                {
                    --_loadingCount;
                }
            }
        }
    }

    private void OnResLoadFinish(bool result, IRes res)
    {
        --_loadingCount;
        DoLoadAsync();
        if (_loadingCount == 0)
        {
            RemoveAllCallback(false);
            _listener?.Invoke();
        }
    }

    private void RemoveAllCallback(bool release)
    {
        if (_callbackRecordList != null)
        {
            var count = _callbackRecordList.Count;
            while (count > 0)
            {
                --count;
                if (release)
                    _callbackRecordList.Last.Value.Free();
                _callbackRecordList.RemoveLast();
            }
        }
    }

    public void ReleaseAllRes()
    {
        _listener = null;
        _loadingCount = 0;
        _waitLoadList.Clear();
        if (_resList.Count > 0)
        {
            _resList.Reverse();

            for (var i = _resList.Count - 1; i >= 0; --i)
            {
                _resList[i].UnsubscribeOnResLoadDoneEvent(OnResLoadFinish);
                _resList[i].Release();
            }

            _resList.Clear();

            ResourceManager.Instance.ClearOnUpdate();
        }
        RemoveAllCallback(true);
    }

    private void AddRes2Array(IRes res, bool lastOrder)
    {
        var key = ResSearchKeys.Create(res.AssetName, res.AssetType);
        var oldRes = FindResInArray(_resList, key);
        key.Free();

        if (oldRes != null)
        {
            return;
        }

        res.Retain();
        _resList.Add(res);

        if (res.State != ResState.Ready)
        {
            ++_loadingCount;
            if (lastOrder)
            {
                _waitLoadList.AddLast(res);
            }
            else
            {
                _waitLoadList.AddFirst(res);
            }
        }
    }

    private static IRes FindResInArray(List<IRes> list, ResSearchKeys resSearchKeys)
    {
        if (list == null)
            return null;

        for (var i = list.Count - 1; i >= 0; --i)
        {
            if (resSearchKeys.Match(list[i]))
            {
                return list[i];
            }
        }

        return null;
    }

    private void AddResListenerRecord(IRes res, Action<bool, IRes> listener)
    {
        if (_callbackRecordList == null)
            _callbackRecordList = new();

        _callbackRecordList.AddLast(new CallbackWrap(res, listener));
    }

    class CallbackWrap
    {
        private readonly Action<bool, IRes> _listener;

        private readonly IRes _res;

        public CallbackWrap(IRes res, Action<bool, IRes> listener)
        {
            _res = res;
            _listener = listener;
        }

        public void Free()
        {
            _res.UnsubscribeOnResLoadDoneEvent(_listener);
        }

        public bool IsRes(IRes res)
        {
            return res.AssetName == _res.AssetName;
        }
    }
}
