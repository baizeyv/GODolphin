using System;
using System.Collections.Generic;
using Godot;

namespace GODolphin.Reactive;

public static class CustomEqualityComparer
{
    public static ColorEqualityComparer Color = new ColorEqualityComparer();
}

public class ColorEqualityComparer : IEqualityComparer<Color>
{
    public bool Equals(Color x, Color y)
    {
        return x.R.Equals(y.R)
            && x.G.Equals(y.G)
            && x.B.Equals(y.B)
            && x.A.Equals(y.A)
            && x.R8 == y.R8
            && x.G8 == y.G8
            && x.B8 == y.B8
            && x.A8 == y.A8
            && x.H.Equals(y.H)
            && x.S.Equals(y.S)
            && x.V.Equals(y.V)
            && x.Luminance.Equals(y.Luminance);
    }

    public int GetHashCode(Color obj)
    {
        var hashCode = new HashCode();
        hashCode.Add(obj.R);
        hashCode.Add(obj.G);
        hashCode.Add(obj.B);
        hashCode.Add(obj.A);
        hashCode.Add(obj.R8);
        hashCode.Add(obj.G8);
        hashCode.Add(obj.B8);
        hashCode.Add(obj.A8);
        hashCode.Add(obj.H);
        hashCode.Add(obj.S);
        hashCode.Add(obj.V);
        hashCode.Add(obj.Luminance);
        return hashCode.ToHashCode();
    }
}
