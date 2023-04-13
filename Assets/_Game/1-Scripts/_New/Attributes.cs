using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

public class Attributes : MonoBehaviour
{
    [Serializable]
    public struct NormalRenderer
    {
        public Renderer renderer;
        public Material material;
    }

    public enum Team
    {
        Player,
        Neutral,
        Enemy
    }

    [PropertySpace(SpaceBefore = 15f, SpaceAfter = 0)]
    public Team team;

    [TitleGroup("Movement")] [HorizontalGroup("Movement/Speed")]
    public float speed;

    [HorizontalGroup("Movement/Speed", LabelWidth = 90f)]
    public float acceleration;

    [TitleGroup("Health")] [HorizontalGroup("Health/Health", LabelWidth = 80f)] [ReadOnly]
    public int health;

    [HorizontalGroup("Health/Health", LabelWidth = 90f)] [PropertySpace(SpaceBefore = 0, SpaceAfter = 6f)]
    public int maxHealth;

    [HorizontalGroup("Health/Invulnerability", LabelWidth = 150f)]
    public float invulnerabilityTime = 1f;

    [HorizontalGroup("Health/Invulnerability", LabelWidth = 90f)]
    public bool canReceiveDamage;

    [TitleGroup("Damage")] [HorizontalGroup("Damage/Damage")]
    public int damage;

    [HorizontalGroup("Damage/Damage", LabelWidth = 120f)]
    public bool canDoDamage = true;

    [TitleGroup("Interactions")]
    [HorizontalGroup("Interactions/Interactions")]
    [PropertySpace(SpaceBefore = 0, SpaceAfter = 15f)]
    public bool canInteract = false;

    [FoldoutGroup("Rendering", false)] [ReadOnly]
    public Transform objectModel;

    [FoldoutGroup("Rendering", false)] [ReadOnly]
    public NormalRenderer[] renderers;

    [FoldoutGroup("Rendering", false)] [ReadOnly]
    public Material hurtMaterial;


    [FoldoutGroup("Events", false)] public UnityEvent onDeath;

    [FoldoutGroup("Events", false)] public UnityEvent onReceiveHit;

    [FoldoutGroup("Events", false)] public UnityEvent onDidHit;

    [FoldoutGroup("Sound", false)] public bool hasHitSound = false;

    [FoldoutGroup("Sound", false)] [ShowIf("hasHitSound")]
    public SimpleAudioEvent hitSound;

    [FoldoutGroup("Sound", false)] public bool hasDeathSound = false;

    [FoldoutGroup("Sound", false)] [ShowIf("hasDeathSound")]
    public SimpleAudioEvent deathSound;
    /*    public MovementType movementType;

    [ShowIf("movementType", Value = MovementType.Advanced)]
    public UnityEvent<string, string, Vector3> OnMove;

    [ShowIf("movementType", Value = MovementType.Simple)]
    public UnityEvent OnMoveSimple;*/


    private Rigidbody rb;
    private bool hasRigidbody = false;
    private Coroutine invulnerabilityCoroutine;


    private void Awake()
    {
        health = maxHealth;
        hurtMaterial = Resources.Load<Material>("Materials/Hurt");

        renderers = GetComponentsInChildren<Renderer>()
            .Where(renderer => renderer.GetComponent<TrailRenderer>() == null)
            .Select(renderer => new NormalRenderer { renderer = renderer, material = renderer.material })
            .ToArray();

        if (objectModel == null)
        {
            if (transform.childCount > 0)
                objectModel = transform.GetChild(0);
            else
                objectModel = transform;
        }

        if (TryGetComponent(out Rigidbody rigidbody))
        {
            rb = rigidbody;
            hasRigidbody = true;
        }
    }

    public void TakeDamage(Attributes otherAttributes)
    {
        if (otherAttributes.team != team && otherAttributes.canDoDamage && canReceiveDamage)
        {
            health -= damage;
            otherAttributes.onDidHit.Invoke();

            invulnerabilityCoroutine = StartCoroutine(InvulnerabilityCoroutine());

            if (health <= 0)
            {
                onDeath.Invoke();
                if (hasDeathSound)
                    SoundMaster.PlayTargetSound(transform.position, deathSound);
            }
            else
            {
                onReceiveHit.Invoke();
                if (hasHitSound)
                    SoundMaster.PlayTargetSound(transform.position, hitSound);
            }

            if (hasRigidbody)
            {
                Vector3 damageDirection = (transform.position - otherAttributes.transform.position).normalized;
                rb.AddForce(damageDirection * otherAttributes.damage * 20f, ForceMode.Impulse);
            }
        }
    }

    private IEnumerator InvulnerabilityCoroutine()
    {
        canReceiveDamage = false;
        foreach (NormalRenderer renderer in renderers)
            renderer.renderer.material = hurtMaterial;

        yield return new WaitForSeconds(invulnerabilityTime);

        canReceiveDamage = true;
        foreach (NormalRenderer renderer in renderers)
            renderer.renderer.material = renderer.material;
    }


    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.TryGetComponent(out Attributes otherAttributes))
            TakeDamage(otherAttributes);

        if (canInteract && collision.transform.TryGetComponent(out IGenericInteractable interactable))
            interactable.Interact(transform.position);
    }
}