using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;

public class LevelSelectionManager : MonoBehaviour
{
    [SerializeField] private CampaignSO campaign;
    [SerializeField] private GameObject levelButtonPrefab;
    [SerializeField] private RectTransform levelContainer;

    private LevelButton _levelButtonScript;

    [SerializeField] private ScrollRect scrollRect;
    [SerializeField] private CustomScrollRect customScrollRect;

    public List<RectTransform> _buttons;

    private GameObject _selected;
    private Camera _camera;
    private Vector3 _buttonPosition;
    private Vector3 _objectPosition;

    private bool _isAndroid;


    private void Start()
    {
        _camera = Camera.main;

        for (var i = 0; i < campaign.levels.Count; i++)
        {
            var level = campaign.levels[i];
            var levelButton = Instantiate(levelButtonPrefab, levelContainer);
            _levelButtonScript = levelButton.GetComponent<LevelButton>();
            _levelButtonScript.levelIndex = i;
            _levelButtonScript.levelData = level;
            _levelButtonScript.campaignData = campaign;

            _levelButtonScript.UpdateData();

            _buttons.Add(levelButton.GetComponent<RectTransform>());
        }


        if (Application.platform == RuntimePlatform.Android) _isAndroid = true;

        if (_isAndroid) return;
        EventSystem.current.firstSelectedGameObject = _buttons[0].gameObject;
    }

    public void Test()
    {
        print("test");
    }

    // Update is called once per frame
    private void Update()
    {
        _selected = EventSystem.current.currentSelectedGameObject;

        if (_selected == null) return;

        _buttonPosition = _selected.transform.InverseTransformPoint(levelContainer.transform.position);
        _objectPosition = new Vector3(_buttonPosition.x, _buttonPosition.y, 0);

        levelContainer.anchoredPosition =
            Vector3.Lerp(levelContainer.anchoredPosition, _objectPosition, 0.02f);
    }
}