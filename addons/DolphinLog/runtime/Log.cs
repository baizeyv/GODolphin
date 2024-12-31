using System;
using System.Diagnostics;
using GODolphin.Pool;
using Godot;

namespace GODolphin.Log;

public static class Log
{
    public static Logger Debug()
    {
        var val = SafeObjectPool<LoggerType>.Instance.Obtain();
        return val.Debug(new StackTrace(true));
    }

    public static Logger Info()
    {
        var val = SafeObjectPool<LoggerType>.Instance.Obtain();
        return val.Info(new StackTrace(true));
    }

    public static Logger Warn()
    {
        var val = SafeObjectPool<LoggerType>.Instance.Obtain();
        return val.Warn(new StackTrace(true));
    }

    public static Logger Error()
    {
        var val = SafeObjectPool<LoggerType>.Instance.Obtain();
        return val.Error(new StackTrace(true));
    }
}

public class LoggerType : IPoolable
{
    private LogType _type = LogType.Info;

    public Logger Error(StackTrace stackTrace)
    {
        _type = LogType.Error;
        var logger = SafeObjectPool<Logger>.Instance.Obtain();
        logger.Setup(_type, this, stackTrace);
        return logger;
    }

    public Logger Warn(StackTrace stackTrace)
    {
        _type = LogType.Warning;
        var logger = SafeObjectPool<Logger>.Instance.Obtain();
        logger.Setup(_type, this, stackTrace);
        return logger;
    }

    public Logger Debug(StackTrace stackTrace)
    {
        _type = LogType.Debug;
        var logger = SafeObjectPool<Logger>.Instance.Obtain();
        logger.Setup(_type, this, stackTrace);
        return logger;
    }

    public Logger Info(StackTrace stackTrace)
    {
        _type = LogType.Info;
        var logger = SafeObjectPool<Logger>.Instance.Obtain();
        logger.Setup(_type, this, stackTrace);
        return logger;
    }

    public void Reset()
    {
        _type = LogType.Info;
    }
}

public class Logger : IPoolable
{
    private string _prefix = "";

    private string _content = "";

    private string _tag = "";

    private LoggerType _loggerType;

    private LogType _logType;

    private StackTrace _stackTrace;

    public Logger() { }

    internal void Setup(LogType type, LoggerType loggerType, StackTrace stackTrace)
    {
        _logType = type;
        _prefix = GetPrefix(type);
        _loggerType = loggerType;
        _stackTrace = stackTrace;
    }

    public Logger Sep()
    {
        if (!string.IsNullOrEmpty(_content))
        {
            _content += "\n\t";
        }
        _content +=
            "[color=#d6a2e8][s]--------------------------------------------------------[/s][/color]\n\t";
        return this;
    }

    public Logger Cr()
    {
        _content += "\n\t";
        return this;
    }

    public Logger Var(string title, object value)
    {
        title = title.Replace(" ", "_");
        _content += $"[color=#dd7694][b]{title}[/b][/color]=[color=green][i]{value}[/i][/color] ";
        return this;
    }

    public Logger Msg(string content)
    {
        _content += $"[color=#95a5a6][u]{content}[/u][/color] ";
        return this;
    }

    public Logger Tag(string tag)
    {
        _tag = $"[color=#778beb][i][u]{tag}[/u][/i][/color]  ";
        return this;
    }

    /// <summary>
    /// 执行输出
    /// [b]content[/b] 加粗
    /// [i]content[/i] 斜体
    /// [u]content[/u] 下划线
    /// [s]content[/s] 删除线
    /// [color=red]content[/color]
    /// [color=#00ff00]content[/color]
    /// </summary>
    public void Do()
    {
        LogStruct logStruct = null;
        if (string.IsNullOrEmpty(_tag))
        {
            logStruct = new LogStruct()
            {
                LogTime = DateTime.Now,
                LogContent = _prefix + _content,
                LogType = _logType,
                LogStackTrace = _stackTrace,
            };
        }
        else
        {
            logStruct = new LogStruct()
            {
                LogTime = DateTime.Now,
                LogContent = _prefix + _tag + _content,
                LogType = _logType,
                LogStackTrace = _stackTrace,
            };
        }

        var logJson = new LogJson(logStruct);

        LogSharedBuffer.Instance.SendLog(logJson);

        GD.PrintRich($"{logStruct.LogContent}\n{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}\n");

        SafeObjectPool<LoggerType>.Instance.Free(_loggerType);
        SafeObjectPool<Logger>.Instance.Free(this);
    }

    private string GetPrefix(LogType type)
    {
        switch (type)
        {
            case LogType.Error:
                return "[color=#ff4757][b]ERR[/b][/color]  \t";
            case LogType.Debug:
                return "[color=#1e90ff][b]DBG[/b][/color]  \t";
            case LogType.Warning:
                return "[color=#ffa502][b]WRN[/b][/color]  \t";
            case LogType.Info:
                return "[color=#dfe4ea][b]INF[/b][/color]  \t";
        }

        return "INF";
    }

    public void Reset()
    {
        _content = "";
        _prefix = "";
        _tag = "";
        _loggerType = null;
        _logType = LogType.Info;
        _stackTrace = null;
    }
}
