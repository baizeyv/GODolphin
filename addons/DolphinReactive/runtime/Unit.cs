using System;

namespace GODolphin.Reactive;

public readonly struct Unit : IEquatable<Unit>
{
    public static readonly Unit Default = default;

    public static bool operator ==(Unit first, Unit second) => true;

    public static bool operator !=(Unit first, Unit second) => false;

    public bool Equals(Unit other)
    {
        return true;
    }

    public override bool Equals(object obj)
    {
        return obj is Unit;
    }

    public override int GetHashCode()
    {
        return 0;
    }

    public override string ToString()
    {
        return "()";
    }
}
