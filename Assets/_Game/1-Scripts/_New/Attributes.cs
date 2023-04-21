using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class Attributes : MonoBehaviour, IPressurePlateListener
{
    public struct RigidbodyInfo
    {
        public float Mass;
        public float Drag;
        public float AngularDrag;
        public bool isKinematic;
        public bool useGravity;
        public RigidbodyConstraints constraints;
    }


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

    [HorizontalGroup("Damage/Damage", LabelWidth = 120f)]
    public bool canDoDamage = true;

    [HorizontalGroup("Damage/Damage2", LabelWidth = 100f)]
    public bool onlyExplosions = false;

    [HorizontalGroup("Damage/Damage2", LabelWidth = 100f)]
    public bool explosive = false;

    [HorizontalGroup("Damage/Damage2", LabelWidth = 100f)]
    public bool contactDamage = true;

    [TitleGroup("Damage")] [ShowIf("canReceiveDamage")]
    public OnDeathBehaviour onDeathBehaviour;

    [TitleGroup("Damage")] public int damagePriority = 1;


    [TitleGroup("Interactions")]
    [PropertySpace(SpaceBefore = 5f, SpaceAfter = 10f)]
    [HorizontalGroup("Interactions/Interactions1")]
    public bool canInteract = false;

    [HorizontalGroup("Interactions/Interactions1")]
    public bool disablePressurePlates = false;

    [HorizontalGroup("Interactions/Interactions")]
    public bool hasInstantiatedData = false;

    [HorizontalGroup("Interactions/Interactions")]
    public bool hasSharedData = false;

    [ShowIf("hasInstantiatedData")] public AttributeDataSO attributeDataPrefab;

    [ShowIf("hasInstantiatedData")] [ReadOnly]
    public AttributeDataSO attributeData;

    [ShowIf("hasSharedData")] public AttributeDataSO sharedData;


    [FoldoutGroup("Rendering", false)] [ReadOnly]
    public Transform objectModel;

    [FoldoutGroup("Rendering", false)] public bool manuallyAddedRenderers = false;

    [FoldoutGroup("Rendering", false)] [ShowIf("manuallyAddedRenderers")]
    public List<NormalRenderer> renderers;

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

    private RigidbodyInfo rbInfo;
    private bool hasHardCollider = false;
    [HideInInspector] public Collider hardCollider;


    private void Awake()
    {
        if (hasSharedData)
        {
            if (TargetGroupControllerSystem.Instance == null)
                GetAttributes().HP = GetAttributes().maxHP;
        }
        else
        {
            GetAttributes().HP = GetAttributes().maxHP;
        }

        hurtMaterial = Resources.Load<Material>("Materials/Hurt");

        if (!manuallyAddedRenderers)
            renderers = GetComponentsInChildren<Renderer>()
                .Where(renderer => renderer.GetComponent<TrailRenderer>() == null)
                .Where(renderer => renderer.GetComponent<Attributes>() == null)
                .Select(renderer => new NormalRenderer { renderer = renderer, material = renderer.material }).ToList();


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

        if (hasInstantiatedData)
        {
            attributeData = Instantiate(attributeDataPrefab);
            attributeData.attributes = this;
            attributeData.flipSystem = GetComponentInChildren<FlipSystem>();
            attributeData.movementSystem = GetComponentInChildren<MovementSystem>();
            attributeData.dashAbility = GetComponentInChildren<DashAbility>();
            attributeData.doubleDashAbility = GetComponentInChildren<DoubleDashAbility>();
            attributeData.chargeShot = GetComponentInChildren<ChargeSystem>();
            attributeData.playerInput = GetComponentInChildren<PlayerInput>();
            attributeData.pickUpSystem = GetComponentInChildren<PickUpSystem>();
            attributeData.playerIdentifier = GetComponentInChildren<PlayerIdentifier>();
        }

        if (hasSharedData)
        {
            SyncEventsWithSharedData();

            rbInfo = new RigidbodyInfo();
            rbInfo.isKinematic = rb.isKinematic;
            rbInfo.Drag = rb.drag;
            rbInfo.Mass = rb.mass;
            rbInfo.AngularDrag = rb.angularDrag;
            rbInfo.constraints = rb.constraints;
            rbInfo.useGravity = rb.useGravity;
        }
    }

    public void ReactivateObject()
    {
        rb.isKinematic = rbInfo.isKinematic;
        rb.drag = rbInfo.Drag;
        rb.angularDrag = rbInfo.AngularDrag;
        rb.constraints = rbInfo.constraints;
        rb.useGravity = rbInfo.useGravity;
        rb.mass = rbInfo.Mass;

        rb.velocity = Vector3.zero;

        foreach (MonoBehaviour component in GetComponents<MonoBehaviour>())
        {
            if (component is Attributes or PlayerInput)
                continue;

            component.enabled = true;
        }

        GetComponent<DoubleDashAbility>().enabled = false;

        objectModel.transform.localRotation = Quaternion.identity;
        objectModel.transform.localPosition = Vector3.zero;

        canReceiveDamage = true;
        StopAllCoroutines();

        foreach (NormalRenderer renderer in renderers)
            renderer.renderer.material = renderer.material;

        if (TryGetComponent(out ItemHolder itemHolder))
            for (var i = 0; i < itemHolder.items.Count; i++)
                itemHolder.items[i].ImmediateReset();
    }

    public void DeathEvent(Vector3 deathPos)
    {
        canDoDamage = false;
        transform.SetParent(null);
        switch (onDeathBehaviour)
        {
            case OnDeathBehaviour.Fly:
                if (hasRigidbody)
                {
                    transform.DOKill();
                    objectModel.DOKill();
                    rb.isKinematic = false;
                    rb.drag = 0.3f;
                    rb.angularDrag = 10f;
                    rb.constraints = RigidbodyConstraints.FreezePositionZ;
                    rb.useGravity = true;
                }


                foreach (MonoBehaviour component in GetComponents<MonoBehaviour>())
                {
                    if (component is Attributes or PlayerInput)
                        continue;

                    component.enabled = false;
                }


                if (!hasSharedData)
                    gameObject.SetActive(false);
                else
                    rb.velocity = Vector3.zero;

                DOVirtual.DelayedCall(0.1f, () =>
                {
                    if (!hasSharedData)
                        gameObject.SetActive(true);

                    rb.AddForce((transform.position - deathPos).normalized * (hasSharedData ? 2f : 10f),
                        ForceMode.VelocityChange);

                    objectModel.transform.localRotation = Quaternion.identity;
                    objectModel.DOLocalRotate(new Vector3(0, 0, 360), .25f, RotateMode.FastBeyond360)
                        .SetLoops(-1, LoopType.Incremental).SetEase(Ease.Linear);
                });

                DOVirtual.DelayedCall(1f, () =>
                {
                    if (hasDeathSound)
                        SoundMaster.PlaySound(transform.position, (int)deathSound);

                    if (hasDeathFX)
                        FXMaster.SpawnFX(transform.position, (int)deathFX);

                    onDeath?.Invoke();
                    transform.DOKill();

                    if (!hasSharedData)
                        gameObject.SetActive(false);
                    else
                        objectModel.transform.DOKill();
                });

                DOVirtual.DelayedCall(1.5f, () =>
                {
                    if (!hasSharedData)
                        Destroy(gameObject);
                });

                break;

            case OnDeathBehaviour.Immediate:
                if (hasDeathSound)
                    SoundMaster.PlaySound(transform.position, (int)deathSound);

                if (hasDeathFX)
                    FXMaster.SpawnFX(transform.position, (int)deathFX);

                onDeath?.Invoke();
                DOVirtual.DelayedCall(.6f, () => Destroy(gameObject));

                gameObject.SetActive(false);
                break;
        }
    }

    public void TakeDamage(Attributes otherAttributes, Vector3 contactPoint = default)
    {
        if ((otherAttributes.team != team || otherAttributes.team == Team.Neutral) && otherAttributes.canDoDamage &&
            canReceiveDamage && damagePriority <= otherAttributes.damagePriority)
        {
            if (onlyExplosions && !otherAttributes.explosive)
                return;

            GetAttributes().HP -= otherAttributes.damage;
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
                Vector3 damageDirection =
                    (transform.position - (contactPoint == default ? otherAttributes.transform.position : contactPoint))
                    .normalized;
                rb.AddForce(damageDirection * (otherAttributes.damage * 20f), ForceMode.Impulse);
            }

            if (GetAttributes().HP <= 0 && GetAttributes().HP > -665)
            {
                //onDeath.Invoke();
                GetAttributes().HP = -666;
                onReceiveHit.Invoke();

                DeathEvent(contactPoint == default ? otherAttributes.transform.position : contactPoint);
            }
            else if (GetAttributes().HP > 0)
            {
                onReceiveHit.Invoke();
            }
        }
    }

    public void Heal(int amount)
    {
        GetAttributes().HP += amount;
        if (GetAttributes().HP > GetAttributes().maxHP)
            GetAttributes().HP = GetAttributes().maxHP;

        onHeal.Invoke();
    }

    public void IncreaseDamagePriority()
    {
        damagePriority++;
    }

    public void DecreaseDamagePriority()
    {
        damagePriority--;
    }

    public void ChangeMaxHP(int amount)
    {
        GetAttributes().maxHP += amount;
        if (GetAttributes().HP > GetAttributes().maxHP)
            GetAttributes().HP = GetAttributes().maxHP;
    }

    private IEnumerator InvulnerabilityCoroutine()
    {
        canReceiveDamage = false;
        foreach (NormalRenderer renderer in renderers) renderer.renderer.material = hurtMaterial;

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
            if (otherAttributes.contactDamage)
                TakeDamage(otherAttributes, collision.contacts[0].point);

        if (canInteract && collision.transform.TryGetComponent(out IGenericInteractable interactable))
        {
            if (hasRigidbody)
                rb.AddForce((transform.position - collision.contacts[0].point).normalized * 15f,
                    ForceMode.VelocityChange);
            interactable.Interact(transform.position);
        }
    }


    /*--- SHARED EVENTS ---*/

    public void DistortView()
    {
        ScreenFXSystem.DistortView(0.3f);
    }


    public void FreezeFrames()
    {
        ScreenFXSystem.FreezeFrames(.3f);
    }


    public void SyncEventsWithSharedData()
    {
        onReceiveHit.AddListener(HitEventSharedData);
        onDeath.AddListener(DeathEventSharedData);
        onHeal.AddListener(HealEventSharedData);
    }

    public void HitEventSharedData()
    {
        sharedData.attributes.onReceiveHit.Invoke();
    }

    public void DeathEventSharedData()
    {
        sharedData.attributes.onDeath.Invoke();
    }

    public void HealEventSharedData()
    {
        sharedData.attributes.onHeal.Invoke();
    }

    public Attributes GetAttributes()
    {
        return hasSharedData ? sharedData.attributes : this;
    }


    private void OnCollisionStay(Collision collision)
    {
        if (!hasSharedData) return;

        if (collision.transform.TryGetComponent(out Attributes otherAttributes))
            if (otherAttributes.contactDamage)
                TakeDamage(otherAttributes, collision.contacts[0].point);
    }
}