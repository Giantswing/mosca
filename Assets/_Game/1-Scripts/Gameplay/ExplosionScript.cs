using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class ExplosionScript : MonoBehaviour
{
    private WaitForSeconds _explosionDamageDuration;
    private float _explosionShowDuration = 1f;
    [SerializeField] private SphereCollider explosionCollider;
    [SerializeField] private MeshRenderer explosionMesh;
    [SerializeField] private MeshRenderer explosionRing;
    private float _explosionSize;
    [SerializeField] private SimpleAudioEvent explosionSound;

    private void Start()
    {
        _explosionSize = explosionCollider.radius;

        explosionRing.transform.localScale = Vector3.zero;
        explosionMesh.transform.localScale = Vector3.zero;
        explosionRing.sharedMaterial.SetFloat("_OpacityMultiplier", 1);
        explosionMesh.sharedMaterial.SetFloat("_ClipThreshold", 0);

        explosionRing.sharedMaterial.DOFloat(0, "_OpacityMultiplier", 0.45f).SetDelay(0.35f);
        explosionRing.transform.DOScale(1.2f, 2.3f).SetEase(Ease.OutBack);

        explosionMesh.transform.DOScale(3f, 0.30f).SetEase(Ease.OutBounce);
        CheckForDamage();

        explosionMesh.sharedMaterial.DOFloat(1f, "_ClipThreshold", 1f).SetEase(Ease.Linear).SetDelay(0.5f)
                .onComplete +=
            () => { Destroy(gameObject); };

        GlobalAudioManager.PlaySound(explosionSound, transform.position);

        /*
        explosionMesh.transform.DOScale(0f, 0.5f).SetEase(Ease.InBack).SetDelay(0.5f).onComplete +=
            () => { Destroy(gameObject); };
        */
    }

    private void CheckForDamage()
    {
        var receivers = Physics.OverlapSphere(transform.position, _explosionSize);
        foreach (var receiver in receivers)
        {
            var otherStats = receiver.GetComponent<STATS>();
            if (otherStats)
            {
                var playerInteractionHandler = receiver.GetComponent<PlayerInteractionHandler>();
                if (playerInteractionHandler)
                    playerInteractionHandler.CheckTakeDamage(1, transform.position);
                else
                    receiver.GetComponent<STATS>().TakeDamage(1, transform.position, true);
            }

            if (receiver.TryGetComponent(out IGenericInteractable interactable))
                interactable.Interact(transform.position);
        }
    }
}