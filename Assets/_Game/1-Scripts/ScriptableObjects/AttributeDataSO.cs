using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AttributeDataSO", menuName = "ScriptableObjects/AttributeDataSO",
    order = 0)]
public class AttributeDataSO : ScriptableObject
{
    public Attributes attributes;
    public MovementSystem movementSystem;
    public FlipSystem flipSystem;
    public DashAbility dashAbility;
    public DoubleDashAbility doubleDashAbility;
    public ChargeSystem chargeShot;
}