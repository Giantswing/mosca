using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class Crown : MonoBehaviour, IPressurePlateListener
{
    private Rigidbody myRb;
    private Collider myCol;
    private Transform originalParent;
    private TrailRenderer myTrail;
    [HideInInspector] public Transform pickUpArea;

    [SerializeField] private Transform my3dModel;
    [SerializeField] private MeshRenderer myMeshRenderer;
    [SerializeField] private AudioSource flyingAudioSource;
    [SerializeField] private SimpleAudioEvent flyingAudioEvent;
    private Vector3 originalScale;

    [Space(15)] [SerializeField] private Transform CrownPostionHand;
    [SerializeField] private Transform CrownPostionHead;

    [Space(15)] [SerializeField] private Transform lightTransform;
    [SerializeField] private Light lightSource;

    [Space(15)] public Color glowColor;
    public Color errorColor;


    [Space(15)] public bool isGrabbed = true;
    [Range(0, 1)] [SerializeField] private float CrownPos;

    [Space(15)] [SerializeField] private float airTime = 0;
    [SerializeField] private float airTimeMax = 2.5f;
    [SerializeField] private float strengthMultiplíer = 1f;

    [Range(0, 1)] [SerializeField] private float bounceDampening = 0.5f;

    //[SerializeField] private float returnTimeMax = 1f;
    [SerializeField] private float minDistanceToGrab = 1f;
    [SerializeField] private float returnStrengthMultiplier = 1f;
    [SerializeField] private float returnModeStrengthForce = 5f;
    [SerializeField] private float returnModeStrengthSmoothDamp = 0.1f;


    private float returnStrength;
    private bool isReturning = false;
    private Vector3 currentVelocity;
    private float crownDistance;
    private float crownDistanceTo;

    [Space(25)] [Header("Debugging")] [Range(0, 15)] [SerializeField]
    private float howFastToLockCamera = 1f;

    private bool returnMode = false;

    [Range(0, 0.3f)] [SerializeField] private float howMuchStrengthHasInTarget = 0.05f;

    private static readonly int Emission = Shader.PropertyToID("_EmissionStrength");
    private static readonly int EmissionColor = Shader.PropertyToID("_EmissionColor");

    private Coroutine FlyingSoundCoroutine;
    private WaitForSeconds FlyingSoundWait = new(0.18f);
    private bool EnteredNoCrownArea = false;
    private int numCollisions = 0;


    private void Awake()
    {
        myRb = GetComponent<Rigidbody>();
        myCol = GetComponent<Collider>();
        myTrail = GetComponentInChildren<TrailRenderer>();
        originalParent = transform.parent;
        originalScale = my3dModel.localScale;
    }

    private void Start()
    {
        myCol.enabled = false;
        myRb.useGravity = false;
        myTrail.enabled = false;
        lightSource.intensity = 0;
        TargetGroupControllerSystem.AddTarget(transform, 0, 0, 0.25f);
    }

    public void UpdateMaterial(float strength, Color color)
    {
        myMeshRenderer.material.SetFloat(Emission, strength);
        myMeshRenderer.material.SetColor(EmissionColor, color);
        lightSource.intensity = strength;
        lightSource.color = color;
    }

    private IEnumerator FlyingSound()
    {
        while (true)
        {
            flyingAudioEvent.Play(flyingAudioSource);
            yield return FlyingSoundWait;
        }
    }

    private void Update()
    {
        if (!isGrabbed)
            crownDistance += (crownDistanceTo - crownDistance) * Time.deltaTime * howFastToLockCamera;
        else
            crownDistance += (crownDistanceTo - crownDistance) * Time.deltaTime * 3f;

        var result = crownDistance * howMuchStrengthHasInTarget;
        if (result < 0.01f)
            result = 0;

        TargetGroupControllerSystem.ModifyTargetImmediate(transform, result, 0);

        if (isGrabbed)
        {
            UpdateTransformWithPlayer();
            crownDistanceTo = 0;
        }
        else
        {
            airTime += Time.deltaTime;
            if (airTime > airTimeMax && returnMode == false)
            {
                returnMode = true;
                myCol.enabled = false;
                myRb.drag = 15f;
            }

            if (numCollisions > 3)
            {
                returnMode = true;
                myCol.enabled = false;
                myRb.drag = 15f;
            }

            //Grab();
            crownDistanceTo = Vector3.Distance(transform.position, originalParent.position);

            currentVelocity = myRb.velocity;
            if (myRb.velocity != Vector3.zero)
                transform.rotation = Quaternion.LookRotation(myRb.velocity);

            if (airTime > 0.2f)
                if (crownDistanceTo < minDistanceToGrab)
                    Grab();
            //transform the rotation to look forward in the same direction as the velocity
        }
    }

    private void FixedUpdate()
    {
        if (!isGrabbed)
        {
            returnStrength += Time.deltaTime * returnStrengthMultiplier;
            //myRb.AddForce(Physics.gravity * myRb.mass);


            if (returnMode)
            {
                transform.position =
                    Vector3.SmoothDamp(transform.position, originalParent.position, ref currentVelocity,
                        returnModeStrengthSmoothDamp);

                myRb.AddForce((originalParent.position - transform.position).normalized * returnModeStrengthForce,
                    ForceMode.Impulse);
            }
            else
            {
                myRb.AddForce((originalParent.position - transform.position).normalized * returnStrength,
                    ForceMode.Impulse);
            }
        }

        lightTransform.position = transform.position + Vector3.forward * 0.5f;
    }

    public void ChangeCrownPos(float finalPos)
    {
        DOTween.To(() => CrownPos, x => CrownPos = x, finalPos, 0.15f);
        if (finalPos == 0) my3dModel.DOScale(originalScale + Vector3.one * 0.25f, 0.15f);
    }

    public void Throw(Vector3 dir, float strength)
    {
        if (!isGrabbed) return;


        numCollisions = 0;
        pickUpArea.gameObject.SetActive(true);
        EnteredNoCrownArea = false;
        airTime = 0;
        returnMode = false;
        myRb.drag = 0;
        returnStrength = 0;
        myTrail.enabled = true;
        myTrail.Clear();
        transform.parent = null;
        isGrabbed = false;
        myCol.enabled = true;
        myRb.useGravity = true;
        myRb.velocity = dir * strength * strengthMultiplíer;
        myRb.velocity = new Vector3(myRb.velocity.x, myRb.velocity.y, 0);
        my3dModel.DOScale(originalScale + Vector3.one * 1f, 0.4f);

        SoundMaster.PlaySound(transform.position, (int)SoundListAuto.CrownThrow, "");
        FlyingSoundCoroutine = StartCoroutine(FlyingSound());
        //TargetGroupControllerSystem.ModifyTarget(transform, 0.5f, 0, 0.5f);

        my3dModel.DOLocalRotate(new Vector3(360, 0, 0), 0.4f, RotateMode.FastBeyond360).SetEase(Ease.Linear)
            .SetLoops(-1);

        //DOVirtual.DelayedCall(returnTimeMax, () => my3dModel.DOKill());
    }

    public void SetUpIgnoreCollisions(Collider[] colliders)
    {
        foreach (var col in colliders)
            Physics.IgnoreCollision(col, myCol);
    }

    public void Grab()
    {
        if (isGrabbed) return;

        pickUpArea.gameObject.SetActive(true);
        UpdateMaterial(0, glowColor);
        StopCoroutine(FlyingSoundCoroutine);

        SoundMaster.PlaySound(transform.position, (int)SoundListAuto.CrownReturnHit, "", false);

        isGrabbed = true;
        myCol.enabled = false;
        myRb.useGravity = false;
        myRb.velocity = Vector3.zero;
        myRb.angularVelocity = Vector3.zero;
        airTime = 0;
        CrownPos = 1;

        //TargetGroupControllerSystem.ModifyTarget(transform, 0, 0, 0.8f);
        //TargetGroupControllerSystem.RemoveTarget(transform);

        var duration = Vector3.Distance(transform.position, originalParent.position) * 0.15f;

        my3dModel.DOKill();
        my3dModel.localRotation = Quaternion.identity;
        //my3dModel.DOLocalRotate(new Vector3(360, 0, 0), 1f, RotateMode.FastBeyond360);
        my3dModel.DOScale(originalScale, 0.5f);
        //do small punch rotation
        my3dModel.DOPunchRotation(new Vector3(0, 35f, 0), 0.3f, 1, 0.5f).SetDelay(0.15f);

        transform.DOMove(originalParent.position, duration).SetEase(Ease.OutBounce).onComplete += () =>
        {
            transform.parent = originalParent;
            myTrail.enabled = false;
        };
    }


    private void UpdateTransformWithPlayer()
    {
        transform.position = Vector3.Lerp(CrownPostionHand.position, CrownPostionHead.position, CrownPos);
        transform.rotation = Quaternion.Lerp(CrownPostionHand.rotation, CrownPostionHead.rotation, CrownPos);
    }


    private void OnCollisionEnter(Collision other)
    {
        var normal = other.contacts[0].normal;
        var originalDir = currentVelocity;
        numCollisions++;

        var dir = Vector3.Reflect(originalDir, normal);

        FXMaster.SpawnFX(other.contacts[0].point, (int)FXListAuto.Clash);
        SoundMaster.PlaySound(transform.position, (int)SoundListAuto.CrownHit, "", true);
        Debug.DrawRay(transform.position, dir.normalized, Color.cyan, 3f);
        myRb.velocity = dir * bounceDampening;

        if (other.gameObject.TryGetComponent(out IGenericInteractable interactable))
        {
            interactable.Interact(transform.position);
        }

        else if (other.gameObject.TryGetComponent(out STATS otherStats))
        {
            otherStats.TakeDamage(1, transform.position);

            returnMode = true;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.TryGetComponent(out NoCrownArea noCrownArea))
            if (!EnteredNoCrownArea)
            {
                EnteredNoCrownArea = true;

                FXMaster.SpawnFX(transform.position, (int)FXListAuto.BubbleHit, "");
                SoundMaster.PlaySound(transform.position, (int)SoundListAuto.BubbleHit, "", true);

                transform.DOShakePosition(0.4f, 0.6f, 10, 90, false, true);
                myRb.drag = 100f;
                myCol.enabled = false;

                DOVirtual.DelayedCall(0.4f, () => { returnMode = true; });

                UpdateMaterial(0.5f, errorColor);
                pickUpArea.gameObject.SetActive(false);
            }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, transform.forward);

        var velocity = GetComponent<Rigidbody>().velocity;
        Gizmos.color = Color.green;
        Gizmos.DrawRay(transform.position, velocity);
    }
}