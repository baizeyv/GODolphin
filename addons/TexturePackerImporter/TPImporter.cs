using Godot;
using Godot.Collections;

namespace TexturePackerImporter;

[Tool]
public partial class TPImporter : EditorImportPlugin
{
    public override string _GetImporterName()
    {
        return "TPImporter";
    }

    public override string _GetVisibleName()
    {
        return "SpriteSheet from TexturePacker";
    }

    public override string[] _GetRecognizedExtensions()
    {
        return new []{"tpsheet"};
    }

    public override string _GetSaveExtension()
    {
        return "res";
    }

    public override string _GetResourceType()
    {
        return "Resource";
    }

    public override int _GetPresetCount()
    {
        return 1;
    }

    public override string _GetPresetName(int presetIndex)
    {
        return "Default";
    }

    public override Array<Dictionary> _GetImportOptions(string path, int presetIndex)
    {
        return new Array<Dictionary>();
    }

    public override bool _GetOptionVisibility(string path, StringName optionName, Dictionary options)
    {
        return true;
    }

    public override int _GetImportOrder()
    {
        return 200;
    }

    public override float _GetPriority()
    {
        return 1.0f;
    }

    public override Error _Import(string sourceFile, string savePath, Dictionary options, Array<string> platformVariants, Array<string> genFiles)
    {
        var sheets = ReadSpriteSheet(sourceFile);
        var sheetFolder = sourceFile.GetBaseName() + ".sprites";
        CreateFolder(sheetFolder);

        var array = sheets["textures"].AsGodotArray();

        foreach (var sheet in array)
        {
            var sheetDic = sheet.AsGodotDictionary();
            var imageName = (string)sheetDic["image"];

            var sheetFile = sourceFile.GetBaseDir() + "/" + imageName;
            var image = ResourceLoader.Load<Texture2D>(sheetFile, "ImageTexture");
            if (image == null)
            {
                GD.PrintErr("Failed to load image texture: " + sheetFile);
                return Error.FileNotFound;
            }

            CreateAtlasTextures(sheetFolder, sheetDic, image, genFiles);
        }

        return ResourceSaver.Save(new Resource(), $"{savePath}.{_GetSaveExtension()}");
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

    private bool CreateAtlasTextures(string sheetFolder, Dictionary sheet, Texture2D image, Array<string> genFiles)
    {
        var array = sheet["sprites"].AsGodotArray();
        foreach (var sprite in array)
        {
            var dic = sprite.AsGodotDictionary();
            if (!CreateAtlasTexture(sheetFolder, dic, image, genFiles))
            {
                return false;
            }
        }

        return true;
    }

    private bool CreateAtlasTexture(string sheetFolder, Dictionary sprite, Texture2D image, Array<string> genFiles)
    {
        var name = sheetFolder + "/" + ((string)sprite["filename"]).GetBaseName() + ".tres";
        AtlasTexture texture;
        if (ResourceLoader.Exists(name, "AtlasTexture"))
        {
            texture = ResourceLoader.Load<AtlasTexture>(name, "AtlasTexture");
        }
        else
        {
            texture = new AtlasTexture();
        }

        texture.Atlas = image;
        var region = sprite["region"].AsGodotDictionary();
        texture.Region = new Rect2((float)region["x"], (float)region["y"], (float)region["w"], (float)region["h"]);
        var margin = sprite["margin"].AsGodotDictionary();
        texture.Margin = new Rect2((float)margin["x"], (float)margin["y"], (float)margin["w"], (float)margin["h"]);
        genFiles.Add(name);
        return SaveResource(name, texture);
    }

    private bool SaveResource(string name, AtlasTexture texture)
    {
        CreateFolder(name.GetBaseDir());
        var status = ResourceSaver.Save(texture, name);
        if (status != Error.Ok)
        {
            GD.PrintErr("Failed to save texture: " + name);
            return false;
        }
        return true;
    }

    private Dictionary ReadSpriteSheet(string filename)
    {
        var file = FileAccess.Open(filename, FileAccess.ModeFlags.Read);
        if (file == null)
        {
            GD.PrintErr("Failed to open texture sheet: " + filename);
            return default;
        }

        var text = file.GetAsText();
        var dict = Json.ParseString(text);
        file.Close();
        file.Dispose();
        return dict.AsGodotDictionary();
    }
}