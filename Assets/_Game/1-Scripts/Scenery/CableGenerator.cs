using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CableGenerator : MonoBehaviour
{
    private WaitForSeconds _cableDelay = new(0.01f);
    [SerializeField] private Material cableMaterial;
    [SerializeField] private LineRenderer[] cables;
    [SerializeField] private int cableHangPoints = 10;
    [SerializeField] private float hangStrength = 5f;
    public Transform[] targets;
    private float[] _targetDistances;
    private float[] _cableLengths;
    [SerializeField] private Vector3 starting_offset;
    [SerializeField] private Vector3 target_offset;


    public void CreateCable()
    {
        cables = new LineRenderer[targets.Length];
        _targetDistances = new float[targets.Length];
        _cableLengths = new float[targets.Length];

        for (var i = 0; i < targets.Length; i++)
        {
            _targetDistances[i] = Vector3.Distance(transform.position, targets[i].position);
            _cableLengths[i] = _targetDistances[i] + hangStrength;
        }

        //for each cable, create line renderer component and assign it to the cables
        for (var i = 0; i < cables.Length; i++)
        {
            var child = new GameObject();
            child.transform.parent = transform;
            var startingPos = transform.position + starting_offset;
            var targetPos = targets[i].position + target_offset;
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
                    -hangStrength * (_targetDistances[i] / 10);
                intermediaryPos += new Vector3(0, yOffset, 0);


                //set the position of the hanging point
                cables[i].SetPosition(1 + j, intermediaryPos);
            }


            cables[i].startWidth = 0.1f;
            cables[i].endWidth = 0.1f;

            cables[i].material = cableMaterial;
        }

        StartCoroutine(UpdateCablePositions());
    }

    private IEnumerator UpdateCablePositions()
    {
        //update the cable positions
        for (var i = 0; i < cables.Length; i++)
        {
            var startingPos = transform.position + starting_offset;
            var targetPos = targets[i].position + target_offset;
            cables[i].SetPosition(0, startingPos);
            cables[i].SetPosition(1 + cableHangPoints, targetPos);
            var currentDistance = Vector3.Distance(transform.position, targets[i].position);

            for (var j = 0; j < cableHangPoints; j++)
            {
                var intermediaryPos = Vector3.Lerp(startingPos, targetPos, j / (float)cableHangPoints);

                var currentHang = Mathf.Max(_cableLengths[i] - currentDistance, 0);
                var yOffset =
                    Mathf.Sin(j / (float)cableHangPoints * Mathf.PI) * -currentHang;


                intermediaryPos += new Vector3(0, yOffset, 0);
                cables[i].SetPosition(1 + j, intermediaryPos);
            }
        }

        yield return _cableDelay;
        StartCoroutine(UpdateCablePositions());
    }
}