using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

public class ScarabWarrior : MonoBehaviour
{
    private enum State
    {
        Idle,
        Follow,
        Throw
    }

    #region External references

    private STATS stats;
    private Rigidbody myRb;
    private float startingZDepth;
    [SerializeField] private GameObject shieldPrefab;
    [SerializeField] private Transform shieldTransform;
    [SerializeField] private Transform handSpear;
    [SerializeField] private Transform shieldHand;
    [SerializeField] private Animator animator;
    [SerializeField] private Transform my3dModel;
    [SerializeField] private GameObject spearPrefab;
    [SerializeField] private CapsuleCollider myCollider;
    private BoxCollider shieldCollider;

    #endregion

    private static readonly int IsThrowing = Animator.StringToHash("isThrowing");
    private static readonly int IsDodging = Animator.StringToHash("isDodging");

    private Stack<Spear> spearPool = new();
    private WaitForSeconds wait_small = new(2f);
    private WaitForSeconds wait_medium = new(3f);
    private WaitForSeconds wait_long = new(5f);
    private Coroutine dodgeCoroutine;
    [Space(30)] [SerializeField] private State state;

    [Space(30)] [SerializeField] private float timeSinceLastAttack = 0;
    [SerializeField] private float attackCooldownBase = 5f;
    [SerializeField] private float followDistance;
    [SerializeField] private float maxFollowDistance;
    [SerializeField] private float goBackDistance;
    [SerializeField] private float throwForce = 5f;
    private float attackCooldown;
    private int LookDir = -1;
    private Vector3 startPos;
    private float distanceToTarget;

    private float rotDirectionTo = 1f;
    private float rotDirection;
    public bool hasShield = true;

    [SerializeField] private float maxRayDistance = 8f;
    public LayerMask ignoreLayerMask;
    private Vector3 dirToGo;
    private float speedToGo;
    private Ray ray;
    private RaycastHit hit;
    private bool canISeePlayer = false;

    private Transform playerTransform;


    private void Awake()
    {
        stats = GetComponent<STATS>();
        myRb = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        state = State.Idle;
        startingZDepth = transform.position.z;
        dodgeCoroutine = StartCoroutine(DodgeCoroutine());
        StopCoroutine(dodgeCoroutine);
        rotDirectionTo = 1f;
        startPos = transform.position;
        my3dModel.DOLocalRotate(Vector3.zero, 0.7f, RotateMode.FastBeyond360);

        InitializeShield();
        InitializeSpears();

        TargetGroupControllerSystem.AddTarget(transform, 0, 0, 0);
    }

    private void InitializeSpears()
    {
        for (var i = 0; i < 3; i++)
        {
            Spear spear = Instantiate(spearPrefab).GetComponent<Spear>();
            spear.myCollider.enabled = false;
            spear.parentCollider = myCollider;
            spear.shieldCollider = shieldCollider;
            spear.zDepthTo = transform.position.z;
            spear.transform.parent = transform;
            spear.myScarabWarrior = this;
            spear.hasCollided = false;

            if (hasShield)
                Physics.IgnoreCollision(shieldCollider, spear.myCollider);

            Physics.IgnoreCollision(myCollider, spear.myCollider);

            spear.gameObject.SetActive(false);
            spearPool.Push(spear);
        }
    }

    private Spear GetSpear()
    {
        Spear resultSpear = spearPool.Pop();
        resultSpear.gameObject.SetActive(true);
        resultSpear.transform.SetParent(null);
        resultSpear.transform.localScale = Vector3.one;
        resultSpear.myRb.isKinematic = false;
        resultSpear.hasCollided = false;
        resultSpear.Initialize();

        return resultSpear;
    }

    public void ReturnSpear(Spear spear)
    {
        spear.myRb.velocity = Vector3.zero;
        spear.transform.SetParent(transform);
        spear.gameObject.SetActive(false);

        spearPool.Push(spear);
    }

    private void InitializeShield()
    {
        hasShield = true;
        shieldTransform = Instantiate(shieldPrefab).transform;
        shieldCollider = shieldTransform.GetComponent<BoxCollider>();
        Physics.IgnoreCollision(shieldTransform.GetComponent<Collider>(), myCollider);

        STATS shieldStats = shieldTransform.GetComponent<STATS>();
        shieldStats.ST_DeathEvent.AddListener(() => { LoseShield(); });
    }

