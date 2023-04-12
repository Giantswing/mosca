using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "Player Reference", menuName = "Mosca/Player Reference", order = 1)]
public class PlayerReferenceSO : ScriptableObject
{
    public GameObject playerGameObject;
    public Transform playerTransform;
    public Rigidbody playerRigidbody;
    public PlayerInteractionHandler playerInteractionHandler;
    public PlayerMovement playerMovement;

    public PlayerCamera playerCamera;

    /*-----*/
    public STATS playerStats;
    public STATS crownStats;
    public Transform crownTransform;
    public Crown crownScript;
    public Dash playerDash;
}