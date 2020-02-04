using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface EventListener
{
    void OnEvent(GameEvent gameEvent);
}
