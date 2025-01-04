using Godot;
using Godot.Collections;

namespace GODolphin.UI;

[Tool]
public partial class UIBindRes : Resource
{
    /// <summary>
    /// * Key: nodePath ||| Value: typeString
    /// * KEY LIST: GODolphinConst.TYPE_FULL_NAME, TYPE_PARENT, TYPE_IS_CUSTOM
    /// * TYPE_IS_CUSTOM 的值为 TRUE_STRING 或 FALSE_STRING
    /// </summary>
    [Export] public Dictionary<string, Dictionary<string, string>> Map;

    /// <summary>
    /// * Key: CustomType ||| Value: nodePath
    /// </summary>
    [Export] public Dictionary<string, string> CustomTypeMap;

    /// <summary>
    /// * 由以上两个Map整理后的Map, KEY->type_name, VALUE->[nodeType:...],[nodeName:...]
    /// * KEY LIST: GODolphinConst.KEY_NODE_NAME, KEY_NODE_TYPE
    /// </summary>
    private Dictionary<string, Array<Dictionary>> _arrangeMap;

    private string _mainTypeName;

    public void Generate(string scenePath)
    {
        // TODO:
        RebuildValue();
        var arr = scenePath.Split('/');
        var nameWithSuffix = arr[arr.Length - 1];
        _mainTypeName = nameWithSuffix.Substring(0, nameWithSuffix.Length - 5);

        Arrange();

        GenerateFiles();

        // TODO:
        // HandleBind(scriptFile);
    }

    private void Arrange()
    {
        _arrangeMap = new();
        foreach (var kvp in Map)
        {
            var dic = kvp.Value;
            if (dic[GODolphinConst.TYPE_IS_CUSTOM].Equals(GODolphinConst.TRUE_STRING))
            {
                var fullType = dic[GODolphinConst.TYPE_FULL_NAME];
                string type = "";
                if (fullType.Contains("."))
                {
                    var arr = fullType.Split('.');
                    type = arr[arr.Length - 1];
                }
                else
                {
                    type = fullType;
                }

                if (!_arrangeMap.ContainsKey(type))
                {
                    _arrangeMap.Add(type, new());
                }
            }
            if (dic.TryGetValue(GODolphinConst.TYPE_PARENT, out var parentFullType))
            {
                // * 存在自定义父类型
                if (parentFullType.Contains("."))
                {
                    var arr = parentFullType.Split('.');
                    var type = arr[arr.Length - 1];

                    var n = kvp.Key.Split('/');
                    var nodeName = n[n.Length - 1];

                    Dictionary info = new();
                    info.Add(GODolphinConst.KEY_NODE_NAME, nodeName);
                    info.Add(GODolphinConst.KEY_NODE_TYPE, dic[GODolphinConst.TYPE_FULL_NAME]);
                    if (_arrangeMap.ContainsKey(type))
                    {
                        _arrangeMap[type].Add(info);
                    }
                    else
                    {
                        Array<Dictionary> array = new();
                        array.Add(info);
                        _arrangeMap.Add(type, array);
                    }
                }
                else
                {
                    var n = kvp.Key.Split('/');
                    var nodeName = n[n.Length - 1];

                    Dictionary info = new();
                    info.Add(GODolphinConst.KEY_NODE_NAME, nodeName);
                    info.Add(GODolphinConst.KEY_NODE_TYPE, dic[GODolphinConst.TYPE_FULL_NAME]);
                    if (_arrangeMap.ContainsKey(parentFullType))
                    {
                        _arrangeMap[parentFullType].Add(info);
                    }
                    else
                    {
                        Array<Dictionary> array = new();
                        array.Add(info);
                        _arrangeMap.Add(parentFullType, array);
                    }
                }
            }
            else
            {
                var n = kvp.Key.Split('/');
                var nodeName = n[n.Length - 1];

                Dictionary info = new();
                info.Add(GODolphinConst.KEY_NODE_NAME, nodeName);
                info.Add(GODolphinConst.KEY_NODE_TYPE, dic[GODolphinConst.TYPE_FULL_NAME]);
                if (_arrangeMap.ContainsKey(_mainTypeName))
                {
                    _arrangeMap[_mainTypeName].Add(info);
                }
                else
                {
                    Array<Dictionary> array = new();
                    array.Add(info);
                    _arrangeMap.Add(_mainTypeName, array);
                }
            }
        }
    }

    /// <summary>
    /// * 利用CustomTypeMap 来修正Map中ValueDic中的一些KEY的值
    /// </summary>
    private void RebuildValue()
    {
        foreach (var typeKvp in CustomTypeMap)
        {
            var nodePath = typeKvp.Value;
            if (Map.TryGetValue(nodePath, out var info))
            {
                var parentInfo = GetParentUITypeInfo(nodePath, out var parentNodePath);
                if (parentInfo != null && parentInfo[GODolphinConst.TYPE_IS_CUSTOM].Equals(GODolphinConst.TRUE_STRING))
                {
                    Dictionary<string, string> dic = new();
                    dic.Add(GODolphinConst.TYPE_FULL_NAME, info[GODolphinConst.TYPE_FULL_NAME]);
                    dic.Add(GODolphinConst.TYPE_PARENT, parentInfo[GODolphinConst.TYPE_FULL_NAME]);
                    dic.Add(GODolphinConst.TYPE_IS_CUSTOM, info[GODolphinConst.TYPE_IS_CUSTOM]);
                    Map[nodePath] = dic;
                }
                //
            }
        }

        GD.Print("Z");
        // TODO: 对Map的Value中的值进行重新赋值
    }

