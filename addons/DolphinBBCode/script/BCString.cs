namespace GODolphin.BBCode;

public class BCString : BCItem
{
    protected internal string @string;
    protected internal BCStyle style;

    public BCString(string @string, BCStyle style)
    {
        this.@string = @string;
        this.style = style;
    }

    public override string ToString()
    {
        return @string;
    }

    public override string toHtml()
    {
        string html = @string;
        if (style != null)
        {
            html = style.u ? "<u>" + html + "</u>" : html;
            html = style.i ? "<i>" + html + "</i>" : html;
            html = style.b ? "<b>" + html + "</b>" : html;
            html = !string.ReferenceEquals(style.color, null)
                ? "<span style=\"color: " + style.color + ";\">" + html + "</span>"
                : html;
            html = style.size > 0 ? "<font size=\"" + style.size + "\">" + html + "</font>" : html;
        }
        return html.Replace("\\n", "<br/>");
    }

    public override string toOmit()
    {
        return ToString().Replace('\n', ' ');
    }
}
