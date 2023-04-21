using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class AreaGetter : MonoBehaviour
{
    [Serializable]
    public struct AreaGetterTarget
    {
        public Transform Target;
        public Rigidbody TargetRb;
        public bool hasRb;
        public float Distance;
    }

    [SerializeField] private Transform parent;
    [Space(15)] [SerializeField] private AreaGetterTarget closest;
    [Space(25)] [SerializeField] private List<AreaGetterTarget> _areaList = new();

    private Collider _collider;


    private void Awake()
    {
        _collider = GetComponentInChildren<Collider>();

        GetIgnoreCollisions();
    }

    private void Start()
    {
        InvokeRepeating(nameof(UpdateCollision), 0, .15f);
    }

    private void GetIgnoreCollisions()
    {
        foreach (Collider col in Physics.OverlapSphere(parent.transform.position, .35f))
            if (col.isTrigger == false)
                Physics.IgnoreCollision(_collider, col);
    }

    private void Update()
    {
        closest = GetClosestTarget();
    }

    private void UpdateCollision()
    {
        _collider.enabled = false;
        _areaList.Clear();
        DOVirtual.DelayedCall(.01f, () => _collider.enabled = true);
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.isTrigger || _areaList.Contains(new AreaGetterTarget { Target = other.transform }))
            return;

        Rigidbody rb = other.GetComponent<Rigidbody>();


        if (_areaList.Contains(new AreaGetterTarget { Target = other.transform }))
            return;

        if (other.TryGetComponent(out MeshCollider meshCollider))
        {
            if (meshCollider.convex == false)
                Physics.IgnoreCollision(_collider, other);
        }
        else
        {
            _areaList.Add(new AreaGetterTarget
            {
                Target = other.transform,
                Distance = Vector3.Distance(parent.transform.position, other.ClosestPoint(parent.transform.position)),
                TargetRb = rb,
                hasRb = rb != null
            });
        }
    }

    private void OnTriggerExit(Collider other)
    {
        _areaList.RemoveAll(x => x.Target == other.transform);

        if (closest.Target == other.transform)
            closest = GetClosestTarget();
    }

    public int GetAreaListCount()
    {
        return _areaList.Count;
    }

    public bool isClosestRigidbodyWithHardCollider()
    {
        if (closest.hasRb)
            return true;

        return false;
    }

    public AreaGetterTarget GetClosestTarget()
    {
        if (_areaList.Count > 0)
        {
            AreaGetterTarget closest = _areaList[0];

            foreach (AreaGetterTarget target in _areaList)
                if (target.Distance < closest.Distance)
                    closest = target;

            return closest;
        }

        return new AreaGetterTarget();
    }

    public Rigidbody GetClosestRigidbodyWithHardCollider()
    {
        if (_areaList.Count > 0)
        {
            AreaGetterTarget closest = _areaList[0];

            foreach (AreaGetterTarget target in _areaList)
                if (target.Distance < closest.Distance && target.hasRb)
                    closest = target;

            return closest.TargetRb;
        }

        return null;
    }

    public void UpdateAreaSize(float growSpeed)
    {
        parent.transform.localScale = new Vector3(parent.transform.localScale.x + growSpeed,
            parent.transform.localScale.y,
            parent.transform.localScale.z);
    }

    public void SetSpecificAreaSize(float size)
    {
        parent.transform.localScale = new Vector3(size,
            parent.transform.localScale.y,
            parent.transform.localScale.z);
    }

    public void ResetArea()
    {
        _areaList.Clear();
        parent.transform.localScale = new Vector3(0.1f, parent.transform.localScale.y, parent.transform.localScale.z);
    }
}