#if TOOLS
using Godot;
using System;

[Tool]
public partial class DolphinSingleton : EditorPlugin
{
	public override void _EnterTree()
	{
		AddAutoloadSingleton("SingletonManager", "res://addons/DolphinSingleton/runtime/SingletonManager.cs");
	}

	public override void _ExitTree()
	{
		// Clean-up of the plugin goes here.
		RemoveAutoloadSingleton("SingletonManager");
	}
}
#endif
