using Godot;
using Godot.Collections;

namespace GODolphin.UI;

[Tool]
public partial class UIEditorWindow : Control
{
    [Export] public Tree HierarchyTree;

    [Export] public Texture2D TitleTexture;

    [Export] public Texture2D BoundTexture;

    [Export] public Texture2D NormalTexture;

    [Export] public Texture2D UnbindButtonTexture;

    [Export] public Texture2D NormalBindTexture;

    [Export] public Texture2D ComponentBindTexture;

    [Export] public Button GenerateButton;

    [Export] public Button RefreshButton;

    private PopupMenu _unbindPopupMenu;

    private const int IconMaxWidth = 20;

    private Dictionary<string, TreeItem> _treeItemMap = new();

    private Dictionary<string, Node> _nodeMap = new();

    /// <summary>
    /// * Key: NodePath
    /// </summary>
    private Dictionary<string, Dictionary<string, string>> _bindMap = new();

    private Dictionary<string, string> _customTypeMap = new();

    private TreeItem _componentBindClickTreeItem;

    private AcceptDialog _dialog;

    private LineEdit _lineEdit;

    private string _uiBindResPath;

    public Vector2I DialogSize = new(300, 100);

    private PackedScene _scene;

    public override void _EnterTree()
    {
        _dialog = new();
        _dialog.OkButtonText = "Bind";
        _dialog.Title = "Component Bind";
        _dialog.SetSize(DialogSize);
        _dialog.Unresizable = true;
        var screenPosition = DisplayServer.Singleton.ScreenGetPosition();
        var screenSize = DisplayServer.Singleton.ScreenGetSize();
        _dialog.Position = new Vector2I(screenPosition.X + screenSize.X / 2 - DialogSize.X / 2,
            screenPosition.Y + screenSize.Y / 2 - DialogSize.Y / 2);
        AddChild(_dialog);
        _dialog.Hide();

        _lineEdit = new();
        _lineEdit.AnchorLeft = 0f;
        _lineEdit.AnchorRight = 1f;
        _lineEdit.AnchorTop = 0.2f;
        _lineEdit.AnchorBottom = 0.8f;

        _dialog.AddChild(_lineEdit);

        _dialog.Connect("confirmed", new Callable(this, nameof(OnDialogConfirmed)));
        HierarchyTree.Connect("button_clicked", new Callable(this, nameof(OnTreeButtonClicked)));
        HierarchyTree.Connect("item_selected", new Callable(this, nameof(OnTreeItemSelected)));
        RefreshButton.Connect("pressed", new Callable(this, nameof(OnRefreshButtonClicked)));
        GenerateButton.Connect("pressed", new Callable(this, nameof(OnGenerateButtonClicked)));
    }

    public void SetupHierarchy(PackedScene scene)
    {
        _scene = scene;
        HierarchyTree.Columns = 3;
        HierarchyTree.SelectMode = Tree.SelectModeEnum.Row;
        var path = scene.ResourcePath;
        _uiBindResPath = path.Substring(0, path.Length - 4) + "tres";

        if (FileAccess.FileExists(_uiBindResPath))
        {
            // * 文件存在
            var uiBindRes = GD.Load<UIBindRes>(_uiBindResPath);
            _bindMap = uiBindRes.Map;
            _customTypeMap = uiBindRes.CustomTypeMap;
        }

        var obj = scene.Instantiate();
        var root = HierarchyTree.CreateItem();
        root.SetText(0, obj.Name);
        root.SetText(1, "Node Type");
        root.SetText(2, "Bind Type");
        root.SetIcon(0, TitleTexture);
        root.SetIconMaxWidth(0, IconMaxWidth);
        HandleRecursive(obj, root);
    }

    private void OnTreeItemSelected()
    {
        var item = HierarchyTree.GetSelected();
        var nodePath = GetNodePathByTreeItem(item);
        OnSelectNode(nodePath);
    }

    private void OnDialogConfirmed()
    {
        if (string.IsNullOrEmpty(_lineEdit.Text))
            return;
        var item = _componentBindClickTreeItem;
        var nodePath = GetNodePathByTreeItem(item);
        if (_bindMap.TryGetValue(nodePath, out var bind) && !string.IsNullOrEmpty(bind[GODolphinConst.TYPE_FULL_NAME]))
        {
            // * 已有绑定
        }
        else
        {
            // * 无绑定
            if (_nodeMap.ContainsKey(nodePath))
            {
                var typeString = GetNamespace() + "." + _lineEdit.Text.Trim();
                Dictionary<string, string> dic = new();
                dic.Add(GODolphinConst.TYPE_FULL_NAME, typeString);
                dic.Add(GODolphinConst.TYPE_IS_CUSTOM, GODolphinConst.TRUE_STRING);
                dic.Add(GODolphinConst.TYPE_SELF, _nodeMap[nodePath].GetType().Name);
                _bindMap.Add(nodePath, dic);
                _customTypeMap.Add(typeString, nodePath);
                item.SetText(2, typeString);
                item.SetIcon(0, BoundTexture);
                item.EraseButton(2, 0);
                item.EraseButton(2, 0);
                item.AddButton(2, UnbindButtonTexture, 2);
                SaveResource();
            }
        }

        _lineEdit.Text = "";
    }

    private string GetNamespace()
    {
        var config = new ConfigFile();
        var err = config.Load(GODolphinConst.GODOLPHIN_CONFIG);
        if (err == Error.Ok)
        {
            return (string)config.GetValue(GODolphinConst.UISettingSection, GODolphinConst.UINamespaceKey,
                GODolphinConst.UIDefaultNamespace);
        }
        return GODolphinConst.UIDefaultNamespace;
    }

