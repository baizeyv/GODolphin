using System;
using GODolphin;
using Godot;
using Godot.Collections;

[Tool]
public partial class UIBindRes : Resource
{
    /// <summary>
    /// * Key: nodePath ||| Value: typeString
    /// </summary>
    [Export] public Dictionary<string, string> Map;

    public void Generate(string scenePath)
    {
        var arr = scenePath.Split('/');
        var nameWithSuffix = arr[arr.Length - 1];
        var name = nameWithSuffix.Substring(0, nameWithSuffix.Length - 5);
        var config = new ConfigFile();
        var err = config.Load(GODolphinConst.GODOLPHIN_CONFIG);
        string scriptPath = GODolphinConst.UIDefaultScriptPath, namespacestr = GODolphinConst.UIDefaultNamespace;
        if (err == Error.Ok)
        {
            scriptPath = (string)config.GetValue(GODolphinConst.UISettingSection, GODolphinConst.UIScriptPathKey,
                GODolphinConst.UIDefaultScriptPath);
            namespacestr = (string)config.GetValue(GODolphinConst.UISettingSection, GODolphinConst.UINamespaceKey,
                GODolphinConst.UIDefaultNamespace);
        }

        CreateFolder(scriptPath);
        var scriptFile = "res://" + scriptPath + name + ".cs";
        var scriptDesignerFile = "res://" + scriptPath + name + ".Designer.cs";
        CreateFile(scriptFile, namespacestr, name);
        CreateDesignerFile(scriptDesignerFile, namespacestr, name);

        HandleBind(scriptFile);
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

    private void CreateFile(string scriptPath, string nameSpace, string className)
    {
        if (!FileAccess.FileExists(scriptPath))
        {
            var file = FileAccess.Open(scriptPath, FileAccess.ModeFlags.Write);
            file.StoreString(
                $"using System;\nusing Godot;\n\nnamespace {nameSpace} {{\n\tpublic partial class {className} : Control {{\n\n\t}}\n}}");
            file.Flush();
            file.Close();
            file.Dispose();
        }
    }

    private void CreateDesignerFile(string designerPath, string nameSpace, string className)
    {
        var file = FileAccess.Open(designerPath, FileAccess.ModeFlags.Write);
        // TODO:
        var content =
            $"using System;\nusing Godot;\n\nnamespace {nameSpace} {{\n\tpublic partial class {className} : Control {{\n";
        foreach (var item in Map)
        {
            var typeString = item.Value;
            var nodePath = item.Key;
            var name = GetPropertyName(nodePath);
            content += $"\n\t\t[Export] public {typeString} {name};\n";
        }

        content += $"\n\t}}\n}}";
        file.StoreString(content);
        file.Flush();
        file.Close();
        file.Dispose();
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
            GD.Print(nodePath + " ?? " + val + "  ?? " + val?.Name);
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