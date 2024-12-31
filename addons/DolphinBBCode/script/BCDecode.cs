using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace GODolphin.BBCode;

public class BCDecode
{
    /// <summary>
    /// 保存词法分析后的token列表
    /// </summary>
    protected internal List<BCToken> tokenList;

    /// <summary>
    /// 保存字体大小的栈
    /// </summary>
    protected internal LinkedList<int> sizeStack;

    /// <summary>
    /// 保存字体颜色的栈
    /// </summary>
    protected internal LinkedList<string> colorStack;

    /// <summary>
    /// 字体的u、b、i属性栈
    /// </summary>
    protected internal int uStack;
    protected internal int bStack;
    protected internal int iStack;

    /// <param name="string"> 传入一个BBCode的String来构造一个BCDecode对象 </param>
    public BCDecode(string @string)
    {
        sizeStack = new LinkedList<int>();
        colorStack = new LinkedList<string>();

        //使用正则表达式来获取token
        var regexPattern = StaticVal.regex;
        Regex regex = new(regexPattern, RegexOptions.IgnoreCase);
        var m = regex.Matches(@string);
        // Pattern p = Pattern.compile(StaticVal.regex, Pattern.CASE_INSENSITIVE);
        // Matcher m = p.matcher(@string);
        tokenList = new List<BCToken>();
        int lastEnd = 0;
        foreach (var match in m)
        {
            if (lastEnd != ((Match)match).Index)
            {
                tokenList.Add(
                    new BCToken(
                        @string.Substring(lastEnd, ((Match)match).Index - lastEnd),
                        StaticVal.STRING
                    )
                );
            }
            string token = ((Match)match).Value;
            tokenList.Add(new BCToken(token, getTokenType(token)));
            lastEnd = ((Match)match).Index + ((Match)match).Length;
        }
        if (lastEnd != @string.Length)
        {
            tokenList.Add(
                new BCToken(@string.Substring(lastEnd, @string.Length - lastEnd), StaticVal.STRING)
            );
        }
    }

    /// <summary>
    /// 传入一个未知的String来判断其Token类型 </summary>
    /// <param name="token"> 一个未知的token String </param>
    /// <returns> Integer token 类型 </returns>
    public static int getTokenType(string token)
    {
        switch (token)
        {
            case "[i]":
                return StaticVal.I_OPEN;
            case "[/i]":
                return StaticVal.I_CLOSE;
            case "[u]":
                return StaticVal.U_OPEN;
            case "[/u]":
                return StaticVal.U_CLOSE;
            case "[b]":
                return StaticVal.B_OPEN;
            case "[/b]":
                return StaticVal.B_CLOSE;
            case "[quote]":
                return StaticVal.QUOTE_OPEN;
            case "[/quote]":
                return StaticVal.QUOTE_CLOSE;
            case "[pre]":
                return StaticVal.PRE_OPEN;
            case "[/pre]":
                return StaticVal.PRE_CLOSE;
            case "[/size]":
                return StaticVal.SIZE_CLOSE;
            case "[/color]":
                return StaticVal.COLOR_CLOSE;
            case "[url]":
                return StaticVal.URL_OPEN;
            case "[/url]":
                return StaticVal.URL_CLOSE;
            case "[code]":
                return StaticVal.CODE_OPEN;
            case "[/code]":
                return StaticVal.CODE_CLOSE;
        }
        switch (token.Substring(1, 1))
        {
            case "q":
                if (token.Substring(7, 1).Equals("游"))
                {
                    return StaticVal.QUOTE_OPEN_AID;
                }
                else
                {
                    return StaticVal.QUOTE_OPEN_ID;
                }
            case "s":
                return StaticVal.SIZE_OPEN;
            case "c":
                return StaticVal.COLOR_OPEN;
            case "i":
                return StaticVal.IMG_OPEN_WITH_SRC;
            case "u":
                return StaticVal.URL_OPEN_WITH_URL;
            case "e":
                return StaticVal.EM;
        }
        Regex urlRegex = new(StaticVal.urlRegex, RegexOptions.IgnoreCase);
        if (urlRegex.IsMatch(token))
        {
            return StaticVal.URL;
        }
        Regex imgRegex = new(StaticVal.urlRegex, RegexOptions.IgnoreCase);
        if (imgRegex.IsMatch(token))
        {
            return StaticVal.IMG;
        }
        return StaticVal.UNKNOWN;
    }

