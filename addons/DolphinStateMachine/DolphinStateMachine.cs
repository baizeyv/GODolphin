#if TOOLS
using Godot;
using System;

[Tool]
public partial class DolphinStateMachine : EditorPlugin
{
	private Control _stateMachineWindowInstance;

	public override void _EnterTree()
	{
        GD.PrintRich("[color=yellow][b]Load Addon -> DolphinStateMachine ![/b][/color]");
        HookDock();
	}

	private void HookDock()
	{
		PackedScene stateMachineWindowScene = GD.Load<PackedScene>("res://addons/DolphinStateMachine/prefab/StateMachineWindow.tscn");
		_stateMachineWindowInstance = stateMachineWindowScene.Instantiate<Control>();

		AddControlToDock(DockSlot.RightBl, _stateMachineWindowInstance);
		_stateMachineWindowInstance.Name = "StateMachineConsole";

        AddAutoloadSingleton("StateMachineSharedBuffer", "res://addons/DolphinStateMachine/script/StateMachineSharedBuffer.cs");
	}

	private void UnHookDock()
	{
		RemoveControlFromDocks(_stateMachineWindowInstance);
		_stateMachineWindowInstance.QueueFree();

		RemoveAutoloadSingleton("StateMachineSharedBuffer");
	}

	public override void _Ready()
	{
        GD.PrintRich("[color=green][b]Addon Ready -> DolphinStateMachine ![/b][/color]");
	}

	public override void _ExitTree()
	{
        GD.PrintRich("[color=red][b]Unload Addon -> DolphinStateMachine ![/b][/color]");
        UnHookDock();
	}
}
#endif
