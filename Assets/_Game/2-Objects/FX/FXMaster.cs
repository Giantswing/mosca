using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum FXTypes
{
    Clash,
    Coin,
    Crate,
    KeyBreak,
    SmokePuff,
    BloodSplat,
    Strike,
    Heal,
    Reset
}

public class FXMaster : MonoBehaviour
{
    [SerializeField] private List<StandardFX> FXList = new();
    private static FXMaster instance;
    private WaitForSeconds[] _fxWaitTimes;

    private void Awake()
    {
        InitializeFX();
        instance = this;
    }

    private void InitializeFX()
    {
        _fxWaitTimes = new WaitForSeconds[FXList.Count];
        for (var j = 0; j < FXList.Count; j++)
        {
            var effect = FXList[j];
            var fxFolder = new GameObject(effect.name);
            fxFolder.transform.parent = transform;
            effect.instances = new Stack<GameObject>();
            _fxWaitTimes[j] = new WaitForSeconds(effect.duration);

            for (var i = 0; i < effect.maxCount; i++)
            {
                var fx = Instantiate(effect.FX, fxFolder.transform);
                effect.instances.Push(fx);
                fx.SetActive(false);
            }
        }
    }

    public static void SpawnFX(Vector3 position, int index = -1, string name = "")
    {
        StandardFX effect;
        if (name != "")
            effect = instance.FindFX(name);
        else
            effect = instance.FindFX(index);


        if (effect == null) return;

        if (effect.instances.Count == 0) return;

        var fx = effect.instances.Pop();
        fx.transform.position = position;
        fx.SetActive(true);
        instance.StartCoroutine(instance.DeactivateFX(effect, fx));
    }

    private StandardFX FindFX(string name)
    {
        return FXList.Find(x => x.name == name);
    }

    private StandardFX FindFX(int index)
    {
        return FXList[index];
    }

    private IEnumerator DeactivateFX(StandardFX effect, GameObject fx)
    {
        yield return _fxWaitTimes[FXList.IndexOf(effect)];
        fx.SetActive(false);
        //var effect = FXList.Find(x => x.name == fx.name);
        effect.instances.Push(fx);
    }
}