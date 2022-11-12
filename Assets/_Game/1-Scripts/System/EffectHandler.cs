using UnityEngine;
using UnityEngine.Pool;

/* Example call
     EffectHandler.SpawnFX(2, transform.position, Vector3.zero, Vector3.zero, 0);
*/

public class EffectHandler : MonoBehaviour
{
    public static EffectHandler FXHandler;

    public ObjectPool<FXScript> ClashFXPool;
    public FXScript clashFxPrefab;

    public ObjectPool<FXScript> CrateExplosionFXPool;
    public FXScript crateExplosionFxPrefab;

    public ObjectPool<FXScript> DodgeFXPool;
    public FXScript DodgeFxPrefab;

    public ObjectPool<FXScript> CoinPickupFXPool;
    public FXScript CoinPickupFxPrefab;

    public ObjectPool<FXScript> DamageTakenFXPool;
    public FXScript DamageTakenFxPrefab;

    public ObjectPool<FXScript> SmokePuffFXPool;
    public FXScript SmokePuffFxPrefab;


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

        DodgeFXPool = new ObjectPool<FXScript>(
            () => Instantiate(DodgeFxPrefab),
            fx => { fx.gameObject.SetActive(true); },
            fx => { fx.gameObject.SetActive(false); },
            fx => { Destroy(fx); },
            false, 1, 3);
        allFXPools[2] = DodgeFXPool;

        CoinPickupFXPool = new ObjectPool<FXScript>(
            () => Instantiate(CoinPickupFxPrefab),
            fx => { fx.gameObject.SetActive(true); },
            fx => { fx.gameObject.SetActive(false); },
            fx => { Destroy(fx); },
            false, 1, 3);
        allFXPools[3] = CoinPickupFXPool;

        DamageTakenFXPool = new ObjectPool<FXScript>(
            () => Instantiate(DamageTakenFxPrefab),
            fx => { fx.gameObject.SetActive(true); },
            fx => { fx.gameObject.SetActive(false); },
            fx => { Destroy(fx); },
            false, 1, 3);
        allFXPools[4] = DamageTakenFXPool;

        SmokePuffFXPool = new ObjectPool<FXScript>(
            () => Instantiate(SmokePuffFxPrefab),
            fx => { fx.gameObject.SetActive(true); },
            fx => { fx.gameObject.SetActive(false); },
            fx => { Destroy(fx); },
            false, 1, 3);
        allFXPools[5] = SmokePuffFXPool;
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