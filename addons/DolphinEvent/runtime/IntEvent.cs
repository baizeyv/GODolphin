using System;
using System.Collections.Generic;
using GODolphin.Reactive;

namespace GODolphin.Event;

public class IntEvent : IDisposable
{
    private Dictionary<int, ISimpleEvent> _events = new();

    private readonly object _gate = new();

    public IDisposable Subscribe(int intEvent, Observer<Unit> observer)
    {
        try
        {
            var subscription = SubscribeCore(intEvent, observer);
            observer.SourceSubscription.Disposable = subscription;
            return observer; // return observer to make subscription chain
        }
        catch
        {
            observer.Dispose();
            throw;
        }
    }

    private IDisposable SubscribeCore(int intEvent, Observer<Unit> observer)
    {
        if (_events.TryGetValue(intEvent, out var evt))
        {
            var simpleEvent = evt as SimpleEvent;
            return simpleEvent?.Subscribe(observer);
        }
        else
        {
            var simpleEvent = new SimpleEvent();
            _events.Add(intEvent, simpleEvent);
            return simpleEvent.Subscribe(observer);
        }
    }

    public IDisposable Subscribe<T>(int intEvent, Observer<T> observer)
    {
        try
        {
            var subscription = SubscribeCore(intEvent, observer);
            observer.SourceSubscription.Disposable = subscription;
            return observer; // return observer to make subscription chain
        }
        catch
        {
            observer.Dispose();
            throw;
        }
    }

    private IDisposable SubscribeCore<T>(int intEvent, Observer<T> observer)
    {
        if (_events.TryGetValue(intEvent, out var evt))
        {
            var simpleEvent = evt as SimpleEvent<T>;
            return simpleEvent?.Subscribe(observer);
        }
        else
        {
            var simpleEvent = new SimpleEvent<T>();
            _events.Add(intEvent, simpleEvent);
            return simpleEvent.Subscribe(observer);
        }
    }

    public void Trigger(int intEvent)
    {
        if (!_events.TryGetValue(intEvent, out var evt))
            return;
        var simpleEvent = evt as SimpleEvent;
        simpleEvent?.Trigger();
    }

    public void Trigger<T>(int intEvent, T data)
    {
        if (!_events.TryGetValue(intEvent, out var evt))
            return;
        var simpleEvent = evt as SimpleEvent<T>;
        simpleEvent?.Trigger(data);
    }

    public void Dispose()
    {
        lock (_gate)
        {
            foreach (var keyValuePair in _events)
            {
                keyValuePair.Value?.Dispose();
            }
            _events.Clear();
        }
    }
}
