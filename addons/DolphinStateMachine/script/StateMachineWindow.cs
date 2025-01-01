using System;
using System.Collections.Generic;
using GODolphin.Singleton;
using Godot;

namespace GODolphin.StateMachine;

[Tool]
public partial class StateMachineWindow : Control
{
    [Export] public BoxContainer StateMachineContainer;

    private TcpServer _server = new();

    private List<StreamPeerTcp> _list = new();

    private Godot.Collections.Dictionary<string, StateMachineNode> _map = new();

    private void OnNewCommand(StateMachineJson json)
    {
        if (json == null)
            return;
        if (json.NeedRemove)
        {
            // * 需要移除
            if (_map.TryGetValue(json.StateMachineName, out var node))
            {
                node.QueueFree();
            }
        }
        else
        {
            if (string.IsNullOrEmpty(json.State))
            {
                // * 状态机初始化
                PackedScene nodePrefab =
                    GD.Load<PackedScene>("res://addons/DolphinStateMachine/prefab/StateMachineNode.tscn");
                var node = nodePrefab?.Instantiate() as StateMachineNode;
                var enumType = Type.GetType(json.EnumTypeFullName);

                var methodInfo = typeof(StateMachineNode).GetMethod(nameof(node.Setup));
                if (methodInfo != null)
                {
                    var genericMethod = methodInfo.MakeGenericMethod(enumType);
                    genericMethod.Invoke(node, new[] { json.StateMachineName });
                }

                _map.Add(json.StateMachineName, node);
                StateMachineContainer.AddChild(node);
            }
            else
            {
                // * 已经存在的状态机
                var node = _map[json.StateMachineName];
                var enumType = Type.GetType(json.EnumTypeFullName);

                var methodInfo = typeof(StateMachineNode).GetMethod(nameof(node.SetState));
                if (methodInfo != null)
                {
                    var genericMethod = methodInfo.MakeGenericMethod(enumType);
                    if (Enum.TryParse(enumType, json.State, out var result))
                    {
                        genericMethod.Invoke(node, new[] { result });
                    }
                }
            }
        }
    }

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
        var err = _server.Listen(GODolphinConst.StateMachinePort, "127.0.0.1");
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
                var json = StateMachineJson.ParseDictionary(obj.AsGodotDictionary());
                if (!string.IsNullOrEmpty(json.Command))
                {
                    if (json.Command.Equals(GODolphinConst.SMCommandClearOnStartGame))
                    {
                        // * 清空
                        foreach (var node in _map)
                        {
                            node.Value.QueueFree();
                        }
                        _map.Clear();
                    }
                }
                else
                {
                    OnNewCommand(json);
                }
            }
        }
    }
}