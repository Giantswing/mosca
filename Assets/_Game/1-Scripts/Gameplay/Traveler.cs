using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;


[Serializable]
public class TravelPoint
{
    public Vector3 offset;
    public Vector3 rotation;
}


public class Traveler : MonoBehaviour
{
    [SerializeField] private List<Vector3> travelPoints = new();
    [SerializeField] private Traveler exitTraveler;
    [SerializeField] private bool isForward = true;
    [SerializeField] private int currentTravelPoint;
    [SerializeField] private bool canTravel = true;
    [SerializeField] private bool exitsToTheLeft = false;
    private int _endingTravelPoint;
    private Action<bool> _onTravelEnd;
    private Action _onEachPoint;
    [SerializeField] private GameObject travelerEffectPrefab;

    [Space(25)] [SerializeField] private float travelSpeed = 0.5f;
    [Space(25)] [SerializeField] private SimpleAudioEvent travelSound;

    private void Awake()
    {
        currentTravelPoint = 0;
        if (exitTraveler != null)
        {
            exitTraveler.travelPoints = travelPoints;
            exitTraveler.isForward = !isForward;
            exitTraveler.currentTravelPoint = travelPoints.Count - 1;
            exitTraveler.travelSpeed = travelSpeed;
            exitTraveler.exitTraveler = this;
        }

        if (isForward)
            SpawnTravelFX();
    }

    private void SpawnTravelFX()
    {
        for (var i = 1; i < travelPoints.Count - 1; i++)
        {
            GameObject fx = Instantiate(travelerEffectPrefab, transform);
            fx.transform.localPosition = travelPoints[i];
        }
    }

    private void DeactivateTravel()
    {
        canTravel = false;
        exitTraveler.canTravel = false;
    }

    private void ActivateTravel()
    {
        canTravel = true;
        exitTraveler.canTravel = true;
    }

    public void StartTravel(Transform target, Action<bool> onTravelEnd = null, Action onEachPoint = null)
    {
        if (!canTravel) return;

        DeactivateTravel();
        currentTravelPoint = isForward ? 1 : travelPoints.Count - 2;
        _endingTravelPoint = isForward ? travelPoints.Count : -1;
        //print("Start travel: " + currentTravelPoint);
        //print("Ending travel point: " + _endingTravelPoint);
        StartCoroutine(Travel(target));

        _onTravelEnd = onTravelEnd;
        _onEachPoint = onEachPoint;
    }

    private IEnumerator Travel(Transform target)
    {
        yield return null;

        SoundMaster.PlaySound(transform.position, (int)SoundListAuto.FlyDodge, true);

        _onEachPoint?.Invoke();
        int point1 = isForward ? currentTravelPoint - 1 : currentTravelPoint + 1;
        int point2 = currentTravelPoint;


        float distanceToNextPoint =
            Vector3.Distance(travelPoints[point1], travelPoints[point2]);

        float movementDuration = distanceToNextPoint / travelSpeed;
        Vector3 startingPos = isForward ? transform.position : exitTraveler.transform.position;

        ScreenFXSystem.ShakeCamera(.4f, 0.5f);

        target.DOMove(startingPos + travelPoints[currentTravelPoint], movementDuration).SetEase(Ease.Linear)
            .onComplete += () =>
        {
            currentTravelPoint += isForward ? 1 : -1;
            //print("Increased travel point: " + currentTravelPoint);

            if (currentTravelPoint == _endingTravelPoint)
                StartCoroutine(CooldownTravel());
            else
                StartCoroutine(Travel(target));
        };
    }


    private IEnumerator CooldownTravel()
    {
        _onTravelEnd?.Invoke(exitTraveler.exitsToTheLeft);
        yield return new WaitForSeconds(.1f);
        ActivateTravel();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out PlayerIdentifier otherPlayer))
        {
            if (!canTravel) return;

            Transform model = otherPlayer.attributes.objectModel.transform;
            otherPlayer.DisableMovement();
            otherPlayer.attributes.canReceiveDamage = false;
            otherPlayer.attributes.hardCollider.enabled = false;

            model.DOLocalRotate(new Vector3(0, 0, -360), 0.3f, RotateMode.FastBeyond360)
                .SetLoops(-1, LoopType.Incremental).SetEase(Ease.Linear);


            StartTravel(otherPlayer.transform, (exitsToTheLeft) =>
            {
                model.DOKill();
                model.DOLocalRotate(new Vector3(0, exitsToTheLeft ? 180 : 0, 0),
                    .35f, RotateMode.FastBeyond360);

                otherPlayer.flipSystem.Flip(exitsToTheLeft ? -1 : 1);
                otherPlayer.attributes.canReceiveDamage = true;
                otherPlayer.attributes.hardCollider.enabled = true;
                otherPlayer.EnableMovement();
            });
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawSphere(transform.position, .6f);

        Gizmos.color = Color.magenta;


        for (var i = 0; i < travelPoints.Count; i++)
        {
            Vector3 point = transform.position + travelPoints[i];
            //var point2 = point + Quaternion.Euler(MovePoints[i].rotation) * transform.up * 0.5f;

            Gizmos.DrawSphere(point, .2f);

            //Gizmos.DrawLine(point, point2);
        }
    }
}