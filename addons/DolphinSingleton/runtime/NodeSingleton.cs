using Godot;

namespace GODolphin.Singleton;

public abstract partial class NodeSingleton<T> : Node, ISingleton
    where T : NodeSingleton<T>
{
    protected static T _instance;

    public static T Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = Global.Tree.Root.GetNode<T>(
                    $"/root/SingletonManager/{SingletonManager.Instance.GetInsName<T>()}"
                );
                _instance.OnSingletonInitialize();
            }
            return _instance;
        }
    }

    public virtual void OnSingletonInitialize() { }

    public override void _ExitTree()
    {
        base._ExitTree();
        _instance = null;
    }
}
