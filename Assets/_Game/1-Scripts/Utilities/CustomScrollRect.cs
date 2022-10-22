using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CustomScrollRect : MonoBehaviour
{
    private GameObject _selected;
    private Camera _camera;
    private Vector3 _buttonPosition;
    private Vector3 _objectPosition;

    private void Start()
    {
        _camera = Camera.main;
    }

    // Update is called once per frame
    private void Update()
    {
        _selected = EventSystem.current.currentSelectedGameObject;

        if (_selected == null) return;

        _buttonPosition = _selected.transform.InverseTransformPoint(transform.position);
        _objectPosition = new Vector3(_buttonPosition.x, _buttonPosition.y + Screen.height / 2, 0);
        transform.position = Vector3.Lerp(transform.position, _objectPosition, Time.deltaTime * 3);
    }
}