using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager
{
    private List<EventListener> listeners = new List<EventListener>();

    public void Propagate(GameEvent gameEvent)
    {
        foreach (var item in listeners)
            item.OnEvent(gameEvent);
    }

    public void Register(EventListener listener)
    {
        listeners.Add(listener);
    }
}
