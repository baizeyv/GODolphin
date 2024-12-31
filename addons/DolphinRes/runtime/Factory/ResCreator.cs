
namespace GODolphin.Res;

public interface IResCreator
{
    IRes Create(ResSearchKeys key);

    bool Match(ResSearchKeys key);
}

public class InternalResCreator : IResCreator
{
    public IRes Create(ResSearchKeys key)
    {
        var internalRes = InternalRes.Create(key.AssetName);
        internalRes.AssetType = key.AssetType;
        return internalRes;
    }

    public bool Match(ResSearchKeys key)
    {
        return key.AssetName.StartsWith(ResFlag.InternalResProtocol);
    }
}