    private void Update()
    {
        playerTransform = TargetGroupControllerSystem.ClosestPlayer(transform);
        rotDirection = Mathf.Lerp(rotDirection, rotDirectionTo, Time.deltaTime * 5f);
        distanceToTarget = Vector3.Distance(transform.position, playerTransform.position);

        if (hasShield)
        {
            shieldTransform.position = shieldHand.position;
            shieldTransform.rotation = shieldHand.rotation;
        }

        switch (state)
        {
            case State.Idle:
                Idle();
                LookAtPlayer(Quaternion.identity, 15f);
                break;
            case State.Follow:
                Follow();
                LookAtPlayer(
                    Quaternion.LookRotation(playerTransform.position - transform.position, Vector3.up),
                    3f);
                break;
            case State.Throw:
                LookAtPlayer(
                    Quaternion.LookRotation(playerTransform.position - transform.position, Vector3.up),
                    15f);
                break;
        }

        if (distanceToTarget > maxFollowDistance + 25f)
            TargetGroupControllerSystem.ModifyTarget(transform, 0, 0);
        else
            TargetGroupControllerSystem.ModifyTarget(transform, 0.5f, 1);
    }


    /* STATES ---------- */


    private void Idle()
    {
        if (distanceToTarget < followDistance)
        {
            state = State.Follow;

            ResetAttack();
            StopCoroutine(dodgeCoroutine);
            dodgeCoroutine = StartCoroutine(DodgeCoroutine());

            my3dModel.DOLocalRotate(new Vector3(0, 90f, 0), 0.7f, RotateMode.FastBeyond360);
        }

        GoBackToStartPos();
    }

    private void Follow()
    {
        timeSinceLastAttack += Time.deltaTime;

        if (distanceToTarget > followDistance)
        {
            StopCoroutine(dodgeCoroutine);
            state = State.Idle;
            my3dModel.DOLocalRotate(Vector3.zero, 0.7f, RotateMode.FastBeyond360);
        }

        dirToGo = (playerTransform.position - transform.position).normalized;
        canISeePlayer = CanISeePlayerInThatDir(dirToGo);

        TryToKeepPlayerCentered();
        CheckIfWeShouldFlip();
        KeepDistanceWithPlayer();

        if (timeSinceLastAttack > attackCooldown) Throw();


        /*
        var dir = (playerReference.playerTransform.position - transform.position).normalized;
        var right = Vector3.Cross(dir, transform.right);

        transform.RotateAround(playerReference.playerTransform.position, right,
            rotSpeed * Time.deltaTime * rotDirection);

        transform.position = new Vector3(transform.position.x, transform.position.y, startingZDepth);
        */
    }

    private void KeepDistanceWithPlayer()
    {
        speedToGo = 0;

        if (canISeePlayer)
        {
            if (distanceToTarget > maxFollowDistance)
                speedToGo = stats.ST_Speed;

            else if (distanceToTarget < goBackDistance) speedToGo = -stats.ST_Speed * .6f;
        }
        else
        {
            speedToGo = stats.ST_Speed * 0.7f;

            if (transform.position.x > playerTransform.position.x)
                dirToGo = -transform.up;
            else
                dirToGo = transform.up;
        }

        Debug.DrawRay(transform.position, dirToGo * 2f, Color.magenta);
        Debug.DrawRay(transform.position, transform.up * 2f, Color.green);

        myRb.AddForce(dirToGo * speedToGo);
    }

    private Vector3 CollisionAvoidance(Vector3 startingDir)
    {
        Vector3 result = startingDir;

        Ray ray = new(transform.position, startingDir);
        RaycastHit hit = new();

        if (Physics.Raycast(ray, out hit, 7f))
        {
            //rotate 90 degrees to the right
            Vector3 dir = Quaternion.AngleAxis(90, transform.up) * startingDir;
            result = dir;
        }

        return result;
    }

    private bool CanISeePlayerInThatDir(Vector3 startingDir)
    {
        Ray ray = new(transform.position, startingDir);
        RaycastHit hit = new();
        var result = false;

        if (Physics.Raycast(ray, out hit, maxRayDistance, ignoreLayerMask))
            if (playerTransform == hit.transform)
                result = true;

        Debug.DrawRay(transform.position, startingDir * maxRayDistance, result ? Color.green : Color.red);
        return result;
    }

    private void CheckIfWeShouldFlip()
    {
        if (transform.position.x > playerTransform.position.x && LookDir == 1)
            Flip(-1);
        else if (transform.position.x < playerTransform.position.x && LookDir == -1)
            Flip(1);
    }

    private void OnDestroy()
    {
        //test
        TargetGroupControllerSystem.RemoveTarget(transform);
    }

