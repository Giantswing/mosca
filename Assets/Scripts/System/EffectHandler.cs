using UnityEngine;
using UnityEngine.Pool;

public class EffectHandler : MonoBehaviour
{
    public static EffectHandler FXHandler;

    public ObjectPool<FXScript> ClashFXPool;
    public FXScript clashFxPrefab;

    public ObjectPool<FXScript> CrateExplosionFXPool;
    public FXScript crateExplosionFxPrefab;

    private ObjectPool<FXScript>[] allFXPools;

    public enum EffectType
    {
        Clash,
        CrateExplosion
    }

    private void Start()
    {
        FXHandler = this;

        allFXPools = new ObjectPool<FXScript>[10];

        ClashFXPool = new ObjectPool<FXScript>(
            () => Instantiate(clashFxPrefab),
            fx => { fx.gameObject.SetActive(true); },
            fx => { fx.gameObject.SetActive(false); },
            fx => { Destroy(fx); },
            false, 3, 10);
        allFXPools[0] = ClashFXPool;

        CrateExplosionFXPool = new ObjectPool<FXScript>(
            () => Instantiate(crateExplosionFxPrefab),
            fx => { fx.gameObject.SetActive(true); },
            fx => { fx.gameObject.SetActive(false); },
            fx => { Destroy(fx); },
            false, 3, 10);
        allFXPools[1] = CrateExplosionFXPool;
    }

    public static void SpawnFX(int fxIndex, Vector3 spawnPos, Vector3 spawnRot, Vector3 moveDir, float moveSpeed)
    {
        var currentFX = FXHandler.allFXPools[fxIndex].Get();
        currentFX.Init(FXHandler.EndFX, fxIndex);
        currentFX.transform.position = spawnPos;
        currentFX.transform.rotation = spawnPos == Vector3.zero ? Quaternion.identity : Quaternion.Euler(spawnRot);
        currentFX.moveDir = moveDir;
        currentFX.moveSpeed = moveSpeed;
    }


    private void EndFX(FXScript fx, int fxIndex)
    {
        allFXPools[fxIndex].Release(fx);
    }
}