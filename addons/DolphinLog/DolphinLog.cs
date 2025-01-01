#if TOOLS
using Godot;
namespace GODolphin.Log;

[Tool]
public partial class DolphinLog : EditorPlugin
{
    private Control _outputWindowInstance;

    public override void _EnterTree()
    {
        GD.PrintRich("[color=yellow][b]Load Addon -> DolphinLog ![/b][/color]");
        HookDock();
    }

    private void HookDock()
    {
        PackedScene outputWindowScene =
            GD.Load("res://addons/DolphinLog/prefab/OutputWindow.tscn") as PackedScene;
        _outputWindowInstance = outputWindowScene.Instantiate() as Control;

        AddControlToBottomPanel(_outputWindowInstance, "Console");
        // AddControlToDock(DockSlot.RightUr, _outputWindowInstance);
        // _outputWindowInstance.Name = "Console";
        AddAutoloadSingleton("LogSharedBuffer", "res://addons/DolphinLog/script/LogSharedBuffer.cs");
    }

    private void UnHookDock()
    {
        RemoveControlFromBottomPanel(_outputWindowInstance);
        _outputWindowInstance.QueueFree();
        // RemoveControlFromDocks(_outputWindowInstance);
        // _outputWindowInstance.QueueFree();
        RemoveAutoloadSingleton("LogSharedBuffer");
    }

    public override void _Ready()
    {
        GD.PrintRich("[color=green][b]Addon Ready -> DolphinLog ![/b][/color]");
    }

    public override void _ExitTree()
    {
        GD.PrintRich("[color=red][b]Unload Addon -> DolphinLog ![/b][/color]");
        UnHookDock();
    }
}
#endif
