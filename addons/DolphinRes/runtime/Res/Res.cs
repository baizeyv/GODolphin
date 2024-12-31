using System;
using System.Threading.Tasks;
using GODolphin.Pool;
using Godot;

namespace GODolphin.Res;

public class Res : SimplerRefCounter, IRes, IPoolable
{
    private string _assetName;
    private ResState _resState = ResState.Idle;
    private GodotObject _asset;

    private event Action<bool, IRes> _onResLoadDoneEvent;

    public string AssetName
    {
        get => _assetName;
        protected set => _assetName = value;
    }

    public ResState State
    {
        get { return _resState; }
        set
        {
            _resState = value;
            if (_resState == ResState.Ready)
            {
                NotifyResLoadDoneEvent(true);
            }
        }
    }

    public Type AssetType { get; set; }

    public GodotObject Asset
    {
        get => _asset;
        protected set { _asset = value; }
    }

    public float Progress
    {
        get
        {
            switch (_resState)
            {
                case ResState.Loading:
                    return CalculateProgress();
                case ResState.Ready:
                    return 1;
            }

            return 0;
        }
    }

    public Res() { }

    protected Res(string assetName)
    {
        _assetName = assetName;
    }

    public void SubscribeOnResLoadDoneEvent(Action<bool, IRes> listener)
    {
        if (listener == null)
            return;
        if (_resState == ResState.Ready)
        {
            listener(true, this);
            return;
        }

        _onResLoadDoneEvent += listener;
    }

    public void UnsubscribeOnResLoadDoneEvent(Action<bool, IRes> listener)
    {
        if (listener == null)
        {
            return;
        }
        if (_onResLoadDoneEvent == null)
            return;
        _onResLoadDoneEvent -= listener;
    }

    protected virtual float CalculateProgress()
    {
        return 0;
    }

    /// <summary>
    /// * 资源加载失败时执行
    /// </summary>
    protected void OnResLoadFailed()
    {
        _resState = ResState.Idle;
        NotifyResLoadDoneEvent(false);
    }

    /// <summary>
    /// * 检测资源是否可以加载
    /// </summary>
    /// <returns></returns>
    protected bool CheckLoadable()
    {
        return _resState == ResState.Idle;
    }

    public virtual void LoadSync() { }

    public virtual void LoadAsync() { }

    public virtual string[] GetDependenciesList()
    {
        return null;
    }

    public bool IsDependenciesLoadFinish()
    {
        var depends = GetDependenciesList();
        if (depends == null || depends.Length == 0)
            return true;

        for (var i = depends.Length - 1; i >= 0; --i)
        {
            var rule = ResSearchKeys.Create(depends[i]);
            var res = ResourceManager.Instance.GetRes(rule);
            rule.Free();

            if (res == null || res.State != ResState.Ready)
            {
                return false;
            }
        }

        return true;
    }

    public bool ReleaseRes()
    {
        if (_resState == ResState.Loading)
            return false;
        if (_resState != ResState.Ready)
            return true;

        OnReleaseRes();
        _resState = ResState.Idle;
        _onResLoadDoneEvent = null;
        return true;
    }

    protected virtual void OnReleaseRes() { }

    public virtual void Free() { }

    public virtual void Reset()
    {
        Asset = null;
        _resState = ResState.Idle;
        _assetName = null;
        _onResLoadDoneEvent = null;
    }

    private void NotifyResLoadDoneEvent(bool result)
    {
        if (_onResLoadDoneEvent == null)
            return;
        _onResLoadDoneEvent(result, this);
        _onResLoadDoneEvent = null;
    }

    protected override void OnZeroRef()
    {
        if (_resState == ResState.Loading)
            return;
        ReleaseRes();
    }

    public virtual async Task DoLoadAsync(System.Action callback)
    {
        callback?.Invoke();
    }

    public override string ToString()
    {
        return $"Name: {_assetName}, State: {_resState}, RefCount:{RefCount}";
    }
}
