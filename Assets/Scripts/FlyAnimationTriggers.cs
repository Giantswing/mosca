using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyAnimationTriggers : MonoBehaviour
{
    [SerializeField] private FlyMovement flyMovement;

    // Start is called before the first frame update
    private void Start()
    {
        flyMovement = GetComponentInParent<FlyMovement>();
    }

    public void StopDashBoost()
    {
        flyMovement.StopDashBoost();
    }

    public void StartDashActiveFrames()
    {
        flyMovement.StartDashBoost();
    }
}