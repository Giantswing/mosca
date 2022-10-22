using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyObjectScript : MonoBehaviour
{
    [SerializeField] private int FXIndex;

    public void Destroy()
    {
        EffectHandler.SpawnFX(FXIndex, transform.position, Vector3.zero,
            Vector3.zero, 0);
        Destroy(gameObject);
    }
}