using System;
using System.Diagnostics;
using Godot;
using Godot.Collections;

namespace GODolphin.Log;

public class LogStruct
{
    /// <summary>
    /// * 日志内容
    /// </summary>
    public string LogContent;

    /// <summary>
    /// * 日志时间
    /// </summary>
    public DateTime LogTime;

    // now.ToString("yyyy-MM-dd HH:mm:ss");

    /// <summary>
    /// * 日志类型
    /// </summary>
    public LogType LogType;

    /// <summary>
    /// * 日志追踪栈
    /// </summary>
    public StackTrace LogStackTrace;

    /// <summary>
    /// * 日志命令
    /// </summary>
    public string LogCommand;
}

public class LogJson
{

    /// <summary>
    /// * 日志内容
    /// </summary>
    public string LogContent;

    /// <summary>
    /// * 日志时间
    /// </summary>
    public string LogTime;

    // now.ToString("yyyy-MM-dd HH:mm:ss");

    /// <summary>
    /// * 日志类型
    /// </summary>
    public string LogType;

    /// <summary>
    /// * 日志追踪栈
    /// </summary>
    public string LogStackTrace;

    /// <summary>
    /// * 日志命令
    /// </summary>
    public string LogCommand;

    public LogJson() { }

    public LogJson(LogStruct logStruct)
    {
        LogTime = logStruct.LogTime.ToString("yyyy-MM-dd\nHH:mm:ss.fff");
        if (logStruct.LogStackTrace == null)
        {
            LogStackTrace = "No stack trace.";
        }
        else
        {
            var trace = logStruct.LogStackTrace;
            var stackString = "";
            if (trace != null)
            {
                bool isFirst = true;
                var frames = trace.GetFrames();
                foreach (var frame in frames)
                {
                    if (isFirst)
                    {
                        isFirst = false;
                        continue;
                    }
                    var method = frame.GetMethod();
                    if (method == null)
                        continue;
                    string className = method.DeclaringType.FullName;
                    string file = frame.GetFileName() ?? "Unknown File";
                    int line = frame.GetFileLineNumber();
                    int column = frame.GetFileColumnNumber();
                    stackString +=
                        $" - {className}.{method.Name} in [color=#6aacf3][u][url={file}]{file}[/url][/u][/color] at line:[color=#6aacf3]{line}[/color], column:[color=#ffa502]{column}[/color] \n";
                }
            }
            LogStackTrace = stackString;
        }
        LogType = logStruct.LogType.ToString();
        LogContent = logStruct.LogContent;
        LogCommand = logStruct.LogCommand;
    }

    public Dictionary Dictionarify()
    {
        Dictionary dic = new()
        {
            { "LogContent", LogContent },
            { "LogTime", LogTime },
            { "LogStackTrace", LogStackTrace },
            { "LogType", LogType },
            { "LogCommand", LogCommand },
        };
        return dic;
    }

    public static LogJson ParseDictionary(Dictionary dic)
    {
        return new LogJson()
        {
            LogContent = (string)dic["LogContent"],
            LogTime = (string)dic["LogTime"],
            LogStackTrace = (string)dic["LogStackTrace"],
            LogType = (string)dic["LogType"],
            LogCommand = (string)dic["LogCommand"],
        };
    }

    public string Stringify()
    {
        return Json.Stringify(Dictionarify());
    }

    public static LogJson ParseString(string jsonContent)
    {
        var result = Json.ParseString(jsonContent);
        var dic = result.AsGodotDictionary();
        LogJson logJson = new();
        logJson.LogContent = (string)dic["LogContent"];
        logJson.LogTime = (string)dic["LogTime"];
        logJson.LogType = (string)dic["LogType"];
        logJson.LogStackTrace = (string)dic["LogStackTrace"];
        logJson.LogCommand = (string)dic["LogCommand"];
        return logJson;
    }
}