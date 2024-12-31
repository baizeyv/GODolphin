using System.Collections.Generic;
using System.Linq;

namespace GODolphin.Res;

public class ResTable : Table<IRes>
{
    public TableIndex<string, IRes> NameIndex = new(res => res.AssetName.ToLower());

    public IRes GetResBySearchKeys(ResSearchKeys keys)
    {
        var assetName = keys.AssetName;

        var resList = NameIndex.Get(assetName);

        if (keys.AssetType != null)
        {
            resList = resList.Where(res => res.AssetType == keys.AssetType);
        }

        return resList.FirstOrDefault();
    }

    protected override void OnAdd(IRes item)
    {
        NameIndex.Add(item);
    }

    protected override void OnRemove(IRes item)
    {
        NameIndex.Remove(item);
    }

    protected override void OnClear()
    {
        NameIndex.Clear();
    }

    public override IEnumerator<IRes> GetEnumerator()
    {
        return NameIndex.Dictionary.SelectMany(item => item.Value).GetEnumerator();
    }

    protected override void OnDispose() { }
}
