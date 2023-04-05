using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class PlayerPickupArea : MonoBehaviour
{
    public Transform player;
    public Transform truePlayer;
    public PlayerInteractionHandler pI;
    public PlayerMovement pM;
    private Transform _transform;

    private void Start()
    {
        _transform = GetComponent<Transform>();
    }

    private void LateUpdate()
    {
        _transform.position = player.position;
    }

    private void OnTriggerEnter(Collider collision)
    {
        //cache collision tag
        var tag = collision.tag;

        switch (tag)
        {
            case "Collectable":
                var collectable = collision.gameObject.GetComponent<CollectableBehaviour>();

                if (collectable != null && collectable.isFollowing == 0)
                    collectable.Collect(player, truePlayer);
                break;
        }
    }
}