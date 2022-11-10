using System.Collections.Generic;
using UnityEngine;

namespace _Game._1_Scripts.UI
{
    [CreateAssetMenu(fileName = "New Tutorial", menuName = "Flugi/Tutorial")]
    public class TutorialSO : ScriptableObject
    {
        public string tutorialName;

        public List<string> tutorialTexts;
        public List<string> tutorialAndroidTexts;

        public Texture2D[] tutorialImages;

        public bool isTutorialCompleted = false;

        public void SetTutorialCompleted()
        {
            isTutorialCompleted = true;
        }
    }
}