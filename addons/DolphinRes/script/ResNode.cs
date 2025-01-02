using Godot;

namespace GODolphin.Res;

[Tool]
public partial class ResNode : Control
{
    [Export]
    public Label ResNameLabel;

    [Export]
    public Label ResStateLabel;

    [Export]
    public Label RefCountLabel;

    public void Setup(ResStruct data)
    {
        ResNameLabel.Text = data.AssetName;
        ResStateLabel.Text = data.ResState;
        RefCountLabel.Text = data.RefCount.ToString();
    }
}