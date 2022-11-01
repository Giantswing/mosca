using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class KeyScript : CollectableBehaviour
{
    [SerializeField] private ParticleSystem myParticles;
    public bool isBeingUsed = false;

    private Vector3 _startPosition;
    private Transform _parentTransform;
    private bool _hasParent = false;

    private new void Start()
    {
        base.Start();
        _startPosition = transform.position;

        _parentTransform = transform.parent;

        if (_parentTransform != null) _hasParent = true;
    }

    private new void Update()
    {
        base.Update();
    }

    public void Reset()
    {
        if (isFollowing == 0) return;
        Explode();

        if (!_hasParent)
        {
            transform.position = _startPosition;
        }
        else
        {
            transform.SetParent(_parentTransform);
            transform.position = _parentTransform.position;
        }

        isFollowing = 0;
        _whoToFollow = null;
    }

    public void Explode()
    {
        print("Im exploding!");
        myParticles.Emit(40);
    }
}