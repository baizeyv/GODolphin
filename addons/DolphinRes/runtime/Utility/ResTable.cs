using System.Collections.Generic;
using System.Linq;
using Godot;
using Godot.Collections;

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

    public Dictionary Dictionarify()
    {
        var list = GetEnumerator();
        Dictionary dic = new();
        while (list.MoveNext())
        {
            var item = (Res)list.Current;
            var resStruct = new ResStruct()
            {
                AssetName = item.AssetName,
                ResState = item.State.ToString(),
                RefCount = item.RefCount
            };
            var str = resStruct.Stringify();
            dic.Add(item.AssetName, str);
        }
        list.Dispose();
        return dic;
    }
}
