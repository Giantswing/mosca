using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

public class LevelSelectionManager : MonoBehaviour
{
    [SerializeField] private CampaignSO campaign;
    [SerializeField] private GameObject levelButtonPrefab;
    [SerializeField] private RectTransform levelContainer;

    private LevelButton _levelButtonScript;

    public List<RectTransform> buttons;

    private GameObject _selected;
    private Camera _camera;
    private Vector3 _buttonPosition;
    private Vector3 _objectPosition;

    private bool _isAndroid;


    private void Start()
    {
        SaveLoadSystem.LoadGame();
        _camera = Camera.main;

        for (var i = 0; i < campaign.levels.Count; i++)
        {
            var level = campaign.levels[i];
            var levelButton = Instantiate(levelButtonPrefab, levelContainer);
            _levelButtonScript = levelButton.GetComponent<LevelButton>();
            _levelButtonScript.levelIndex = i;
            _levelButtonScript.levelData = level;
            _levelButtonScript.campaignData = campaign;
            _levelButtonScript.levelSelectionManager = this;
            buttons.Add(levelButton.GetComponent<RectTransform>());
            _levelButtonScript.UpdateData();

            if (campaign.levels[i].bSideScene != null)
            {
                var levelB = campaign.levels[i].bSideScene;
                var levelButtonB = Instantiate(levelButtonPrefab, levelButton.transform);
                _levelButtonScript = levelButtonB.GetComponent<LevelButton>();
                _levelButtonScript.levelIndex = i;
                _levelButtonScript.isBLevel = true;
                _levelButtonScript.levelData = levelB;
                _levelButtonScript.campaignData = campaign;
                _levelButtonScript.levelSelectionManager = this;
                //buttons.Add(levelButtonB.GetComponent<RectTransform>());
                _levelButtonScript.UpdateData();
            }
        }

        if (Application.platform == RuntimePlatform.Android)
        {
            QualitySettings.vSyncCount = 0;
            Application.targetFrameRate = 60;
        }

        if (Application.platform == RuntimePlatform.Android) _isAndroid = true;

        if (_isAndroid) return;
        EventSystem.current.firstSelectedGameObject = buttons[0].gameObject;
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