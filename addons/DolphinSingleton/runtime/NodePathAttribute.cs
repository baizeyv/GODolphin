using System;

namespace GODolphin.Singleton;

public class NodePathAttribute : Attribute
{
    public NodePathAttribute(string nodeName)
    {
        NodeName = nodeName;
    }

    public string NodeName { get; private set; }
}
