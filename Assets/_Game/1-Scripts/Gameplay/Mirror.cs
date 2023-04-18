using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class Mirror : MonoBehaviour
{
    [SerializeField] private GameObject clonePrefab;
    [SerializeField] private float threshold = 5f;
    [ReadOnly] public GameObject clone;

    private void Start()
    {
        InitializeClone();
    }

    private void Update()
    {
        CheckIfCloneShouldBeReset();
    }

    private void CheckIfCloneShouldBeReset()
    {
        /*
        if (Vector3.Distance(playerReference.playerTransform.position, transform.position) < threshold)
            //if the clone is too far away from the mirror
            if (Vector3.Distance(clone.transform.position, transform.position) > threshold + 0.3f)
            {
                print("resetting clone");
                cloneScript.MoveToMirroredPosition();
            }
            */
    }

    private void InitializeClone()
    {
        clone = Instantiate(clonePrefab, transform.position, transform.rotation);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, threshold);
    }
}