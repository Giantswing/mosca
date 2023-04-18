using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthSystem : MonoBehaviour
{
    public bool canBeRevived = true;

    public void Die()
    {
        if (Reviver.instance == null || LevelManager.Instance == null || !Reviver.CanRevive() || !canBeRevived)
        {
            LevelLoadSystem.LoadLevel(LevelLoadSystem.LevelToLoad.Restart);
        }
        else
        {
            print("reviver exists, trying to revive");
            Reviver.instance.Revive();
        }
    }
}