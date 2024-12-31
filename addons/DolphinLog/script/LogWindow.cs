using System.Collections.Generic;
using Godot;
using Godot.Collections;

namespace GODolphin.Log;

[Tool]
public partial class LogWindow : Control
{
    [Export]
    public Button ClearButton;

    [Export]
    public Button CollapseButton;

    [Export]
    public BoxContainer LogContainer;

    [Export]
    public CheckButton LockScrollCheckButton;

    [Export]
    public CheckButton ClearOnStartButton;

    [Export]
    public CheckBox InfoCheckBox;

    [Export]
    public CheckBox DebugCheckBox;

    [Export]
    public CheckBox WarnCheckBox;

    [Export]
    public CheckBox ErrorCheckBox;

    [Export]
    public ScrollContainer ScrollContainer;

    [Export]
    public LineEdit SearchText;

    private ConfigFile _config = new();

    public Dictionary _logMap = new()
    {
        { LogType.Info.GetHashCode(), new Array<LogNode>() },
        { LogType.Debug.GetHashCode(), new Array<LogNode>() },
        { LogType.Warning.GetHashCode(), new Array<LogNode>() },
        { LogType.Error.GetHashCode(), new Array<LogNode>() },
    };

    public override void _EnterTree()
    {
        InfoCheckBox.ButtonPressed = true;
        DebugCheckBox.ButtonPressed = true;
        WarnCheckBox.ButtonPressed = true;
        ErrorCheckBox.ButtonPressed = true;
        LockScrollCheckButton.ButtonPressed = true;
        ClearOnStartButton.ButtonPressed = true;
        SearchText.Text = "";

        InfoCheckBox.Connect("toggled", new Callable(this, nameof(InfoCheckBoxToggle)));
        DebugCheckBox.Connect("toggled", new Callable(this, nameof(DebugCheckBoxToggle)));
        WarnCheckBox.Connect("toggled", new Callable(this, nameof(WarnCheckBoxToggle)));
        ErrorCheckBox.Connect("toggled", new Callable(this, nameof(ErrorCheckBoxToggle)));
        ClearButton.Connect("pressed", new Callable(this, nameof(OnClearPressed)));
        CollapseButton.Connect("pressed", new Callable(this, nameof(OnCollapsePressed)));
        SearchText.Connect("text_changed", new Callable(this, nameof(OnTextChanged)));
        _TCPReady();
    }

    private void OnNewLog(LogJson logJson)
    {
        if (logJson == null)
            return;
        PackedScene logNodePrefab = GD.Load("res://addons/AnimaLog/LogNode.tscn") as PackedScene;
        var logNode = logNodePrefab?.Instantiate() as LogNode;
        logNode?.Setup(logJson);
        LogContainer.AddChild(logNode);
        var typeId = logJson.LogType;
        LogType type = LogType.Info;
        switch (typeId)
        {
            case "Info":
                type = LogType.Info;
                break;
            case "Warning":
                type = LogType.Warning;
                break;
            case "Error":
                type = LogType.Error;
                break;
            case "Debug":
                type = LogType.Debug;
                break;
        }

        _logMap[type.GetHashCode()].AsGodotArray().Add(logNode);

        switch (type)
        {
            case LogType.Info:
                logNode.Visible = InfoCheckBox.ButtonPressed;
                break;
            case LogType.Debug:
                logNode.Visible = DebugCheckBox.ButtonPressed;
                break;
            case LogType.Warning:
                logNode.Visible = WarnCheckBox.ButtonPressed;
                break;
            case LogType.Error:
                logNode.Visible = ErrorCheckBox.ButtonPressed;
                break;
        }
        logNode?.SetMatch(SearchText.Text);
    }

