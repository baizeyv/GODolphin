using Godot;

namespace GODolphin.Log;

public partial class LogSharedBuffer : Node
{
    public static LogSharedBuffer Instance { get; private set; }

    public override void _EnterTree()
    {
        Instance = this;
        _TCPEnterTree();
        SendCommand(GODolphinConst.CommandClearOnStartGame); // clear on start game
    }

    public void SendLog(LogJson logJson)
    {
        _TCPSendLog(logJson);
    }

    public void SendCommand(string msg)
    {
        _TCPSend(msg);
    }

    public override void _ExitTree()
    {
        _TCPExitTree();
    }

    #region TCP

    private StreamPeerTcp _client;

    private void _TCPEnterTree()
    {
        _client = new();
        _client.ConnectToHost("127.0.0.1", GODolphinConst.LogPort);
        GD.PrintRich($"[color=green]Connect To 127.0.0.1:{GODolphinConst.LogPort}[/color]");
    }

    private void _TCPExitTree()
    {
        _client?.DisconnectFromHost();
        _client?.Dispose();
    }

    private void _TCPSend(string msg)
    {
        _client?.Poll();
        // GD.Print("CLIENT STATUS A:" + _client.GetStatus());
        if (_client?.GetStatus() == StreamPeerTcp.Status.Connected)
        {
            LogStruct logStruct = new() { LogCommand = msg };
            var logJson = new LogJson(logStruct);
            _client.PutVar(logJson.Dictionarify());
            // var err = _client.PutData(msg.ToUtf8Buffer());
        }
        // GD.Print("CLIENT STATUS B:" + _client.GetStatus());
    }

    private void _TCPSendLog(LogJson logJson)
    {
        _client?.Poll();
        // GD.Print("CLIENT STATUS A:" + _client.GetStatus());
        if (_client?.GetStatus() == StreamPeerTcp.Status.Connected)
        {
            _client.PutVar(logJson.Dictionarify());
            // var err = _client.PutData(msg.ToUtf8Buffer());
        }
        // GD.Print("CLIENT STATUS B:" + _client.GetStatus());
    }

    #endregion

}