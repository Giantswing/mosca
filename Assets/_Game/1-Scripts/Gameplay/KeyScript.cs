using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class KeyScript : CollectableBehaviour
{
    [SerializeField] private ParticleSystem myParticles;
    public bool isBeingUsed = false;

    private Vector3 _startPosition;

    private new void Start()
    {
        base.Start();
        _startPosition = transform.position;
    }

    private new void Update()
    {
        base.Update();
    }

    public void Reset()
    {
        if (isFollowing == 0) return;

        Explode();
        transform.position = _startPosition;
        isFollowing = 0;
        _whoToFollow = null;
    }

    public void Explode()
    {
        print("Im exploding!");
        myParticles.Emit(40);
    }
}