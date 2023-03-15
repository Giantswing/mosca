using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

public class ButtonScript : MonoBehaviour
{
    [SerializeField] private Transform button;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private SimpleAudioEvent buttonSound;
    private float _buttonStartPos;
    public UnityEvent OnPress;
    [SerializeField] private bool CanBePressedMultipleTimes = false;
    [SerializeField] private bool isPressed = false;
    [SerializeField] private Material cableMaterial;
    [SerializeField] private LineRenderer[] cables;
    [SerializeField] private int cableHangPoints = 10;
    [SerializeField] private float hangStrength = 5f;

    private void Start()
    {
        _buttonStartPos = button.localPosition.y;

        CreateCable();
    }

    private void CreateCable()
    {
        //get how many events are connected to the onpress unity event
        var eventCount = OnPress.GetPersistentEventCount();
        print(eventCount);

        //create as many linerenderer components as there are events and add those components to the object
        cables = new LineRenderer[eventCount];

        //for each cable, create line renderer component and assign it to the cables
        for (var i = 0; i < cables.Length; i++)
        {
            var child = new GameObject();
            child.transform.parent = transform;
            var startingPos = transform.position + new Vector3(0, 0, 1f);
            var targetPos = ((Component)OnPress.GetPersistentTarget(i)).transform.position + new Vector3(0, 0, 1f);
            cables[i] = child.gameObject.AddComponent<LineRenderer>();
            cables[i].positionCount = 2 + cableHangPoints;
            cables[i].SetPosition(0, startingPos);
            cables[i].SetPosition(1 + cableHangPoints, targetPos);

            //set the positions of the hanging points
            for (var j = 0; j < cableHangPoints; j++)
            {
                //get intermediary position between the two points
                var intermediaryPos = Vector3.Lerp(startingPos, targetPos, j / (float)cableHangPoints);

                //add vertical offset to the intermediary position, so that the cable hangs down
                var yOffset =
                    Mathf.Sin(j / (float)cableHangPoints * Mathf.PI) *
                    -hangStrength; // adjust the 0.2f value to change the hang intensity
                intermediaryPos += new Vector3(0, yOffset, 0);


                //set the position of the hanging point
                cables[i].SetPosition(1 + j, intermediaryPos);
            }


            cables[i].startWidth = 0.1f;
            cables[i].endWidth = 0.1f;

            cables[i].material = cableMaterial;
        }
    }

    public void Press()
    {
        if (isPressed) return;
        isPressed = true;
        OnPress.Invoke();
        buttonSound.Play(audioSource);
        button.DOLocalMoveY(_buttonStartPos + 0.3f, 0.25f).SetEase(Ease.InOutQuad).OnComplete(() =>
        {
            if (CanBePressedMultipleTimes)
                button.DOLocalMoveY(_buttonStartPos, 0.65f).SetEase(Ease.InOutQuad).SetEase(Ease.OutBounce)
                    .SetDelay(0.3f).OnComplete(() => { isPressed = false; });
        });
    }
}