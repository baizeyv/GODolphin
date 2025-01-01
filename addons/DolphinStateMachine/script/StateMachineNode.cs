using System;
using GODolphin.Action;
using GODolphin.Log;
using Godot;
using Godot.Collections;

namespace GODolphin.StateMachine;

[Tool]
public partial class StateMachineNode : Control
{
    [Export] public Label StateMachineName;

    [Export] public Tree Tree;

    [Export] public ItemList ItemList;

    [Export] public Label CurrentState;

    [Export] public Texture2D TitleTexture;

    [Export] public Texture2D SelectedTexture;

    [Export] public Texture2D PrevTexture;

    [Export] public Texture2D DisableTexture;

    [Export]
    public Texture2D DownTexture;

    private Dictionary<int, TreeItem> _treeMap;

    private const int IconMaxWidth = 20;

    private int _currentState;

    private bool _isSetState;

    public void Setup<T>(string machineName) where T : Enum
    {
        _treeMap = new();
        StateMachineName.Text = machineName;
        var root = Tree.CreateItem();
        var type = typeof(T);
        root.SetText(0, type.Name);
        root.SetIcon(0, TitleTexture);
        root.SetIconMaxWidth(0, IconMaxWidth);
        var values = Enum.GetValues(type);
        foreach (var item in values)
        {
            var val = (T)item;
            var child = Tree.CreateItem(root);
            child.SetText(0, val.ToString());
            child.SetIcon(0, DisableTexture);
            child.SetIconMaxWidth(0, IconMaxWidth);
            _treeMap.Add(val.GetHashCode(), child);
        }
        ItemList.SetFixedIconSize(Vector2I.One * IconMaxWidth);
    }

    public void SetState<T>(T state) where T : Enum
    {
        CurrentState.Text = state.ToString();
        foreach (var kvp in _treeMap)
        {
            var item = kvp.Value;
            if (kvp.Key == state.GetHashCode())
            {
                item.SetIcon(0, SelectedTexture);
            }
            else if (kvp.Key == _currentState && _isSetState)
            {
                item.SetIcon(0, PrevTexture);
            }
            else
            {
                item.SetIcon(0, DisableTexture);
            }

            item.SetIconMaxWidth(0, IconMaxWidth);
        }
        _isSetState = true;
        _currentState = state.GetHashCode();
        ItemList.AddItem(state.ToString(), DownTexture, false);
    }

    // public override void _Ready()
    // {
    //     Setup<LogType>("TestLogStateMachine");
    //     SetState(LogType.Error);
    //     this.AppendAction(this.Delay(5f, this.Callback(() => { SetState(LogType.Debug); })));
    // }
}