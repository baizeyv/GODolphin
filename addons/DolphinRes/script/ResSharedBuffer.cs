using GODolphin.Pool;
using Godot;
using Godot.Collections;

namespace GODolphin.Res;

public partial class ResSharedBuffer : Node
{
    public static ResSharedBuffer Instance { get; private set; }

    private StreamPeerTcp _client;

    public void SendTable(ResTable resTable)
    {
        _client?.Poll();
        if (_client?.GetStatus() == StreamPeerTcp.Status.Connected)
        {
            var dic = resTable.Dictionarify();
            _client.PutVar(dic);
        }
    }

    public void SendClear()
    {
        _client?.Poll();
        if (_client?.GetStatus() == StreamPeerTcp.Status.Connected)
        {
            var dic = new Dictionary()
            {
                { GODolphinConst.ResCommandClearOnStartGame, GODolphinConst.ResCommandClearOnStartGame }
            };
            _client.PutVar(dic);
        }
    }

    public void SendSearchKeyCount()
    {
        _client?.Poll();
        if (_client?.GetStatus() == StreamPeerTcp.Status.Connected)
        {
            var count = SafeObjectPool<ResSearchKeys>.Instance.CurInUseCount;
            var freeCount = SafeObjectPool<ResSearchKeys>.Instance.FreeCount;
            var dic = new Dictionary()
            {
                { GODolphinConst.ResSearchKeyCommand, count },
                { GODolphinConst.ResSearchKeyCommand + GODolphinConst.ResFreeCountCommand, freeCount }
            };
            _client.PutVar(dic);
        }
    }

    public void SendLoaderCount()
    {
        _client?.Poll();
        if (_client?.GetStatus() == StreamPeerTcp.Status.Connected)
        {
            var count = SafeObjectPool<ResLoader>.Instance.CurInUseCount;
            var freeCount = SafeObjectPool<ResLoader>.Instance.FreeCount;
            var dic = new Dictionary()
            {
                { GODolphinConst.ResLoaderCountCommand, count },
                { GODolphinConst.ResLoaderCountCommand + GODolphinConst.ResFreeCountCommand, freeCount }
            };
            _client.PutVar(dic);
        }
    }

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

    private void _TCPEnterTree()
    {
        _client = new();
        _client.ConnectToHost("127.0.0.1", GODolphinConst.ResPort);
        GD.PrintRich(
            $"[color=green]ResSharedBuffer Connect To 127.0.0.1:{GODolphinConst.ResPort}[/color]");
    }

    private void _TCPExitTree()
    {
        _client?.DisconnectFromHost();
        _client?.Dispose();
    }
}