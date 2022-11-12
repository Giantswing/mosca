using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointScript : MonoBehaviour
{
    public bool isActivated = false;
    public int checkpointNumber;
    public bool pauseCheckpoint = false;

    private void Start()
    {
        LevelManager.GetCheckpoints().Add(this);
        LevelManager.ReorderCheckpoints();
    }

    private void OnDrawGizmos()
    {
        if (isActivated)
            Gizmos.color = Color.red;
        else
            Gizmos.color = Color.green;

        Gizmos.DrawWireSphere(transform.position, 4.5f);
    }
}