using Godot;

namespace GODolphin.Singleton;
public static class Global
{
    public static SceneTree Tree { get; private set; }

    public static void Initialize(SceneTree tree)
    {
        Tree = tree;
    }
}
