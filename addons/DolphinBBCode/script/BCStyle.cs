namespace GODolphin.BBCode;

public class BCStyle
{
    public int size;
    public string color;
    public bool u;
    public bool i;
    public bool b;

    ///
    /// <param name="size"> 文本大小 </param>
    /// <param name="color"> 文本颜色 </param>
    /// <param name="u"> 是否有下划线 </param>
    /// <param name="i"> 是否斜体 </param>
    /// <param name="b"> 是否加粗 </param>
    public BCStyle(int size, string color, bool u, bool i, bool b)
    {
        this.size = size;
        this.color = color;
        this.u = u;
        this.i = i;
        this.b = b;
    }
}
