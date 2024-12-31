namespace GODolphin.BBCode;

public abstract class BCItem
{
    public abstract override string ToString();

    /// <summary>
    /// 转换为html以便在WebView中显示 </summary>
    /// <returns> String 对应的html </returns>
    public abstract string toHtml();

    /// <summary>
    /// 获取其简略信息以便在TextView中显示 </summary>
    /// <returns> String 简略信息 </returns>
    public abstract string toOmit();
}
