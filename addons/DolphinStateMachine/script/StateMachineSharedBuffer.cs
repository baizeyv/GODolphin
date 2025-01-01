using System;
using Godot;

namespace GODolphin.StateMachine;

public partial class StateMachineSharedBuffer : Node
{
    public static StateMachineSharedBuffer Instance { get; private set; }

    private StreamPeerTcp _client;

    public override void _EnterTree()
    {
        Instance = this;
        _TCPEnterTree();
        SendClear();
    }

    public override void _ExitTree()
    {
        _TCPExitTree();
    }

    public void SendClear()
    {
        _client?.Poll();
        if (_client?.GetStatus() == StreamPeerTcp.Status.Connected)
        {
            StateMachineJson json = new()
            {
                Command = GODolphinConst.SMCommandClearOnStartGame
            };
            _client.PutVar(json.Dictionarify());
        }
    }

    public void SendStateMachine<T>(StateMachine<T> stateMachine, bool isInit, bool needRemove) where T : Enum
    {
        _client?.Poll();
        if (_client?.GetStatus() == StreamPeerTcp.Status.Connected)
        {
            StateMachineJson json = new()
            {
                EnumTypeFullName = typeof(T).FullName,
                StateMachineName = stateMachine.MachineName,
                State = isInit ? "" : stateMachine.CurrentStateId.ToString(),
                NeedRemove = needRemove
            };
            _client.PutVar(json.Dictionarify());
        }
    }

    private void _TCPEnterTree()
    {
        _client = new();
        _client.ConnectToHost("127.0.0.1", GODolphinConst.StateMachinePort);
        GD.PrintRich(
            $"[color=green]StateMachineSharedBuffer Connect To 127.0.0.1:{GODolphinConst.StateMachinePort}[/color]");
    }

    private void _TCPExitTree()
    {
        _client?.DisconnectFromHost();
        _client?.Dispose();
    }
}