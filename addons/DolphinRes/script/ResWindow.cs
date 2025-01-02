using System.Collections.Generic;
using System.Linq;
using Godot;
using Godot.Collections;

namespace GODolphin.Res;

[Tool]
public partial class ResWindow : Control
{
    [Export] public BoxContainer NodeContainer;

    [Export] public Label SearchKeyCountLabel;

    [Export] public Label ResLoaderCountLabel;

    [Export] public Label SearchKeyFreeCountLabel;

    [Export] public Label ResLoaderFreeCountLabel;

    private TcpServer _server = new();

    private List<StreamPeerTcp> _list = new();

    private Godot.Collections.Dictionary<string, ResNode> _map = new();

    public override void _EnterTree()
    {
        _TCPReady();
    }

    public override void _Process(double delta)
    {
        _TCPProcess();
    }

    public override void _ExitTree()
    {
        _server.Stop();
        _server?.Dispose();
        _list.Clear();
        _map.Clear();
    }

    private void _TCPReady()
    {
        var err = _server.Listen(GODolphinConst.ResPort, "127.0.0.1");
    }

    private void _TCPProcess()
    {
        if (_server.IsConnectionAvailable())
        {
            var conn = _server.TakeConnection();
            _list.Add(conn);
        }

        foreach (var item in _list)
        {
            while (item.GetAvailableBytes() > 0)
            {
                var obj = item.GetVar();
                var dic = obj.AsGodotDictionary();
                if (dic.ContainsKey(GODolphinConst.ResCommandClearOnStartGame))
                {
                    // * 启动时清空
                    foreach (var kvp in _map)
                    {
                        kvp.Value.QueueFree();
                    }

                    _map.Clear();
                    SearchKeyCountLabel.Text = "0";
                    ResLoaderCountLabel.Text = "0";
                    SearchKeyFreeCountLabel.Text = "0";
                    ResLoaderFreeCountLabel.Text = "0";
                }
                else if (dic.ContainsKey(GODolphinConst.ResSearchKeyCommand))
                {
                    var count = (int)dic[GODolphinConst.ResSearchKeyCommand];
                    SearchKeyCountLabel.Text = $"{count}";
                    var freeCount = (int)dic[GODolphinConst.ResSearchKeyCommand + GODolphinConst.ResFreeCountCommand];
                    SearchKeyFreeCountLabel.Text = $"{freeCount}";
                }
                else if (dic.ContainsKey(GODolphinConst.ResLoaderCountCommand))
                {
                    var count = (int)dic[GODolphinConst.ResLoaderCountCommand];
                    ResLoaderCountLabel.Text = $"{count}";
                    var freeCount = (int)dic[GODolphinConst.ResLoaderCountCommand + GODolphinConst.ResFreeCountCommand];
                    ResLoaderFreeCountLabel.Text = $"{freeCount}";
                }
                else
                {
                    foreach (var kvp in dic)
                    {
                        var resStruct = ResStruct.ParseString((string)kvp.Value);
                        var assetName = (string)kvp.Key;
                        if (_map.TryGetValue(assetName, out var node))
                        {
                            node.Setup(resStruct);
                        }
                        else
                        {
                            PackedScene nodePrefab =
                                GD.Load<PackedScene>("res://addons/DolphinRes/prefab/ResNode.tscn");
                            node = nodePrefab.Instantiate<ResNode>();
                            NodeContainer.AddChild(node);
                            node.Setup(resStruct);
                            _map.Add(assetName, node);
                        }
                    }

                    // * 需要移除的
                    var noList = _map.Where(item => !dic.ContainsKey(item.Key));
                    foreach (var kvp in noList)
                    {
                        var key = kvp.Key;
                        kvp.Value.QueueFree();
                        _map.Remove(key);
                    }
                }
            }
        }
    }
}