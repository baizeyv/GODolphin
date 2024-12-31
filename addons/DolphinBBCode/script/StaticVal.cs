namespace GODolphin.BBCode;

public class StaticVal
{
    public const int UNKNOWN = -1;
    public const int STRING = 0;
    public const int QUOTE_OPEN = 1;
    public const int QUOTE_OPEN_ID = 2;
    public const int QUOTE_OPEN_AID = 3;
    public const int SIZE_OPEN = 4;
    public const int COLOR_OPEN = 5;
    public const int U_OPEN = 6;
    public const int I_OPEN = 7;
    public const int B_OPEN = 8;
    public const int IMG = 9;
    public const int IMG_OPEN_WITH_SRC = 10;
    public const int URL_OPEN = 11;
    public const int URL_OPEN_WITH_URL = 12;
    public const int EM = 13;
    public const int PRE_OPEN = 15;
    public const int CODE_OPEN = 16;
    public const int URL = 17;

    public const int QUOTE_CLOSE = 50;
    public const int SIZE_CLOSE = 51;
    public const int COLOR_CLOSE = 52;
    public const int U_CLOSE = 53;
    public const int I_CLOSE = 54;
    public const int B_CLOSE = 55;
    public const int URL_CLOSE = 57;
    public const int PRE_CLOSE = 58;
    public const int CODE_CLOSE = 59;

    public static string regex;

    public const string urlRegex = "[a-zA-z]+://[^\\s]*";
    public const string imgRegex =
        "\\[img\\][a-zA-z]+://[^\\s]*?(\\.jpg|\\.gif|\\.png|\\.jpeg)\\[/img\\]";

    public static readonly string[] tokenList = new string[]
    {
        "b",
        "/b",
        "i",
        "/i",
        "u",
        "/u",
        "img\\][a-zA-z]+://[^\\s]*?(\\.jpg|\\.gif|\\.png|\\.jpeg)\\[/img",
        "img=[a-zA-z]+://[^\\s]*?(\\.jpg|\\.gif|\\.png|\\.jpeg)",
        "url",
        "/url",
        "url=[a-zA-z]+://[^\\s+^\\[]*",
        "size=[1234567]",
        "/size",
        "color=[a-zA-Z0-9,\\#]{3,20}",
        "/color",
        "quote",
        "quote=[A-Za-z0-9\\u4e00-\\u9fa5,\\.,\\*,\\:]{1,40}",
        "/quote",
        "pre",
        "/pre",
        "em[0-9]{1,3}",
        "code",
        "/code",
    };

    static StaticVal()
    {
        regex = Regex;
    }

    private static string Regex
    {
        get
        {
            string regex = "";
            for (int i = 0; i < tokenList.Length; i++)
            {
                regex += "\\[" + tokenList[i] + "\\]|";
            }
            return regex + "[a-zA-z]+://[^\\s+^\\[]*";
        }
    }
}
