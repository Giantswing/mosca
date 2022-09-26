using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FPSCounterScript : MonoBehaviour
{
    private TextMeshProUGUI _fpsText;

    private float _updateFPS = .1f;

    // Start is called before the first frame update
    private void Start()
    {
        _fpsText = GetComponent<TextMeshProUGUI>();
    }

    private void Update()
    {
        if (_updateFPS >= 0)
        {
            _updateFPS -= Time.deltaTime;
            if (_updateFPS < 0)
            {
                _fpsText.SetText(Mathf.Round(1f / Time.unscaledDeltaTime).ToString());
                _updateFPS = .1f;
            }
        }
    }
}