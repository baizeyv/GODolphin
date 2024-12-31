namespace GODolphin.BBCode;

public class BCToken
{
    public string value;
    public int type;

    public BCToken(string value, int type)
    {
        this.value = value;
        this.type = type;
    }

    public override string ToString()
    {
        return value;
    }
}
