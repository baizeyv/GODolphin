#if TOOLS
using Godot;
using System;
namespace GODolphin.Pool;

[Tool]
public partial class DolphinPool : EditorPlugin
{
	public override void _EnterTree()
	{
		// Initialization of the plugin goes here.
	}

	public override void _ExitTree()
	{
		// Clean-up of the plugin goes here.
	}
}
#endif
