using System;
using System.Collections.Generic;

public class EventManager
{
    private readonly Dictionary<EventType, Action> _uiEventHandler;

    public EventManager()
    {
        _uiEventHandler = new Dictionary<EventType, Action>();
    }

    public void AddEvent(EventType type, Action action)
    {
        if (!_uiEventHandler.ContainsKey(type))
        {
            _uiEventHandler.Add(type, action);
        }
        else
        {
            _uiEventHandler[type] -= action;
            _uiEventHandler[type] += action;
        }
    }

    public void RemoveEvent(EventType type, Action action)
    {
        _uiEventHandler[type] -= action;
    }

    public void CallBackEvent(EventType type)
    {
        if (_uiEventHandler.ContainsKey(type))
        {
            _uiEventHandler[type]?.Invoke();
        }
    }
}
