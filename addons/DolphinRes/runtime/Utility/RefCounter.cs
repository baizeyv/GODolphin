namespace GODolphin.Res;

public interface IRefCounter
{
    int RefCount { get; }

    void Retain(object refOwner = null);

    void Release(object refOwner = null);
}

public class SimplerRefCounter : IRefCounter
{
    public int RefCount { get; private set; }

    public SimplerRefCounter()
    {
        RefCount = 0;
    }

    public void Retain(object refOwner = null)
    {
        ++RefCount;
    }

    public void Release(object refOwner = null)
    {
        --RefCount;
        if (RefCount == 0)
        {
            // * 引用归零
            OnZeroRef();
        }
    }

    protected virtual void OnZeroRef() { }
}
