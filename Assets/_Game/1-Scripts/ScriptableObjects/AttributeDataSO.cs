using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

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
    public PlayerInput playerInput;
}