    public virtual string getValue(int i)
    {
        return tokenList[i].value;
    }

    /// <summary>
    /// 通过字体的各种属性栈获取当前字体的Style </summary>
    /// <returns> 当前字体的Style </returns>
    protected internal virtual BCStyle Style
    {
        get
        {
            int size = sizeStack.Count > 0 ? sizeStack.Last.Value : 0;
            string color = colorStack.Count > 0 ? colorStack.Last.Value : null;
            return new BCStyle(size, color, uStack > 0, iStack > 0, bStack > 0);
        }
    }

    /// <summary>
    /// 取得字体颜色 </summary>
    /// <param name="token"> 形如"[color=***]”的token </param>
    /// <returns> String 当前字体的color </returns>
    public static string getColor(string token)
    {
        string color = token.Substring(7, (token.Length - 1) - 7);
        return color;
    }

    /// <summary>
    /// 取得字体大小 </summary>
    /// <param name="token"> 形如"[size=5]”的token </param>
    /// <returns> Integer 当前字体的size </returns>
    public static int? getSize(string token)
    {
        return int.Parse(token.Substring(6, 1));
    }

    /// <summary>
    /// 取得被引用者的ID </summary>
    /// <param name="token"> 形如"[quote=***]”的token </param>
    /// <returns> String 被引用者的ID </returns>
    public static string getQuoter(string token)
    {
        return token.Length > 7 ? token.Substring(7, (token.Length - 1) - 7) : null;
    }

    public virtual BCItem Item
    {
        get { return getItem(0, tokenList.Count); }
    }

    /// <summary>
    /// 获取解析后的BBCode对象 </summary>
    /// <param name="start"> tokenList的起始索引 </param>
    /// <param name="end"> tokenList的结束索引 </param>
    /// <returns> 一个BCItem </returns>

