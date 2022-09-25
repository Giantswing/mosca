using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyAnimationTriggers : MonoBehaviour
{
    [SerializeField] private PlayerMovement flyMovement;

    // Start is called before the first frame update
    private void Start()
    {
        flyMovement = GetComponentInParent<PlayerMovement>();
    }

    public void StopDashBoost()
    {
        flyMovement.StopDashBoost();
    }


    public void StartDashBoost()
    {
        flyMovement.StartDashBoost();
    }


    public void StartDoubleDash()
    {
        flyMovement.StartDoubleDash();
    }

    public void StopDoubleDash()
    {
        flyMovement.StopDoubleDash();
    }

    public void AllowDoubleDash()
    {
        flyMovement.AllowDoubleDash();
    }
}