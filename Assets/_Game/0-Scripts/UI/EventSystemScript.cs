using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class EventSystemScript : MonoBehaviour
{
    public static EventSystemScript Instance;

    private void Start()
    {
        Instance = this;
    }

    public static void ChangeFirstSelected(GameObject obj)
    {
        var eventSystem = EventSystem.current;
        eventSystem.SetSelectedGameObject(obj, new BaseEventData(eventSystem));
    }
}