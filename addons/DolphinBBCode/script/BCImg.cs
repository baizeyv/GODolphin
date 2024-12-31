namespace GODolphin.BBCode;

public class BCImg : BCItem
{
    protected internal string src;

    public BCImg(string src)
    {
        this.src = src;
    }

    public override string ToString()
    {
        return "[img=" + src + "]";
    }

    public override string toHtml()
    {
        return "<img style=\"max-width:100%;height:auto\" src=\"" + src + "\"/>";
    }

    public override string toOmit()
    {
        return "[图片]";
    }
}
