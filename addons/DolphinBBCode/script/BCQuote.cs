namespace GODolphin.BBCode;

public class BCQuote : BCItem
{
    protected internal string quoter;
    protected internal BCItem bCItem;
    protected internal bool pre;

    public BCQuote(string quoter, BCItem bCItem)
    {
        this.quoter = quoter;
        this.bCItem = bCItem;
        this.pre = false;
    }

    public BCQuote(BCItem bCItem, bool pre)
    {
        this.quoter = null;
        this.bCItem = bCItem;
        this.pre = true;
    }

    public override string ToString()
    {
        return pre ? "[pre]" + bCItem + "[/pre]"
            : string.ReferenceEquals(quoter, null) ? "[quote]" + bCItem + "[/quote]"
            : "[quote=" + quoter + "]" + bCItem + "[/quote]";
    }

    public override string toHtml()
    {
        return pre
            ? "<pre>" + bCItem.toHtml() + "</pre>"
            : "<div style=\"margin:10;padding:10;padding-right:0;margin-right:10;border-left-style:solid;border-color:#C8C8C8\">"
                + (
                    (!string.ReferenceEquals(quoter, null))
                        ? "<span style=\"color:blue\">@" + quoter + "</span><br/>"
                        : ""
                )
                + bCItem.toHtml()
                + "</div>";
    }

    public override string toOmit()
    {
        return bCItem.toOmit();
    }
}
