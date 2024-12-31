using System;
using System.Collections.Generic;

namespace GODolphin.Reactive;

public sealed class ClampedReactiveVariable<T> : ReactiveVariable<T>
    where T : IComparable<T>
{
    private readonly T min,
        max;

    private static IComparer<T> Comparer { get; } = Comparer<T>.Default;

    public ClampedReactiveVariable(T initialValue, T min, T max)
        : base(initialValue)
    {
        this.min = min;
        this.max = max;
    }

    protected override void OnValueChanging(ref T value)
    {
        if (Comparer.Compare(value, min) < 0)
        {
            value = min;
        }
        else if (Comparer.Compare(value, max) > 0)
        {
            value = max;
        }
    }
}
