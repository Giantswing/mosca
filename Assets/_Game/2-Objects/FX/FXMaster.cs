using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
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
    Reset,
    HeartContainer
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

    public void CreateFXListEnumDynamically()
    {
        print("regenerating fx enum");
        //create string array with all sound names
        var FxNames = new string[FXList.Count];

        for (var i = 0; i < FXList.Count; i++)
        {
            var fxName = FXList[i].name;
            fxName = fxName.Replace(" ", "");
            FxNames[i] = fxName;
        }

        GenerateEnum.Go("FXListAuto", FxNames);
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

    public static void SpawnFX(Vector3 position, int index = -1, string name = "", Transform parent = null)
    {
        StandardFX effect;
        if (name != "")
            effect = instance.FindFX(name);
        else
            effect = instance.FindFX(index);


        if (effect == null) return;

        if (effect.instances.Count == 0) return;

        var fx = effect.instances.Pop();

        fx.SetActive(true);
        if (parent != null)
        {
            fx.transform.parent = parent;
            fx.transform.localPosition = Vector3.zero;
        }
        else
        {
            fx.transform.position = position;
        }

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
        fx.transform.parent = transform;
        //var effect = FXList.Find(x => x.name == fx.name);
        effect.instances.Push(fx);
    }
}

/* Custom editor for SoundMaster */
[CustomEditor(typeof(FXMaster))]
public class FXMasterEditor : Editor
{
    //add button
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        var myScript = (FXMaster)target;
        if (GUILayout.Button("Generate FX Enum"))
            myScript.CreateFXListEnumDynamically();
    }
}