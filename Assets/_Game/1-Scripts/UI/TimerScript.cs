using System;
using TMPro;
using UnityEngine;

namespace UI
{
    public class TimerScript : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI timerText;

        [SerializeField] private SmartData.SmartInt.IntReader levelTimerInt;
        [SerializeField] private SmartData.SmartFloat.FloatReader levelTimerMax;


        public void UpdateTimer()
        {
            timerText.text = levelTimerInt.value + "/" + levelTimerMax.value;
            timerText.color = levelTimerInt.value > levelTimerMax.value ? Color.red : Color.white;
        }
    }
}