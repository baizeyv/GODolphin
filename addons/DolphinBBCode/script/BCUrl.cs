namespace GODolphin.BBCode;

public class BCUrl : BCString
{
    public string url;

    public BCUrl(string @string, BCStyle style)
        : base(@string, style)
    {
        this.url = @string;
    }

    public BCUrl(string @string, string url, BCStyle style)
        : base(@string, style)
    {
        this.url = url;
    }

    public override string ToString()
    {
        return "[url=" + url + "]" + @string + "[/url]";
    }

    public override string toHtml()
    {
        return "<a href=\"" + url + "\">" + base.toHtml() + "</a>";
    }

    public override string toOmit()
    {
        return @string;
    }
}
