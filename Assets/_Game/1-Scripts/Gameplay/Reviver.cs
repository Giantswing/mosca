using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class Reviver : MonoBehaviour
{
    public static Reviver instance;
    public static Reviver activeReviver;
    public static List<Reviver> allRevivers = new();
    public bool isActivated = false;
    public bool hasBeenUsed = false;

    public Material offMaterial;
    public Material onMaterial;
    public ParticleSystem spawnParticleSystem;
    public Transform topPart;
    public Transform spawnPoint;
    public Transform respawnPosition;

    public MeshRenderer[] meshRenderers;

    private void Awake()
    {
        allRevivers.Clear();
        instance = this;
    }

    private void Start()
    {
        instance = this;
        spawnParticleSystem.Stop();
        Close();
        Deactivate();
        allRevivers.Add(this);
    }

    public static bool CanRevive()
    {
        var result = false;
        foreach (Reviver reviver in allRevivers)
            if (reviver.isActivated && !reviver.hasBeenUsed)
            {
                result = true;
                break;
            }

        return result;
    }

    private IEnumerator Revive_Coroutine()
    {
        SoundMaster.PlaySound(transform.position, (int)SoundListAuto.ReviverReviving, true);
        TargetGroupControllerSystem.Instance.sharedPlayerData.attributes.HP = TargetGroupControllerSystem.Instance
            .sharedPlayerData.attributes.maxHP;
        TargetGroupControllerSystem.Instance.sharedPlayerData.attributes.onHeal.Invoke();


        foreach (AttributeDataSO playerData in TargetGroupControllerSystem.Instance.playerList)
        {
            FXMaster.SpawnFX(playerData.attributes.transform.position, (int)FXListAuto.SmokePuff);
            SoundMaster.PlaySound(playerData.attributes.transform.position, (int)SoundListAuto.SimplePop);
            playerData.attributes.transform.localScale = Vector3.zero;
            playerData.attributes.ReactivateObject();
            playerData.attributes.hardCollider.enabled = false;
        }

        TargetGroupControllerSystem.ChangePlayersEnabled(false);

        yield return new WaitForSeconds(0.3f);
        activeReviver.spawnParticleSystem.Play();

        foreach (AttributeDataSO playerData in TargetGroupControllerSystem.Instance.playerList)
            playerData.attributes.transform.DOMove(activeReviver.spawnPoint.position, 0.3f).SetEase(Ease.InBack);

        yield return new WaitForSeconds(0.6f);
        activeReviver.Open();
        yield return new WaitForSeconds(0.2f);

        foreach (AttributeDataSO playerData in TargetGroupControllerSystem.Instance.playerList)
            playerData.attributes.transform.DOScale(Vector3.one, 0.95f).SetEase(Ease.OutElastic);

        yield return new WaitForSeconds(0.7f);
        activeReviver.spawnParticleSystem.Stop();

        foreach (AttributeDataSO playerData in TargetGroupControllerSystem.Instance.playerList)
        {
            if (transform.position.x > activeReviver.respawnPosition.position.x)
                playerData.flipSystem.Flip(1);

            playerData.attributes.transform.DOLocalMove(activeReviver.respawnPosition.position, 0.8f)
                    .SetEase(Ease.InBack)
                    .onComplete +=
                () =>
                {
                    TargetGroupControllerSystem.ChangePlayersEnabled(true);
                    playerData.attributes.hardCollider.enabled = true;
                    activeReviver.Close();
                    activeReviver.hasBeenUsed = true;
                    activeReviver.Deactivate();
                    playerData.attributes.ReactivateObject();
                };
        }
    }

    public void Open()
    {
        topPart.transform.localRotation = Quaternion.Euler(-90f, 0, 180f);
        topPart.transform.localPosition = new Vector3(0, 0.38f, 0);
        topPart.transform.DOLocalMoveY(1.8f, 0.5f).SetEase(Ease.OutBack).onComplete += () =>
        {
            topPart.transform.DOLocalRotate(new Vector3(-90f, 0, 0), 0.5f).SetDelay(0.2f).SetEase(Ease.OutBack);
        };
    }

    public void Close()
    {
        topPart.transform.localRotation = Quaternion.Euler(-90f, 0, 0);
        topPart.transform.localPosition = new Vector3(0, 1.8f, 0);

        topPart.transform.DOLocalRotate(new Vector3(-90f, 0, 180f), 0.5f).SetEase(Ease.InBack).onComplete += () =>
        {
            topPart.transform.DOLocalMoveY(0.38f, 0.5f).SetEase(Ease.OutBounce);
        };
    }

    private void DeactivateAllOtherRevivers()
    {
        foreach (Reviver reviver in allRevivers)
            if (reviver != activeReviver)
                reviver.Deactivate();
    }

    public void Activate()
    {
        if (isActivated || hasBeenUsed) return;

        SoundMaster.PlaySound(transform.position, (int)SoundListAuto.ReviverDetecting, true);
        activeReviver = this;
        isActivated = true;
        foreach (MeshRenderer child in meshRenderers) child.material = onMaterial;


        topPart.transform.DOLocalMoveY(0.5f, 0.5f).SetEase(Ease.OutBounce).onComplete += () =>
        {
            topPart.transform.DOLocalMoveY(0.38f, 0.5f).SetEase(Ease.InBack);
        };

        DeactivateAllOtherRevivers();
    }

    public void Deactivate()
    {
        foreach (MeshRenderer child in meshRenderers) child.material = offMaterial;

        isActivated = false;
    }

    public void Revive()
    {
        StartCoroutine(Revive_Coroutine());
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) Activate();
    }
}