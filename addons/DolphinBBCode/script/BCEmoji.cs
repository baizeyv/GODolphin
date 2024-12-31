namespace GODolphin.BBCode;

public class BCEmoji : BCItem
{
    protected internal int emojiId;

    public BCEmoji(int emojiId)
    {
        this.emojiId = emojiId;
    }

    public BCEmoji(string emojiId)
        : this(int.Parse(emojiId)) { }

    public override string ToString()
    {
        return "[em" + emojiId + "]";
    }

    public override string toHtml()
    {
        return "<img src=\"file:///android_asset/em" + emojiId + ".gif\" alt=\"[em13]\">";
    }

    public override string toOmit()
    {
        return ToString();
    }
}