    public virtual BCItem getItem(int start, int end)
    {
        if (start >= end)
        {
            return new BCString("", null);
        }
        int closeEnd;

        IList<BCItem> list = new List<BCItem>();

        for (int i = start; i < end; i++)
        {
            switch (tokenList[i].type)
            {
                //单Token直接转换为对象
                case StaticVal.URL:
                    list.Add(new BCUrl(getValue(i), Style));
                    continue;
                case StaticVal.EM:
                    list.Add(new BCEmoji(getValue(i).Substring(3, (getValue(i).Length - 1) - 3)));
                    continue;
                case StaticVal.IMG_OPEN_WITH_SRC:
                    list.Add(new BCImg(getValue(i).Substring(5, (getValue(i).Length - 1) - 5)));
                    continue;
                case StaticVal.IMG:
                    list.Add(new BCImg(getValue(i).Substring(5, (getValue(i).Length - 6) - 5)));
                    continue;

                //如果是字体属性则修改当前字体Style的属性
                case StaticVal.B_OPEN:
                    bStack++;
                    continue;
                case StaticVal.U_OPEN:
                    uStack++;
                    continue;
                case StaticVal.I_OPEN:
                    iStack++;
                    continue;
                case StaticVal.B_CLOSE:
                    if (bStack > 0)
                    {
                        bStack--;
                        continue;
                    }
                    else
                    {
                        break;
                    }
                case StaticVal.U_CLOSE:
                    if (uStack > 0)
                    {
                        uStack--;
                        continue;
                    }
                    else
                    {
                        break;
                    }
                case StaticVal.I_CLOSE:
                    if (iStack > 0)
                    {
                        iStack--;
                        continue;
                    }
                    else
                    {
                        break;
                    }
                case StaticVal.SIZE_OPEN:
                    sizeStack.AddFirst((int)getSize(getValue(i)));
                    continue;
                case StaticVal.SIZE_CLOSE:
                    if (sizeStack.Count > 0)
                    {
                        sizeStack.RemoveFirst();
                        continue;
                    }
                    else
                    {
                        break;
                    }
                case StaticVal.COLOR_OPEN:
                    colorStack.AddFirst(getColor(getValue(i)));
                    continue;
                case StaticVal.COLOR_CLOSE:
                    if (colorStack.Count > 0)
                    {
                        colorStack.RemoveFirst();
                        continue;
                    }
                    else
                    {
                        break;
                    }

                //对于需要寻找封闭标签的Token 在找到其对应的闭标签后递归调用getItem来获取标签里的BCItem
                case StaticVal.QUOTE_OPEN:
                case StaticVal.QUOTE_OPEN_AID:
                case StaticVal.QUOTE_OPEN_ID:
                    closeEnd = FindCloseTag(
                        i + 1,
                        end,
                        StaticVal.QUOTE_CLOSE,
                        StaticVal.QUOTE_OPEN,
                        StaticVal.QUOTE_OPEN_AID,
                        StaticVal.QUOTE_OPEN_ID
                    );
                    if (closeEnd > i)
                    {
                        list.Add(new BCQuote(getQuoter(getValue(i)), getItem(i + 1, closeEnd)));
                        i = closeEnd;
                        continue;
                    }
                    else
                    {
                        break;
                    }

                case StaticVal.URL_OPEN:
                    if (
                        tokenList.Count > i + 2
                        && tokenList[i + 2].type == StaticVal.URL_CLOSE
                        && tokenList[i + 1].type == StaticVal.URL
                    )
                    {
                        list.Add(new BCUrl(getValue(i + 1), Style));
                        i += 2;
                        continue;
                    }
                    else
                    {
                        break;
                    }

                case StaticVal.URL_OPEN_WITH_URL:
                    if (tokenList.Count > i + 2 && tokenList[i + 2].type == StaticVal.URL_CLOSE)
                    {
                        list.Add(
                            new BCUrl(
                                getValue(i + 1),
                                getValue(i).Substring(5, (getValue(i).Length - 1) - 5),
                                Style
                            )
                        );
                        i += 2;
                        continue;
                    }
                    else
                    {
                        break;
                    }

                case StaticVal.CODE_OPEN:
                    closeEnd = FindCloseTag(i + 1, end, StaticVal.CODE_CLOSE);
                    if (closeEnd > i)
                    {
                        list.Add(new BCCode(tokenList, i + 1, closeEnd));
                        i = closeEnd;
                        continue;
                    }
                    else
                    {
                        break;
                    }

                case StaticVal.PRE_OPEN:
                    closeEnd = FindCloseTag(i + 1, end, StaticVal.PRE_CLOSE);
                    if (closeEnd > i)
                    {
                        list.Add(new BCQuote(getItem(i + 1, closeEnd), true));
                        i = closeEnd;
                        continue;
                    }
                    else
                    {
                        break;
                    }
            }
            list.Add(new BCString(getValue(i), Style));
        }
        //若列表为空则返回一个空字符对象
        if (list.Count == 0)
        {
            return new BCString("", null);
        }
        //若为列表里为单值则只返回其第一个对象
        else if (list.Count == 1)
        {
            return list[0];
        }
        //否则返回一个由此列表构成的BCItems
        else
        {
            return new BCItems(list);
        }
    }

    /// <summary>
    /// 用于寻找对应的闭标签 </summary>
    /// <param name="start"> tokenList的起始位置 </param>
    /// <param name="end"> tokenList的结束位置 </param>
    /// <param name="closeTag"> 寻找的闭标签 </param>
    /// <param name="openTags"> 对应的开标签 </param>
    /// <returns> Integer 若找到则返回tokenList的Index 否则返回未封闭的层数的负值 </returns>
    public virtual int FindCloseTag(int start, int end, int closeTag, params int?[] openTags)
    {
        //默认封闭层数为-1
        int num = -1;

        for (int i = start; i < end; i++)
        {
            if (tokenList[i].type == closeTag)
            {
                num++;
                if (num == 0)
                {
                    //已封闭则返回当前的索引
                    return i;
                }
            }
            foreach (int? openTag in openTags)
            {
                if (tokenList[i].type == openTag.Value)
                {
                    num--;
                }
            }
        }
        return num;
    }
}
