using System.Collections.Generic;
using System.Linq;
using GODolphin.Pool;
using Godot;

namespace GODolphin.Res;

/// <summary>
/// * 资源工厂
/// </summary>
public static class ResFactory
{
    private static List<IResCreator> _resCreators = ListPool<IResCreator>.Obtain();

    public static IRes Create(ResSearchKeys key)
    {
        var retRes = _resCreators
            .Where(creator => creator.Match(key))
            .Select(creator => creator.Create(key))
            .FirstOrDefault();
        if (retRes != null)
            return retRes;
        GD.Print($"Failed to create res. Not find by ResSearchKeys: {key}");
        return null;
    }

    public static void AddCreator(IResCreator creator)
    {
        _resCreators.Add(creator);
    }

    public static void AddCreator<T>()
        where T : IResCreator, new()
    {
        _resCreators.Add(new T());
    }

    public static void RemoveCreator<T>()
    {
        _resCreators.RemoveAll(item => item.GetType() == typeof(T));
    }
}
