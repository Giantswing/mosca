using System;
using System.Collections;
using System.Linq;
using DG.Tweening;
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

    public enum OnDeathBehaviour
    {
        Fly,
        Immediate
    }

    [PropertySpace(SpaceBefore = 15f, SpaceAfter = 0)]
    public Team team;

    [TitleGroup("Movement")] [HorizontalGroup("Movement/Speed")]
    public float speed;

    [HorizontalGroup("Movement/Speed", LabelWidth = 90f)]
    public float acceleration;

    [TitleGroup("Health")] [HorizontalGroup("Health/Health", LabelWidth = 60f)] [ReadOnly]
    public int HP;

    [HorizontalGroup("Health/Health", LabelWidth = 60f)] [PropertySpace(SpaceBefore = 0, SpaceAfter = 6f)]
    public int maxHP;

    [HorizontalGroup("Health/Invulnerability", LabelWidth = 150f)]
    public float invulnerabilityTime = 1f;

    [HorizontalGroup("Health/Invulnerability", LabelWidth = 90f)]
    public bool canReceiveDamage;

    [TitleGroup("Damage")] [HorizontalGroup("Damage/Damage")]
    public int damage;

    [TitleGroup("Damage")] [ShowIf("canReceiveDamage")]
    public OnDeathBehaviour onDeathBehaviour;

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

    [FoldoutGroup("Events", false)] public UnityEvent onHeal;

    [FoldoutGroup("Sound", false)] public bool hasHitSound = false;

    [FoldoutGroup("Sound", false)] [ShowIf("hasHitSound")]
    public SoundListAuto hitSound;

    [FoldoutGroup("Sound", false)] public bool hasDeathSound = false;

    [FoldoutGroup("Sound", false)] [ShowIf("hasDeathSound")]
    public SoundListAuto deathSound;

    [FoldoutGroup("Sound", false)] public bool hasBumpSound = false;

    [FoldoutGroup("Sound", false)] [ShowIf("hasBumpSound")]
    public SoundListAuto bumpSound;

    [FoldoutGroup("FX", false)] public bool hasHitFX = false;

    [FoldoutGroup("FX", false)] [ShowIf("hasHitFX")]
    public FXListAuto hitFX;

    [FoldoutGroup("FX", false)] public bool hasDeathFX = false;

    [FoldoutGroup("FX", false)] [ShowIf("hasDeathFX")]
    public FXListAuto deathFX;

    [FoldoutGroup("FX", false)] public bool hasBumpFX = false;

    [FoldoutGroup("FX", false)] [ShowIf("hasBumpFX")]
    public FXListAuto bumpFX;

    private Rigidbody rb;
    private bool hasRigidbody = false;
    private Coroutine invulnerabilityCoroutine;

    private bool hasHardCollider = false;
    [HideInInspector] public Collider hardCollider;


    private void Awake()
    {
        HP = maxHP;
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

        hardCollider = GetComponents<Collider>().FirstOrDefault(collider => collider.isTrigger == false);
        if (hardCollider != null)
            hasHardCollider = true;
    }

    public void DeathEvent(Vector3 deathPos)
    {
        switch (onDeathBehaviour)
        {
            case OnDeathBehaviour.Fly:
                //disable all components except for this attributes and rigidbody
                foreach (MonoBehaviour component in GetComponents<MonoBehaviour>())
                {
                    if (component is Attributes)
                        continue;

                    component.enabled = false;
                }

                if (hasRigidbody)
                {
                    print("b");

                    rb.mass = 10f;
                    rb.drag = 0;
                    rb.freezeRotation = false;
                    rb.useGravity = true;

                    rb.AddForce((transform.position - deathPos).normalized * 3f,
                        ForceMode.Impulse);
                    rb.AddForce(Vector3.up * 5f, ForceMode.Impulse);
                    rb.AddTorque(Vector3.forward * 1000f, ForceMode.Impulse);
                    objectModel.DOLocalRotate(new Vector3(0, 0, 360f), 0.4f).SetLoops(5);
                }

                DOVirtual.DelayedCall(1f, () =>
                {
                    onDeath?.Invoke();
                    transform.DOKill();
                });

                break;

            case OnDeathBehaviour.Immediate:
                Destroy(gameObject);
                break;
        }
    }


    public void TakeDamage(Attributes otherAttributes)
    {
        if (otherAttributes.team != team && otherAttributes.canDoDamage && canReceiveDamage)
        {
            HP -= otherAttributes.damage;
            otherAttributes.onDidHit.Invoke();

            invulnerabilityCoroutine = StartCoroutine(InvulnerabilityCoroutine());

            if (hasHitSound)
                SoundMaster.PlaySound(transform.position, (int)hitSound);

            if (hasHitFX)
                FXMaster.SpawnFX(transform.position, (int)hitFX);

            objectModel.DOShakeScale(1f, otherAttributes.damage);
            objectModel.DOShakePosition(1f, otherAttributes.damage);

            if (hasRigidbody)
            {
                Vector3 damageDirection = (transform.position - otherAttributes.transform.position).normalized;
                rb.AddForce(damageDirection * otherAttributes.damage * 20f, ForceMode.Impulse);
            }

            if (HP <= 0 && HP > -665)
            {
                if (hasDeathSound)
                    SoundMaster.PlaySound(transform.position, (int)deathSound);

                if (hasDeathFX)
                    FXMaster.SpawnFX(transform.position, (int)deathFX);
                //onDeath.Invoke();
                HP = -666;
                onReceiveHit.Invoke();
                DeathEvent(otherAttributes.transform.position);
            }
            else if (HP > 0)
            {
                onReceiveHit.Invoke();
            }
        }
    }

    public void TakeDamageStatic(Vector3 damagePos, int damageAmount, float forceMultiplier = 1f)
    {
        HP -= damageAmount;
        invulnerabilityCoroutine = StartCoroutine(InvulnerabilityCoroutine());

        if (hasHitSound)
            SoundMaster.PlaySound(transform.position, (int)hitSound);
        //SoundMaster.PlayTargetSound(transform.position, hitSound);

        if (hasHitFX)
            FXMaster.SpawnFX(transform.position, (int)hitFX);

        objectModel.DOShakeScale(1f, damageAmount);
        objectModel.DOShakePosition(1f, damageAmount);

        if (hasRigidbody)
        {
            Vector3 damageDirection = (transform.position - damagePos).normalized;
            rb.AddForce(damageDirection * 20f * forceMultiplier, ForceMode.Impulse);
        }

        if (HP <= 0 && HP > -665)
        {
            //onDeath.Invoke();
            if (hasDeathSound)
                SoundMaster.PlaySound(transform.position, (int)deathSound);

            if (hasDeathFX)
                FXMaster.SpawnFX(transform.position, (int)deathFX);

            HP = -666;
            onReceiveHit.Invoke();
            DeathEvent(damagePos);
        }
        else if (HP > 0)
        {
            onReceiveHit.Invoke();
        }
    }

    public void Heal(int amount)
    {
        HP += amount;
        if (HP > maxHP)
            HP = maxHP;

        onHeal.Invoke();
    }

    public void ChangeMaxHP(int amount)
    {
        maxHP += amount;
        if (HP > maxHP)
            HP = maxHP;
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
        if (hasBumpSound)
            SoundMaster.PlaySound(transform.position, (int)bumpSound);

        if (hasBumpFX)
            FXMaster.SpawnFX(transform.position, (int)bumpFX);


        if (collision.transform.TryGetComponent(out Attributes otherAttributes))
            TakeDamage(otherAttributes);

        if (canInteract && collision.transform.TryGetComponent(out IGenericInteractable interactable))
            interactable.Interact(transform.position);
    }
}