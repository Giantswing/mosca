using System;
using TMPro;
using UnityEngine;

namespace UI
{
    public class TimerScript : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI timerText;
        private string _timeMax;
        private float _timeMaxFloat;

        private void OnEnable()
        {
            LevelManager.UpdateTimer += UpdateTimer;
        }

        private void OnDisable()
        {
            LevelManager.UpdateTimer -= UpdateTimer;
        }

        private void Start()
        {
            _timeMaxFloat = LevelManager.LevelData().timeToWin;
            _timeMax = _timeMaxFloat.ToString("F0");
        }

        public void UpdateTimer(float time)
        {
            timerText.text = time.ToString("F0") + "/" + _timeMax;

            if (time > _timeMaxFloat) timerText.color = Color.red;
        }
    }
}