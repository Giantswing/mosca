using DG.Tweening;
using UnityEngine;

public class SecretWallScriptV2 : MonoBehaviour
{
    private static readonly int DissolveAmount = Shader.PropertyToID("_DissolveAmount");
    [SerializeField] private MeshRenderer[] occluderObj;
    [SerializeField] private SecretWallScriptV2 otherWall;

    [SerializeField] private MeshRenderer meshRenderer;
    [SerializeField] private SimpleAudioEvent foundAudioEvent;
    [SerializeField] private Material dissolveMaterial;
    [SerializeField] private ParticleSystem foundParticles;

    private bool _isDisappearing;

    private void Start()
    {
        if (occluderObj != null)
            foreach (var occluder in occluderObj)
                occluder.enabled = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (_isDisappearing) return;
            Disappear();
            _isDisappearing = true;
        }
    }

    public void Disappear(bool isCalledFromOtherWall = false)
    {
        foundParticles.Emit(30);
        var mats = meshRenderer.materials;
        for (var i = 0; i < meshRenderer.materials.Length; i++)
            mats[i] = dissolveMaterial;

        meshRenderer.materials = mats;

        for (var i = 0; i < meshRenderer.materials.Length; i++)
            meshRenderer.materials[i].DOFloat(1, DissolveAmount, 1f).OnComplete(() => { Destroy(gameObject); });

        //meshRenderer.materials[i].DOFloat(1, DissolveAmount, 2f).OnComplete(() => { Destroy(gameObject); });
        GlobalAudioManager.PlaySound(foundAudioEvent);

        foreach (var occluder in occluderObj) Destroy(occluder.gameObject);

        if (otherWall != null)
            Destroy(otherWall.gameObject);


        /*

        if (!isCalledFromOtherWall)
            if (otherWall != null)
                otherWall.Disappear(true);
                */

        /*

        for (var i = 0; i < meshRenderer.materials.Length; i++)
            meshRenderer.materials[i].DOFloat(1, DissolveAmount, 2f).OnComplete(() => { Destroy(gameObject); });
            */
    }
}