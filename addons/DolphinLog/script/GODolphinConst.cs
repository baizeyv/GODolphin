namespace GODolphin;

public partial class GODolphinConst
{
    public static readonly string GODOLPHIN_CONFIG = "user://config.gdph";

    public static readonly string CommandClearOnStartGame = "CLEAR_ON_START_GAME";

    /// <summary>
    /// * 在windows中如果无输出则
    /// ! 控制面板 -> 系统和安全 -> Windows Defender 防火墙 -> 左边的高级设置
    /// 在入站规则和出站规则分别进行以下操作
    /// 新建规则 -> Port -> TCP 特定端口 7300 -> -> 允许连接 -> 一直下一步随便起名字
    /// </summary>
    public static readonly ushort LogPort = 7300;

    public static readonly string LogSection = "LOG";

    public static readonly string LogInfoKey = "LOG_INFO";

    public static readonly string LogDebugKey = "LOG_DEBUG";

    public static readonly string LogWarnKey = "LOG_WARN";

    public static readonly string LogErrorKey = "LOG_ERROR";

    public static readonly string LogLockKey = "LOG_LOCK";

    public static readonly string LogClearKey = "LOG_CLEAR";
}