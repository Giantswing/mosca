using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FXScript : MonoBehaviour
{
    private Action<FXScript, int> _endFX;
    [SerializeField] private float startTimer;
    private ParticleSystem[] _myParticleSystems;
    private int _fxIndex;

    [HideInInspector] public Vector3 moveDir;
    [HideInInspector] public float moveSpeed;
    private Vector3 _myPos;
    private Transform _transform;


    private void Start()
    {
        _myParticleSystems = gameObject.GetComponents<ParticleSystem>();
        _transform = transform;
        _myPos = _transform.position;
    }

    public void Init(Action<FXScript, int> endFX, int fxIndex)
    {
        _endFX = endFX;
        _fxIndex = fxIndex;

        StartCoroutine(EndFX());
    }

    private IEnumerator EndFX()
    {
        yield return new WaitForSeconds(startTimer);
        _endFX(this, _fxIndex);
    }

    private void Update()
    {
        if (moveSpeed != 0)
        {
            _myPos = transform.position + moveDir.normalized * (moveSpeed * Time.deltaTime * 60f);
            _transform.position = _myPos;
        }
    }
}