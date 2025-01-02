using Godot;
using Godot.Collections;

namespace GODolphin.Res;

public partial class ResStruct : GodotObject
{
    public string AssetName;

    public string ResState;

    public int RefCount;

    public string Stringify()
    {
        Dictionary dic = new Dictionary()
        {
            ["ResName"] = AssetName,
            ["ResState"] = ResState,
            ["RefCount"] = RefCount.ToString()
        };
        return Json.Stringify(dic);
    }

    public static ResStruct ParseString(string str)
    {
        var dic = Json.ParseString(str).AsGodotDictionary();
        return new ResStruct()
        {
            AssetName = (string)dic["ResName"],
            ResState = (string)dic["ResState"],
            RefCount = (int)dic["RefCount"]
        };
    }
}