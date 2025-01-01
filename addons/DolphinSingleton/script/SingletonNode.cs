using System.Diagnostics;
using Godot;

namespace GODolphin.Singleton;

[Tool]
public partial class SingletonNode : Control
{
    [Export]
    public Label TypeNameLabel;

    [Export]
    public Label NodeNameLabel;

    [Export]
    public Label NameSpaceLabel;

    public void Setup(string typeName, string nodeName, string namespaceName)
    {
        TypeNameLabel.Text = typeName;
        NodeNameLabel.Text = nodeName;
        NameSpaceLabel.Text = namespaceName;
    }
}