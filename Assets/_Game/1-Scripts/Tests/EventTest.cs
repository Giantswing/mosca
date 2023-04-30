using System;
using UnityEngine;
using UnityEngine.Events;

public class EventTest : MonoBehaviour
{
    [SerializeField] private UnityEvent<EventContext> testEvent;
    public SerializableAttribute testAttribute;

    private void OnTriggerEnter(Collider other)
    {
        testEvent.Invoke(new EventContext()
        {
            eventBool = true,
            eventInt = 1,
            eventFloat = 1.5f,
            eventString = "Hello World"
        });
    }
}