using System;
using System.Collections.Generic;
using GODolphin.Reactive;

namespace GODolphin.Event;

public class StringEvent : IDisposable
{
    private Dictionary<string, ISimpleEvent> _events = new();

    private readonly object _gate = new();

    public IDisposable Subscribe(string stringEvent, Observer<Unit> observer)
    {
        try
        {
            var subscription = SubscribeCore(stringEvent, observer);
            observer.SourceSubscription.Disposable = subscription;
            return observer; // return observer to make subscription chain
        }
        catch
        {
            observer.Dispose();
            throw;
        }
    }

    private IDisposable SubscribeCore(string stringEvent, Observer<Unit> observer)
    {
        if (_events.TryGetValue(stringEvent, out var evt))
        {
            var simpleEvent = evt as SimpleEvent;
            return simpleEvent?.Subscribe(observer);
        }
        else
        {
            var simpleEvent = new SimpleEvent();
            _events.Add(stringEvent, simpleEvent);
            return simpleEvent.Subscribe(observer);
        }
    }

    public IDisposable Subscribe<T>(string stringEvent, Observer<T> observer)
    {
        try
        {
            var subscription = SubscribeCore(stringEvent, observer);
            observer.SourceSubscription.Disposable = subscription;
            return observer; // return observer to make subscription chain
        }
        catch
        {
            observer.Dispose();
            throw;
        }
    }

    private IDisposable SubscribeCore<T>(string stringEvent, Observer<T> observer)
    {
        if (_events.TryGetValue(stringEvent, out var evt))
        {
            var simpleEvent = evt as SimpleEvent<T>;
            return simpleEvent?.Subscribe(observer);
        }
        else
        {
            var simpleEvent = new SimpleEvent<T>();
            _events.Add(stringEvent, simpleEvent);
            return simpleEvent.Subscribe(observer);
        }
    }

    public void Trigger(string stringEvent)
    {
        if (!_events.TryGetValue(stringEvent, out var evt))
            return;
        var simpleEvent = evt as SimpleEvent;
        simpleEvent?.Trigger();
    }

    public void Trigger<T>(string stringEvent, T data)
    {
        if (!_events.TryGetValue(stringEvent, out var evt))
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
