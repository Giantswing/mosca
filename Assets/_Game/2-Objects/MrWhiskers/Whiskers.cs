using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Whiskers : MonoBehaviour
{
    [SerializeField] private Animator animator;
    private static readonly int IsGettingOutside = Animator.StringToHash("isGettingOutside");

    public void ShowUp()
    {
        animator.SetBool(IsGettingOutside, true);
    }

    public void Hide()
    {
        animator.SetBool(IsGettingOutside, false);
    }
}