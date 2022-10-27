using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurrentLevelHolder : MonoBehaviour
{
    [SerializeField] private LevelSO currentLevel;
    public static CurrentLevelHolder Instance;


    private void Awake()
    {
        Instance = this;
    }

    public static LevelSO GetCurrentLevel()
    {
        return Instance.currentLevel;
    }
}