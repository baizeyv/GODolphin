using System.Collections;
using System.Collections.Generic;

namespace GODolphin.BBCode;

public class BCItems : BCItem, IEnumerable<BCItem>
{
    protected internal IList<BCItem> itemList;

    public BCItems()
    {
        itemList = new List<BCItem>();
    }

    public BCItems(BCItem item)
        : this()
    {
        itemList.Add(item);
    }

    public BCItems(IList<BCItem> items)
    {
        itemList = items;
    }

    public override string ToString()
    {
        string @string = "";
        foreach (BCItem item in itemList)
        {
            @string += item;
        }
        return @string;
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public virtual IEnumerator<BCItem> GetEnumerator()
    {
        return itemList.GetEnumerator();
    }

    public override string toHtml()
    {
        string @string = "";
        foreach (BCItem item in itemList)
        {
            @string += item.toHtml();
        }
        return @string;
    }

    public override string toOmit()
    {
        string @string = "";
        if (itemList.Count < 1)
        {
            return @string;
        }
        foreach (BCItem item in itemList)
        {
            if (!(item is BCQuote))
            {
                @string += item.toOmit() + " ";
            }
            else
            {
                @string += "[引用] ";
            }
        }
        return @string;
    }
}
