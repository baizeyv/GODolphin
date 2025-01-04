#if TOOLS
using Godot;
using System.Collections.Generic;
using GODolphin;

[Tool]
public partial class DolphinUI : EditorPlugin
{
    private PopupMenu _menu;

    private Node _sceneTreeDock;

    private UIEditorWindow _uiEditor;

    private ConfigFile _config = new();

    private LineEdit _pathEdit;

    private LineEdit _namespaceEdit;

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
        /////////////////////////////////////////////////
        var settingDialog = new AcceptDialog();
        settingDialog.OkButtonText = "Save";
        settingDialog.Unresizable = true;

        var vbox = new VBoxContainer();
        settingDialog.AddChild(vbox);
        Label pathLabel = new();
        pathLabel.Text = "ScriptPath:";
        vbox.AddChild(pathLabel);
        _pathEdit = new();
        vbox.AddChild(_pathEdit);
        Label namespaceLabel = new();
        namespaceLabel.Text = "Namespace:";
        vbox.AddChild(namespaceLabel);
        _namespaceEdit = new();
        vbox.AddChild(_namespaceEdit);

        settingDialog.Connect("confirmed", new Callable(this, nameof(OnDialogConfirmed)));
        var err = _config.Load(GODolphinConst.GODOLPHIN_CONFIG);
        string scriptPath = GODolphinConst.UIDefaultScriptPath, namespacestr = GODolphinConst.UIDefaultNamespace;
        if (err == Error.Ok)
        {
            scriptPath = (string)_config.GetValue(GODolphinConst.UISettingSection, GODolphinConst.UIScriptPathKey,
                GODolphinConst.UIDefaultScriptPath);
            namespacestr = (string)_config.GetValue(GODolphinConst.UISettingSection, GODolphinConst.UINamespaceKey,
                GODolphinConst.UIDefaultNamespace);
        }
        else if (err == Error.FileNotFound)
        {
            _config.SetValue(GODolphinConst.UISettingSection, GODolphinConst.UIScriptPathKey,
                GODolphinConst.UIDefaultScriptPath);
            _config.SetValue(GODolphinConst.UISettingSection, GODolphinConst.UINamespaceKey,
                GODolphinConst.UIDefaultNamespace);
            _config.Save(GODolphinConst.GODOLPHIN_CONFIG);
        }
        _pathEdit.Text = scriptPath;
        _namespaceEdit.Text = namespacestr;

        /////////////////////////////////////////////////
        EditorInterface.Singleton.PopupDialogCentered(settingDialog);
    }

    private void OnDialogConfirmed()
    {
        var err = _config.Load(GODolphinConst.GODOLPHIN_CONFIG);
        var pathStr = _pathEdit.Text[_pathEdit.Text.Length - 1] == '/' ? _pathEdit.Text : _pathEdit.Text + '/';
        _config.SetValue(GODolphinConst.UISettingSection, GODolphinConst.UIScriptPathKey,
            pathStr);
        _config.SetValue(GODolphinConst.UISettingSection, GODolphinConst.UINamespaceKey,
            _namespaceEdit.Text);
        _config.Save(GODolphinConst.GODOLPHIN_CONFIG);
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