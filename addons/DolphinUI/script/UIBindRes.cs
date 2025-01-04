using Godot;
using Godot.Collections;

[Tool]
public partial class UIBindRes : Resource
{
    /// <summary>
    /// * Key: nodePath ||| Value: typeString
    /// </summary>
    [Export]
    public Dictionary<string, string> Map;
}
