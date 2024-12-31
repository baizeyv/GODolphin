using System.Collections.Generic;

namespace GODolphin.BBCode;

public class BCCode : BCItem
{
    protected internal string code;

    public BCCode(string code)
    {
        this.code = code;
    }

    public BCCode(List<BCToken> list, int start, int end)
    {
        code = "";
        for (int i = start; i < end; i++)
        {
            code += list[i].ToString();
        }
    }

    public override string ToString()
    {
        return "[code]" + code + "[/code]";
    }

    public override string toHtml()
    {
        return "<div class=\"code\">" + code + "</div>";
    }

    public override string toOmit()
    {
        return "[代码]" + code.Replace('\n', ' ');
    }
}
