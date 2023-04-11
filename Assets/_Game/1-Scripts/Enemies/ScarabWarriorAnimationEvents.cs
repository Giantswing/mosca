using UnityEngine;

public class ScarabWarriorAnimationEvents : MonoBehaviour
{
    [SerializeField] private ScarabWarrior myScarabWarrior;

    public void SpearThrow()
    {
        myScarabWarrior.SpearThrow();
    }

    public void Dodge()
    {
        myScarabWarrior.Dodge();
    }
}