    // private void OnNewLog(LogJsonGroup jsonGroup)
    // {
    //     if (jsonGroup == null || jsonGroup.Logs == null)
    //         return;
    //     PackedScene logNodePrefab = GD.Load("res://addons/AnimaLog/LogNode.tscn") as PackedScene;
    //     if (_currentLogIndex + 1 < jsonGroup.Logs.Length)
    //     {
    //         for (int i = _currentLogIndex + 1; i < jsonGroup.Logs.Length; i++)
    //         {
    //             _currentLogIndex++;
    //             var logNode = logNodePrefab?.Instantiate() as LogNode;
    //             var logJson = jsonGroup.Logs[i];
    //             logNode?.Setup(logJson);
    //             LogContainer.AddChild(logNode);
    //             var typeId = logJson.LogType;
    //             LogType type = LogType.Info;
    //             switch (typeId)
    //             {
    //                 case "Info":
    //                     type = LogType.Info;
    //                     break;
    //                 case "Warning":
    //                     type = LogType.Warning;
    //                     break;
    //                 case "Error":
    //                     type = LogType.Error;
    //                     break;
    //                 case "Debug":
    //                     type = LogType.Debug;
    //                     break;
    //             }
    //
    //             _logMap[type.GetHashCode()].AsGodotArray().Add(logNode);
    //
    //             switch (type)
    //             {
    //                 case LogType.Info:
    //                     logNode.Visible = InfoCheckBox.ButtonPressed;
    //                     break;
    //                 case LogType.Debug:
    //                     logNode.Visible = DebugCheckBox.ButtonPressed;
    //                     break;
    //                 case LogType.Warning:
    //                     logNode.Visible = WarnCheckBox.ButtonPressed;
    //                     break;
    //                 case LogType.Error:
    //                     logNode.Visible = ErrorCheckBox.ButtonPressed;
    //                     break;
    //             }
    //         }
    //     }
    // }

    private void OnClearPressed()
    {
        foreach (var kvp in _logMap)
        {
            foreach (var node in kvp.Value.AsGodotArray())
            {
                ((Node)node).QueueFree();
            }

            kvp.Value.AsGodotArray().Clear();
        }
    }

    private void OnCollapsePressed()
    {
        foreach (var kvp in _logMap)
        {
            foreach (var node in kvp.Value.AsGodotArray())
            {
                ((LogNode)node).Collapse();
            }
        }
    }

    private void InfoCheckBoxToggle(bool toggleOn)
    {
        foreach (var node in _logMap[LogType.Info.GetHashCode()].AsGodotArray())
        {
            ((LogNode)node).Visible = toggleOn;
        }
        if (toggleOn)
        {
            var arr = _logMap[LogType.Info.GetHashCode()].AsGodotArray();
            foreach (var item in arr)
            {
                ((LogNode)item).SetMatch(SearchText.Text);
            }
        }
    }

    private void DebugCheckBoxToggle(bool toggleOn)
    {
        foreach (var node in _logMap[LogType.Debug.GetHashCode()].AsGodotArray())
        {
            ((LogNode)node).Visible = toggleOn;
        }
        if (toggleOn)
        {
            var arr = _logMap[LogType.Debug.GetHashCode()].AsGodotArray();
            foreach (var item in arr)
            {
                ((LogNode)item).SetMatch(SearchText.Text);
            }
        }
    }

    private void WarnCheckBoxToggle(bool toggleOn)
    {
        foreach (var node in _logMap[LogType.Warning.GetHashCode()].AsGodotArray())
        {
            ((LogNode)node).Visible = toggleOn;
        }
        if (toggleOn)
        {
            var arr = _logMap[LogType.Warning.GetHashCode()].AsGodotArray();
            foreach (var item in arr)
            {
                ((LogNode)item).SetMatch(SearchText.Text);
            }
        }
    }

    private void ErrorCheckBoxToggle(bool toggleOn)
    {
        foreach (var node in _logMap[LogType.Error.GetHashCode()].AsGodotArray())
        {
            ((LogNode)node).Visible = toggleOn;
        }

        if (toggleOn)
        {
            var arr = _logMap[LogType.Error.GetHashCode()].AsGodotArray();
            foreach (var item in arr)
            {
                ((LogNode)item).SetMatch(SearchText.Text);
            }
        }
    }

