#if TOOLS
using Godot;

namespace TexturePackerImporter;
[Tool]
public partial class TexturePackerImporter : EditorPlugin
{
	private TPImporter _importer;
	public override void _EnterTree()
	{
		_importer = new TPImporter();
		AddImportPlugin(_importer);
	}

	public override void _ExitTree()
	{
		RemoveImportPlugin(_importer);
		_importer = null;
	}

}
#endif
