#if TOOLS
using Godot;
using System;

namespace GODolphin.Res;
[Tool]
public partial class DolphinRes : EditorPlugin
{

	private Control _resWindowInstance;

	public override void _EnterTree()
	{
        GD.PrintRich("[color=yellow][b]Load Addon -> DolphinRes ![/b][/color]");
		HookDock();
	}

	private void HookDock()
	{
		PackedScene resWindowScene = GD.Load<PackedScene>("res://addons/DolphinRes/prefab/ResWindow.tscn");
		_resWindowInstance = resWindowScene.Instantiate<Control>();

		AddControlToDock(DockSlot.RightBl, _resWindowInstance);
		_resWindowInstance.Name = "ResourceConsole";

		AddAutoloadSingleton("ResSharedBuffer", "res://addons/DolphinRes/script/ResSharedBuffer.cs");
	}

	private void UnHookDock()
	{
		RemoveControlFromDocks(_resWindowInstance);
		_resWindowInstance.QueueFree();

		RemoveAutoloadSingleton("ResSharedBuffer");
	}

	public override void _Ready()
	{
        GD.PrintRich("[color=green][b]Addon Ready -> DolphinRes ![/b][/color]");
	}

	public override void _ExitTree()
	{
        GD.PrintRich("[color=red][b]Unload Addon -> DolphinRes ![/b][/color]");
		UnHookDock();
	}
}
#endif
