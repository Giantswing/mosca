using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyObjectScript : MonoBehaviour
{
    [SerializeField] private int FXIndex;

    public void Destroy()
    {
        FXMaster.SpawnFX(transform.position, FXIndex);
        Destroy(gameObject);
    }
}