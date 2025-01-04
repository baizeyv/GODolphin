#if TOOLS
using Godot;
using System.Collections.Generic;

[Tool]
public partial class DolphinUI : EditorPlugin
{
    private PopupMenu _menu;

    private Node _sceneTreeDock;

    private UIEditorWindow _uiEditor;

    public override void _EnterTree()
    {
        AddToolMenuItem("UIEditorSetting", new Callable(this, nameof(OpenUIEditorSetting)));

        var baseControl = EditorInterface.Singleton.GetBaseControl();
        // * node 名称见 scene_tree_dock.cpp scene_tree_dock.h
        _sceneTreeDock = baseControl.FindChild("Scene", true, false);
        var sceneMenu = FindChildrenByType<PopupMenu>(_sceneTreeDock);
        _menu = ((PopupMenu)sceneMenu[0]);
        _menu.Connect("menu_changed", new Callable(this, nameof(OnMenuChanged)));
        _menu.Connect("id_pressed", new Callable(this, nameof(OnIDPressed)));
    }

    private void OnMenuChanged()
    {
        if (_menu.ItemCount == 0)
        {
            return;
        }
        else if (_menu.ItemCount == 1)
        {
            _menu.AddItem("UI Editor", 99); // * id 不能超过100
        }
    }

    private void OnSceneChanged()
    {

    }

    private void OnIDPressed(long id)
    {
        if (id == 99)
        {
            var currentScene = EditorInterface.Singleton.GetEditedSceneRoot();
            if (currentScene != null)
            {
                var path = currentScene.SceneFilePath;
                var scene = GD.Load<PackedScene>(path);

                PackedScene uiEditor = GD.Load<PackedScene>("res://addons/DolphinUI/prefab/UIEditorWindow.tscn");
                _uiEditor = uiEditor.Instantiate<UIEditorWindow>();
                _uiEditor.Name = "UI Editor";

                _uiEditor.SetupHierarchy(scene);

                AddControlToDock(DockSlot.LeftUl, _uiEditor);
            }
        }
    }

    private List<Node> FindChildrenByType<T>(Node node)
    {
        List<Node> ret = new List<Node>();
        foreach (var child in node.GetChildren())
        {
            if (child.GetType() == typeof(T))
            {
                ret.Add(child);
            }
        }

        return ret;
    }

    private void OpenUIEditorSetting()
    {
        // TODO:
    }

    private string curSceneFile = "";

    public override void _Process(double delta)
    {
        var file = EditorInterface.Singleton.GetEditedSceneRoot()?.SceneFilePath;
        if (!curSceneFile.Equals(file))
        {
            if (_uiEditor != null)
            {
                _uiEditor.QueueFree();
                _uiEditor = null;
            }
            curSceneFile = file;
        }
    }

    public override void _ExitTree()
    {
        RemoveToolMenuItem("UIEditorSetting");
        _menu.Disconnect("menu_changed", new Callable(this, nameof(OnMenuChanged)));
        _menu.Disconnect("id_pressed", new Callable(this, nameof(OnIDPressed)));
        _uiEditor?.QueueFree();
        _uiEditor = null;
    }
}
#endif