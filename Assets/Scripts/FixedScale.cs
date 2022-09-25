using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class FixedScale : MonoBehaviour
{
    public float FixeScale = 1;
    public GameObject parent;

    // Update is called once per frame
    private void Update()
    {
        var localScale = parent.transform.localScale;
        if (parent.transform.localScale.magnitude <= .01f) return;

        transform.localScale = new Vector3(FixeScale / localScale.x,
            FixeScale / localScale.y * -1f, FixeScale / localScale.z);
    }
}