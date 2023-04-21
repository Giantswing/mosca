using DG.Tweening;
using SmartData.SmartEvent;
using UnityEngine;

public class PortalScript : MonoBehaviour
{
    public EventDispatcher transitionEvent;
    public bool activated = false;

    private void OnEnable()
    {
        transform.localScale = Vector3.zero;
        transform.DOScale(1.5f, 1f).SetEase(Ease.InQuart);
        transform.DORotate(new Vector3(0, 0, 360), 2f, RotateMode.FastBeyond360).SetEase(Ease.Linear)
            .SetLoops(-1, LoopType.Incremental);
    }

    private void OnDisable()
    {
        DOTween.Kill(transform);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (activated) return;

            activated = true;
            TargetGroupControllerSystem.ChangePlayersEnabled(false);
            foreach (AttributeDataSO playerData in TargetGroupControllerSystem.Instance.playerList)
            {
                playerData.attributes.hardCollider.enabled = false;
                playerData.pickUpSystem.enabled = false;

                playerData.attributes.objectModel.transform.DOShakePosition(5f, .3f);
                playerData.attributes.objectModel.transform
                    .DOLocalRotate(new Vector3(0, 0, 360), .3f, RotateMode.FastBeyond360)
                    .SetLoops(-1, LoopType.Incremental).SetEase(Ease.Linear);

                playerData.attributes.transform.DOMove(transform.position, 1f).SetEase(Ease.InQuart).onComplete += () =>
                {
                    playerData.attributes.transform.DOScale(Vector3.zero, .5f).SetEase(Ease.InQuart).onComplete +=
                        () =>
                        {
                            transitionEvent.Dispatch();
                            LevelTransitionScript.StartTransition();
                        };
                };
            }
        }
    }
}