    private string GetNodePathByTreeItem(TreeItem item)
    {
        foreach (var kvp in _treeItemMap)
        {
            if (kvp.Value == item)
            {
                return kvp.Key;
            }
        }

        return "";
    }

    private void SaveResource()
    {
        var res = new UIBindRes();
        res.Map = _bindMap;
        res.CustomTypeMap = _customTypeMap;
        ResourceSaver.Save(res, _uiBindResPath);
    }

    private void HandleRecursive(Node node, TreeItem curRoot, string rootPath = "")
    {
        foreach (var child in node.GetChildren())
        {
            string path = rootPath;
            if (string.IsNullOrEmpty(path))
            {
                path = child.Name;
            }
            else
            {
                path += "/" + child.Name;
            }

            var item = HierarchyTree.CreateItem(curRoot);
            if (_bindMap.ContainsKey(path))
            {
                // * 已存在绑定
                item.SetText(0, child.Name);
                item.SetText(1, child.GetType().Name);
                item.SetText(2, _bindMap[path][GODolphinConst.TYPE_FULL_NAME]);
                item.SetIcon(0, BoundTexture);
                item.SetIconMaxWidth(0, IconMaxWidth);
                item.AddButton(2, UnbindButtonTexture, 2);
            }
            else
            {
                // * 不存在绑定
                item.SetText(0, child.Name);
                item.SetText(1, child.GetType().Name);
                item.SetIcon(0, NormalTexture);
                item.SetIconMaxWidth(0, IconMaxWidth);
                item.AddButton(2, NormalBindTexture, 0);
                item.AddButton(2, ComponentBindTexture, 1);
            }

            _treeItemMap[path] = item;

            _nodeMap[path] = child;

            HandleRecursive(child, item, path);
        }
    }

    private void OnTreeButtonClicked(TreeItem item, int column, int id, int mouseButtonIndex)
    {
        if (mouseButtonIndex == MouseButton.Left.GetHashCode())
        {
            if (column == 2)
            {
                var nodePath = GetNodePathByTreeItem(item);
                if (_bindMap.TryGetValue(nodePath, out var bind) && !string.IsNullOrEmpty(bind[GODolphinConst.TYPE_FULL_NAME]))
                {
                    // * 已有绑定, 点击后取消绑定
                    if (id == 2)
                    {
                        item.EraseButton(2, 0);
                        _bindMap.Remove(nodePath);
                        if (_customTypeMap.ContainsKey(item.GetText(2)))
                        {
                            _customTypeMap.Remove(item.GetText(2));
                        }
                        item.SetIcon(0, NormalTexture);
                        item.SetText(2, "");
                        item.AddButton(2, NormalBindTexture, 0);
                        item.AddButton(2, ComponentBindTexture, 1);
                        SaveResource();
                    }
                }
                else
                {
                    // * 无绑定
                    if (id == 0)
                    {
                        // * 无绑定
                        // * 进行普通绑定
                        if (_nodeMap.ContainsKey(nodePath))
                        {
                            var typeString = _nodeMap[nodePath].GetType().FullName;
                            Dictionary<string, string> dic = new();
                            dic.Add(GODolphinConst.TYPE_FULL_NAME, typeString);
                            dic.Add(GODolphinConst.TYPE_IS_CUSTOM, GODolphinConst.FALSE_STRING);
                            dic.Add(GODolphinConst.TYPE_SELF, _nodeMap[nodePath].GetType().Name);
                            _bindMap.Add(nodePath, dic);
                            item.SetText(2, typeString);
                            item.SetIcon(0, BoundTexture);
                            item.EraseButton(2, 0);
                            item.EraseButton(2, 0);
                            item.AddButton(2, UnbindButtonTexture, 2);
                            SaveResource();
                        }
                    }
                    else if (id == 1)
                    {
                        // * 进行特殊绑定
                        _componentBindClickTreeItem = item;
                        _dialog.Show();
                    }
                }
            }
        }
    }

    private void OnSelectNode(string nodePath)
    {
        var sceneRoot = EditorInterface.Singleton.GetEditedSceneRoot();
        var selection = EditorInterface.Singleton.GetSelection();
        var nd = sceneRoot.GetNodeOrNull(nodePath);
        if (nd == null)
            return;
        selection.Clear();
        selection.AddNode(nd);
    }

    private void OnGenerateButtonClicked()
    {
        if (string.IsNullOrEmpty(_uiBindResPath))
        {
            return;
        }

        if (!FileAccess.FileExists(_uiBindResPath))
        {
            SaveResource();
        }

        var uiBindRes = GD.Load<UIBindRes>(_uiBindResPath);
        uiBindRes.Generate(_scene.ResourcePath);
    }

    private void OnRefreshButtonClicked()
    {
        HierarchyTree.Clear();
        SetupHierarchy(_scene);
    }

    public override void _ExitTree()
    {
        HierarchyTree?.Disconnect("button_clicked", new Callable(this, nameof(OnTreeButtonClicked)));
        HierarchyTree?.Disconnect("item_selected", new Callable(this, nameof(OnTreeItemSelected)));
        RefreshButton.Disconnect("pressed", new Callable(this, nameof(OnRefreshButtonClicked)));
        GenerateButton.Disconnect("pressed", new Callable(this, nameof(OnGenerateButtonClicked)));
        _dialog?.Disconnect("confirmed", new Callable(this, nameof(OnDialogConfirmed)));
    }
}