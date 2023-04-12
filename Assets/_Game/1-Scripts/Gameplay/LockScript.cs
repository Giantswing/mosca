using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class LockScript : MonoBehaviour
{
    [SerializeField] private Transform gate;
    [SerializeField] private Transform topPart;
    [SerializeField] private bool isOpening = false;
    [SerializeField] private SimpleAudioEvent openSound;
    [SerializeField] private GameObject dustParticles;

    private float _checkDelay = 0.1f;

    public void OpenGate()
    {
        StartCoroutine(DestroyRoutine());
    }

    private IEnumerator DestroyRoutine()
    {
        dustParticles.SetActive(true);
        gate.DOLocalMoveY(6f, 0.5f);
        yield return new WaitForSeconds(1.3f);
        dustParticles.SetActive(false);
        yield return new WaitForSeconds(0.2f);
        Destroy(transform.parent.gameObject);
    }

    private void OnDestroy()
    {
        transform.DOKill();
        gate.DOKill();
    }

    private void Update()
    {
        _checkDelay -= Time.deltaTime;
    }

    private IEnumerator OpenGateRoutine(KeyScript otherKey)
    {
        isOpening = true;
        yield return new WaitForSeconds(0.1f);

        if (otherKey.isFollowing != 3 && otherKey.isBeingUsed) yield return null;

        otherKey.RemoveHolder();
        otherKey.DOKill();
        otherKey.displayObject.DOKill();
        otherKey.displayObject.transform.localRotation = Quaternion.identity;
        otherKey.isFollowing = 4;


        if (transform.position.x > otherKey.transform.position.x)
            otherKey.transform.localScale = new Vector3(-1, 1, 1);

        otherKey.transform.DOMove(transform.position - Vector3.left * 0.35f * otherKey.transform.localScale.x, 0.5f)
            .onComplete += () =>
        {
            SoundMaster.PlaySound(transform.position, (int)SoundListAuto.LockOpening, true);

            topPart.DOLocalRotate(new Vector3(0, 90f, 0), 0.5f);
            transform.DOMoveY(transform.position.y + 2.5f, 0.5f).SetDelay(0.3f);
            otherKey.Exhaust();
            transform.DOScale(0, 0.5f).SetDelay(0.3f).onComplete += () => { OpenGate(); };

            otherKey.transform.DORotate(new Vector3(-90f, 0, 0), 0.5f);

            otherKey.transform.DOScale(Vector3.zero, 0.5f).SetDelay(0.3f).onComplete +=
                () => { Destroy(otherKey.gameObject); };
        };
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_checkDelay < 0 && isOpening == false && other.CompareTag("Collectable"))
        {
            KeyScript otherKey = other.GetComponent<KeyScript>();
            if (otherKey == null) return;
            _checkDelay = 2f;
            StartCoroutine(OpenGateRoutine(otherKey));
        }
    }
}