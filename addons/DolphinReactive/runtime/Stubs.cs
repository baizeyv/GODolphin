using System;

namespace GODolphin.Reactive;

internal static class Stubs
{
    internal static readonly Action<Result> HandleResult = (result) =>
    {
        if (result.IsFailure)
        {
            ObservableSystem.GetUnhandledExceptionHandler().Invoke(result.Exception);
        }
    };
}
