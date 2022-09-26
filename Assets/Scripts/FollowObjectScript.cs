using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowObjectScript : MonoBehaviour
{
    [SerializeField] private Transform _whoToFollow;

    // Start is called before the first frame update
    private void Start()
    {
        if (_whoToFollow == null)
            _whoToFollow = GameObject.FindGameObjectsWithTag("Player")[0].transform;

        transform.position = _whoToFollow.position;
    }

    // Update is called once per frame
    private void Update()
    {
        transform.position = Vector3.Lerp(transform.position, _whoToFollow.position, .65f);
    }
}