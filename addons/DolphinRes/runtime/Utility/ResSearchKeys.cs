using System;
using GODolphin.Pool;

namespace GODolphin.Res;

public class ResSearchKeys : IPoolable
{
    /// <summary>
    /// * 资源名称 (Lower)
    /// </summary>
    public string AssetName { get; set; }

    /// <summary>
    /// * 资源类型 (For example: Sprite)
    /// </summary>
    public Type AssetType { get; set; }

    public static ResSearchKeys Create(string assetName, Type assetType = null)
    {
        var key = SafeObjectPool<ResSearchKeys>.Instance.Obtain();
        key.AssetName = assetName.ToLower();
        key.AssetType = assetType;
        return key;
    }

    public bool Match(IRes res)
    {
        if (res.AssetName != AssetName)
            return false;
        var isMatch = true;
        if (AssetType != null)
        {
            isMatch = res.AssetType == AssetType;
        }

        return isMatch;
    }

    public void Free()
    {
        SafeObjectPool<ResSearchKeys>.Instance.Free(this);
    }

    /// <summary>
    /// * Free 方法会调用Reset
    /// </summary>
    public void Reset()
    {
        AssetName = null;
        AssetType = null;
    }

    public override string ToString()
    {
        return $"AssetName: {AssetName}, AssetType: {AssetType}";
    }
}
