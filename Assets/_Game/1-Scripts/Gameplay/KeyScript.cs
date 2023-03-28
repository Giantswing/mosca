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

    [SerializeField] private SimpleAudioEvent breakSound;

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
    }

    public void Explode()
    {
        isBeingUsed = true;
        getCollider().enabled = false;

        transform.DOShakePosition(0.25f, 1f, 10, 90, false);
        transform.DOShakeScale(0.5f, 5f, 10, 90, false).onComplete += () =>
        {
            EffectHandler.SpawnFX((int)EffectHandler.EffectType.KeyBreak, transform.position, Vector3.zero,
                Vector3.zero,
                0);

            GlobalAudioManager.PlaySound(breakSound, transform.position);

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
            isBeingUsed = false;
            getCollider().enabled = true;
        };
    }

    public void Exhaust()
    {
        myParticles.Emit(40);
    }
}