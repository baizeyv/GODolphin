using Godot;

namespace GODolphin.Singleton;

[Tool]
public partial class SingletonResource : Resource
{
    [Export]
    public string[] ManagerTypeArray;
}