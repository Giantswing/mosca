using UnityEngine;
using UnityEngine.Events;

public class EventCaller : MonoBehaviour
{
    public UnityAction OnEndEvent;
    public UnityAction OnStartEvent;

    public void StartEvent()
    {
        OnStartEvent?.Invoke();
    }

    public void EndEvent()
    {
        OnEndEvent?.Invoke();
    }
}