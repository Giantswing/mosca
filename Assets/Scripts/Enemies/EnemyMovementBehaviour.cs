using System;
using DG.Tweening;
using UnityEngine;

public class EnemyMovementBehaviour : MonoBehaviour
{
    private Vector3 _oldPosition, _currentMovement;
    private float _finalRotation, _finalRotationTo;
    [SerializeField] private Transform my3dModel;
    private int _currentDirection = 1;
    private bool _shouldFlip = false;

    private void Start()
    {
        _oldPosition = transform.position;
    }

    private void OnDisable()
    {
        DOTween.Kill(my3dModel);
    }

    private void Update()
    {
        _currentMovement = (transform.position - _oldPosition).normalized;

        _finalRotationTo = _currentMovement.x + _currentMovement.y * .5f;
        _finalRotation += (_finalRotationTo - _finalRotation) * Time.deltaTime * 2f;

        transform.rotation = Quaternion.Euler(0, 0, _finalRotation * -40f);

        if (_currentMovement.x > 0 && _currentDirection == -1)
            _shouldFlip = true;
        else if (_currentMovement.x < 0 && _currentDirection == 1) _shouldFlip = true;

        if (_shouldFlip)
        {
            _currentDirection *= -1;
            my3dModel.DOLocalRotate(new Vector3(0, _currentDirection == 1 ? 0 : 180, 0), 0.5f);
            _shouldFlip = false;
        }

        _oldPosition = transform.position;
    }
}