    private void TryToKeepPlayerCentered()
    {
        float xdist = transform.position.x - playerTransform.position.x;
        var maxOffset = 2f;
        var maxSpeed = 0.25f;

        if (canISeePlayer)
        {
            if (xdist > 0 && xdist < maxOffset)
                myRb.AddForce(Vector3.right * maxSpeed,
                    ForceMode.Impulse);
            else if (xdist < 0 && xdist > -maxOffset)
                myRb.AddForce(Vector3.left * maxSpeed,
                    ForceMode.Impulse);
        }

        if (transform.position.y < playerTransform.position.y)
            myRb.AddForce(new Vector3(0, 1f, 0) * 0.02f,
                ForceMode.Impulse);
        else if (transform.position.y > playerTransform.position.y)
            myRb.AddForce(new Vector3(0, -1f, 0) * 0.02f,
                ForceMode.Impulse);
    }

    /* METHODS ---------- */

    public void LoseShield()
    {
        hasShield = false;
    }

    private void GoBackToStartPos()
    {
        Vector3 myRbVelocity = myRb.velocity;
        transform.position = Vector3.SmoothDamp(transform.position, startPos, ref myRbVelocity, 2f);
        myRb.velocity = myRbVelocity;
    }

    private void LookAtPlayer(Quaternion lookRot, float lookSpeed = 15f)
    {
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRot, Time.deltaTime * lookSpeed);
    }

    private void ResetAttack()
    {
        float randomChance = UnityEngine.Random.Range(0f, 1f);
        timeSinceLastAttack = 0;
        attackCooldown = attackCooldownBase + UnityEngine.Random.Range(-1f, 1f);

        if (randomChance < 0.3f) attackCooldown = 1f;
    }

    private void Flip(int dir)
    {
        /*
        if (dir == 1)
            my3dModel.DOLocalRotate(new Vector3(0, 90f, 0), 0.7f, RotateMode.FastBeyond360);
        else
            my3dModel.DOLocalRotate(new Vector3(0, 90f, 0), 0.7f, RotateMode.FastBeyond360);
            */

        LookDir = dir;
    }


    private IEnumerator DodgeCoroutine()
    {
        animator.SetBool(IsDodging, true);
        DOVirtual.DelayedCall(1f, () => animator.SetBool(IsDodging, false));

        //choose a random wait from the three available
        float waitTime = UnityEngine.Random.Range(1f, 3f);

        if (waitTime < 1.5f)
            yield return wait_small;
        else if (waitTime < 2.5f)
            yield return wait_medium;
        else
            yield return wait_long;

        rotDirectionTo *= -1;

        StartCoroutine(DodgeCoroutine());
    }

    public void Dodge()
    {
        myRb.AddForce(new Vector3(UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f), 0) * 13f,
            ForceMode.Impulse);
    }

    /*
    private IEnumerator ThrowCoroutine()
    {
        ResetAttack();
        animator.SetBool(IsThrowing, true);
        yield return new WaitForSeconds(0.7f);
        animator.SetBool(IsThrowing, false);


        //spear.GetComponent<Rigidbody>().AddForce(transform.up * 1000f);

        yield return new WaitForSeconds(1f);


        state = State.Follow;
        rotDirectionCoroutine = StartCoroutine(ChangeRotDirection());

        handSpear.gameObject.SetActive(true);
    }
    */

    private void Throw()
    {
        state = State.Throw;
        ResetAttack();
        animator.SetBool(IsThrowing, true);
        DOVirtual.DelayedCall(0.7f, () =>
        {
            animator.SetBool(IsThrowing, false);
            state = State.Follow;
        });

        SoundMaster.PlaySound(transform.position, (int)SoundListAuto.EnemyGrunt, true);
    }

    public void SpearThrow()
    {
        /*
        var spear = Instantiate(spearPrefab, handSpear.transform.position, Quaternion.identity);
        */

        SoundMaster.PlaySound(transform.position, (int)SoundListAuto.BigWhoosh, true);
        Spear spear = GetSpear();

        spear.transform.position = handSpear.transform.position + handSpear.transform.forward * 0.5f;
        spear.transform.rotation =
            Quaternion.LookRotation(playerTransform.position - transform.position);

        handSpear.gameObject.SetActive(false);

        Vector3 dir = (playerTransform.position - handSpear.position).normalized;
        spear.myRb.AddForce(dir * (throwForce * 500f));


        DOVirtual.DelayedCall(.8f, () =>
        {
            handSpear.gameObject.SetActive(true);
            handSpear.transform.localScale = Vector3.zero;
            handSpear.transform.DOScale(Vector3.one, 0.5f);
        });
    }

    public void DestroyShield()
    {
        if (hasShield)
            Destroy(shieldTransform.gameObject);
    }

    public void Shake()
    {
        my3dModel.DOShakePosition(0.5f, 0.5f, 10, 90f, false, true);
        my3dModel.DOShakeRotation(0.5f, 2f, 10, 90f, false);

        my3dModel.DOShakeScale(0.5f, 0.5f, 10, 90f, false);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, followDistance);

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, maxFollowDistance);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, goBackDistance);
    }
}