    private Dictionary<string, string> GetParentUITypeInfo(string nodePath, out string parentNodePath)
    {
        if (nodePath.Contains("/"))
        {
            var parentPath = nodePath.Substring(0, nodePath.LastIndexOf('/'));
            if (Map.TryGetValue(parentPath, out var parentInfo))
            {
                parentNodePath = parentPath;
                return parentInfo;
            }
            else
            {
                var info = GetParentUITypeInfo(parentPath, out var val);
                parentNodePath = val;
                return info;
            }
        }

        parentNodePath = "";
        return null;
    }

    private void CreateFolder(string folder)
    {
        var dir = DirAccess.Open("res://");
        if (!dir.DirExists(folder))
        {
            if (dir.MakeDirRecursive(folder) != Error.Ok)
            {
                GD.PrintErr("Failed to create folder: " + folder);
            }
        }

        dir.Dispose();
    }

    private void GenerateFiles()
    {
        GenerateScriptPathAndNamespace(out var scriptPath, out var nameSpace);
        // * 创建设置好的脚本目录
        CreateFolder(scriptPath);
        if (_arrangeMap.Keys.Count > 1)
        {
            // * 需要子目录
            CreateFolder(scriptPath + _mainTypeName + "/");
        }

        foreach (var kvp in _arrangeMap)
        {
            var typeName = kvp.Key;
            var val = kvp.Value;
            if (typeName.Equals(_mainTypeName))
            {
                var mainFile = "res://" + scriptPath + typeName + ".cs";
                var designerFile = "res://" + scriptPath + typeName + ".Designer.cs";
                GenerateFile(typeName, mainFile, designerFile, nameSpace, val);
            }
            else
            {
                var mainFile = "res://" + scriptPath + _mainTypeName + "/" + typeName + ".cs";
                var designerFile = "res://" + scriptPath + _mainTypeName + "/" + typeName + ".Designer.cs";
                GenerateFile(typeName, mainFile, designerFile, nameSpace, val);
            }
        }
    }

    /// <summary>
    /// * 生成文件
    /// </summary>
    private void GenerateFile(string typeName, string scriptFilePath, string designerFilePath, string nameSpace,
        Array<Dictionary> exportPropertyArray)
    {
        // * 生成主文件
        GenerateMainFile(scriptFilePath, nameSpace, typeName);
        // * 生成设计文件
        GenerateDesignerFile(designerFilePath, nameSpace, typeName, exportPropertyArray);
    }

    /// <summary>
    /// * 代码生成 主文件
    /// </summary>
    /// <param name="scriptFilePath"></param>
    /// <param name="nameSpace"></param>
    /// <param name="type"></param>
    private void GenerateMainFile(string scriptFilePath, string nameSpace, string type)
    {
        if (!FileAccess.FileExists(scriptFilePath))
        {
            var file = FileAccess.Open(scriptFilePath, FileAccess.ModeFlags.Write);
            file.StoreString(
                $"using System;\nusing Godot;\n\nnamespace {nameSpace} {{\n\tpublic partial class {type} : Control {{\n\n\t}}\n}}");
            file.Flush();
            file.Close();
            file.Dispose();
        }
    }

    private void GenerateDesignerFile(string designerFilePath, string nameSpace, string type,
        Array<Dictionary> exportPropertyArray)
    {
        var file = FileAccess.Open(designerFilePath, FileAccess.ModeFlags.Write);
        var content =
            $"using System;\nusing Godot;\n\nnamespace {nameSpace} {{\n\tpublic partial class {type} : Control {{\n";
        foreach (var exportProperty in exportPropertyArray)
        {
            var nodeName = (string)exportProperty[GODolphinConst.KEY_NODE_NAME];
            var nodeType = (string)exportProperty[GODolphinConst.KEY_NODE_TYPE];

            content += $"\n\t\t[Export] public {nodeType} {nodeName};\n";
        }

        content += $"\n\t}}\n}}";
        file.StoreString(content);
        file.Flush();
        file.Close();
        file.Dispose();
    }

    private void GenerateScriptPathAndNamespace(out string scriptPath, out string nameSpace)
    {
        var config = new ConfigFile();
        var err = config.Load(GODolphinConst.GODOLPHIN_CONFIG);
        scriptPath = GODolphinConst.UIDefaultScriptPath;
        nameSpace = GODolphinConst.UIDefaultNamespace;
        if (err == Error.Ok)
        {
            scriptPath = (string)config.GetValue(GODolphinConst.UISettingSection, GODolphinConst.UIScriptPathKey,
                GODolphinConst.UIDefaultScriptPath);
            nameSpace = (string)config.GetValue(GODolphinConst.UISettingSection, GODolphinConst.UINamespaceKey,
                GODolphinConst.UIDefaultNamespace);
        }
    }

    private void HandleBind(string scriptPath)
    {
        var root = EditorInterface.Singleton.GetEditedSceneRoot();
        if (root == null)
            return;
        var script = GD.Load<Script>(scriptPath);
        root.SetScript(script);

        foreach (var item in Map)
        {
            var nodePath = item.Key;
            var propertyName = GetPropertyName(nodePath);
            var val = root.GetNodeOrNull(nodePath);
            root.Set(propertyName, val);
        }

        EditorInterface.Singleton.SaveScene();
    }

    private string GetPropertyName(string nodePath)
    {
        string name = "";
        if (nodePath.Contains('/'))
        {
            var arr = nodePath.Split('/');
            name = arr[arr.Length - 1];
        }
        else
        {
            name = nodePath;
        }

        return name;
    }
}