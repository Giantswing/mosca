using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class EntityInside
{
    public Transform entityTransform;
    public Attributes entityAttributes;
}

public class ParentChanger : MonoBehaviour
{
    //[SerializeField] private float timeInMax = 0.2f;
    //[SerializeField] private bool isPlayerIn;
    //[SerializeField] private bool changedParent;
    //[SerializeField] private Transform otherTransform;
    [SerializeField] private List<EntityInside> listOfEntities = new();
    private EventCaller _eventCaller;

    private float _timeIn;

    // Start is called before the first frame update
    private void Start()
    {
        if (gameObject.name.Contains("convex_collision") == false) Destroy(this);
        _eventCaller = GetComponentInParent<EventCaller>();
        //timeInMax = 0.2f;
    }

    // Update is called once per frame
    /*
    private void Update()
    {
        if (isPlayerIn && !changedParent)
            _timeIn += Time.deltaTime;
        if (_timeIn > timeInMax)
        {
            otherTransform.parent = transform;
            changedParent = true;
            _timeIn = 0;
            _eventCaller.OnStartEvent?.Invoke();
        }
    }
    */

    private bool CheckIfEntityIsInside(Transform other_transform)
    {
        for (var i = 0; i < listOfEntities.Count; i++)
            if (listOfEntities[i].entityTransform == other_transform)
                return true;
        return false;
    }

    public bool IsSomeoneInside()
    {
        return listOfEntities.Count > 0;
    }

    public bool AreAllPlayersInside()
    {
        int playersInLevel = TargetGroupControllerSystem.Instance.playerList.Count;

        var playersInside = 0;

        for (var i = 0; i < listOfEntities.Count; i++)
            if (listOfEntities[i].entityAttributes.GetComponent<PlayerIdentifier>())
                playersInside++;

        return playersInLevel == playersInside;
    }


    private void AddEntity(Transform other_transform, Attributes other_attributes)
    {
        listOfEntities.Add(new EntityInside { entityTransform = other_transform, entityAttributes = other_attributes });
        other_transform.parent = transform;
        //other_stats.IsInsideElevator = true;
    }

    private void RemoveEntity(Transform other_transform)
    {
        for (var i = 0; i < listOfEntities.Count; i++)
            if (listOfEntities[i].entityTransform == other_transform)
            {
                listOfEntities[i].entityTransform.parent = null;
                //listOfEntities[i].entityStats.IsInsideElevator = false;
                listOfEntities.RemoveAt(i);
                break;
            }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            /*
            otherTransform = other.transform;
            isPlayerIn = true;
            STATS otherStats = other.GetComponent<STATS>();
            otherStats.IsInsideElevator = true;
            */
            if (CheckIfEntityIsInside(other.transform)) return;
            AddEntity(other.transform, other.GetComponent<Attributes>());

            if (AreAllPlayersInside())
                _eventCaller.OnStartEvent?.Invoke();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
            /*
            other.transform.parent = null;
            isPlayerIn = false;
            changedParent = false;
            _timeIn = 0;
            */
            RemoveEntity(other.transform);
    }

    /*
    public void SetTimeMax(float time)
    {
        timeInMax = time;
    }
    */
}