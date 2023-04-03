using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrolPointHolder : MonoBehaviour
{
    public List<PatrolPoint> patrolPoints = new();
    private Vector3 start_pos;

    public void ResetPos()
    {
        transform.position = start_pos;
    }

    private void Start()
    {
        start_pos = transform.position;

        foreach (var patrol in patrolPoints)
        {
            var instance = new GameObject();
            //patrol.instance = instance.transform;
            instance.transform.parent = transform;
        }
    }
}