using System;
using System.Collections;
using UnityEngine;

public class TimerTick : MonoBehaviour
{
    public static Action tickFrame;
    public static Action tickEverySecondFrame;
    public static Action tickEveryThirdFrame;
    public static Action tickEveryQuarterOfSecond;
    public static Action tickEveryHalfOfSecond;
    public static Action tickEverySecond;

    private WaitForSeconds quarterOfSecond = new(0.25f);
    private WaitForSeconds halfOfSecond = new(0.5f);
    private WaitForSeconds oneSecond = new(1f);


    private void Start()
    {
        StartCoroutine(TickEveryQuarterOfSecond());
        StartCoroutine(TickEveryHalfOfSecond());
        StartCoroutine(TickEverySecond());
    }

    private IEnumerator TickEveryQuarterOfSecond()
    {
        tickEveryQuarterOfSecond?.Invoke();
        yield return quarterOfSecond;

        StartCoroutine(TickEveryQuarterOfSecond());
    }

    private IEnumerator TickEveryHalfOfSecond()
    {
        tickEveryHalfOfSecond?.Invoke();
        yield return halfOfSecond;

        StartCoroutine(TickEveryHalfOfSecond());
    }

    private IEnumerator TickEverySecond()
    {
        tickEverySecond?.Invoke();
        yield return oneSecond;

        StartCoroutine(TickEverySecond());
    }

    private void Update()
    {
        tickFrame?.Invoke();

        if (Time.frameCount % 2 == 0)
            tickEverySecondFrame?.Invoke();

        if (Time.frameCount % 3 == 0)
            tickEveryThirdFrame?.Invoke();
    }
}