    private void OnTextChanged(string text)
    {
        if (InfoCheckBox.ButtonPressed)
        {
            var arr = _logMap[LogType.Info.GetHashCode()].AsGodotArray();
            foreach (var item in arr)
            {
                ((LogNode)item).SetMatch(text);
            }
        }
        if (DebugCheckBox.ButtonPressed)
        {
            var arr = _logMap[LogType.Debug.GetHashCode()].AsGodotArray();
            foreach (var item in arr)
            {
                ((LogNode)item).SetMatch(text);
            }
        }
        if (WarnCheckBox.ButtonPressed)
        {
            var arr = _logMap[LogType.Warning.GetHashCode()].AsGodotArray();
            foreach (var item in arr)
            {
                ((LogNode)item).SetMatch(text);
            }
        }
        if (ErrorCheckBox.ButtonPressed)
        {
            var arr = _logMap[LogType.Error.GetHashCode()].AsGodotArray();
            foreach (var item in arr)
            {
                ((LogNode)item).SetMatch(text);
            }
        }
    }

    public override void _Process(double delta)
    {
        _TCPProcess();
        ///////////////////////////////////////

        // var f = LogConstant.LogFile;
        // var err = _config.Load(f);
        // if (err == Error.Ok)
        // {
        //     var sections = _config.GetSections();
        //     foreach (var section in sections)
        //     {
        //         if (_addedArray.Contains(section))
        //             continue;
        //         var logContent = (string)_config.GetValue(section, LogConstant.KeyContent);
        //         var logTime = (string)_config.GetValue(section, LogConstant.KeyTime);
        //         var logStackTrace = (string)_config.GetValue(section, LogConstant.KeyStackTrace);
        //         var logType = (string)_config.GetValue(section, LogConstant.KeyType);
        //         var logJson = new LogJson();
        //         logJson.LogContent = logContent;
        //         logJson.LogTime = logTime;
        //         logJson.LogStackTrace = logStackTrace;
        //         logJson.LogType = logType;
        //         OnNewLog(logJson);
        //
        //         _addedArray.Add(section);
        //     }
        // }

        if (LockScrollCheckButton.ButtonPressed)
        {
            ScrollContainer.ScrollVertical = int.MaxValue;
        }
    }

    public override void _ExitTree()
    {
        InfoCheckBox.Disconnect("toggled", new Callable(this, nameof(InfoCheckBoxToggle)));
        DebugCheckBox.Disconnect("toggled", new Callable(this, nameof(DebugCheckBoxToggle)));
        WarnCheckBox.Disconnect("toggled", new Callable(this, nameof(WarnCheckBoxToggle)));
        ErrorCheckBox.Disconnect("toggled", new Callable(this, nameof(ErrorCheckBoxToggle)));
        ClearButton.Disconnect("pressed", new Callable(this, nameof(OnClearPressed)));
        CollapseButton.Disconnect("pressed", new Callable(this, nameof(OnCollapsePressed)));
        SearchText.Disconnect("text_changed", new Callable(this, nameof(OnTextChanged)));
        OnClearPressed();

        // GodotPipe.Dispose();

        _server.Stop();
        _server?.Dispose();
        _list.Clear();
    }

    #region TCP

    private TcpServer _server = new();

    private List<StreamPeerTcp> _list = new();

    private void _TCPReady()
    {
        var err = _server.Listen(GODolphinConst.LogPort, "127.0.0.1");
    }

    private void _TCPProcess()
    {
        if (_server.IsConnectionAvailable())
        {
            // GD.Print("Connection Available");
            var conn = _server.TakeConnection();
            _list.Add(conn);
        }

        foreach (var item in _list)
        {
            while (item.GetAvailableBytes() > 0)
            {
                var obj = item.GetVar();
                var logJson = LogJson.ParseDictionary(obj.AsGodotDictionary());
                if (!string.IsNullOrEmpty(logJson.LogCommand))
                {
                    if (
                        logJson.LogCommand.Equals(GODolphinConst.CommandClearOnStartGame)
                        && ClearOnStartButton.ButtonPressed
                    )
                    {
                        OnClearPressed();
                    }
                }
                else
                {
                    OnNewLog(logJson);
                }
                // GD.Print("EDITOR RECEIVE :" + logJson.LogContent);
            }
        }
    }
    #endregion
}