#if TOOLS
using Godot;
using System;
using GODolphin.Singleton;

[Tool]
public partial class DolphinSingleton : EditorPlugin
{
	private Control _singletonWindowInstance;
	public override void _EnterTree()
	{
		AddAutoloadSingleton("SingletonManager", "res://addons/DolphinSingleton/runtime/SingletonManager.cs");
		HookDock();
	}

	private void HookDock()
	{
		PackedScene stateMachineWindowScene = GD.Load<PackedScene>("res://addons/DolphinSingleton/prefab/SingletonWindow.tscn");
		_singletonWindowInstance = stateMachineWindowScene.Instantiate<Control>();

		AddControlToDock(DockSlot.RightBl, _singletonWindowInstance);
		_singletonWindowInstance.Name = "SingletonConsole";
	}

	private void UnHookDock()
	{
		RemoveControlFromDocks(_singletonWindowInstance);
		_singletonWindowInstance.QueueFree();
		RemoveAutoloadSingleton("SingletonManager");
	}

	public override void _ExitTree()
	{
		UnHookDock();
	}
}
#endif
