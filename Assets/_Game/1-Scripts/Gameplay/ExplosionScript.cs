using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionScript : MonoBehaviour
{
    private WaitForSeconds _explosionDamageDuration;
    private float _explosionShowDuration = 1f;
    [SerializeField] private SphereCollider explosionCollider;
    private float _explosionSize;

    private void Start()
    {
        _explosionSize = explosionCollider.radius;
        CheckForDamage();
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
        }
    }

    // Update is called once per frame
    private void Update()
    {
        _explosionShowDuration -= Time.deltaTime;

        if (_explosionShowDuration <= 0f)
            Destroy(gameObject);
    }
}