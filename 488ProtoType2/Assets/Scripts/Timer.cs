using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{
    //public float CurrentTime;
    //public float MaxTime;
    public Image water;

    public Coroutine TimerCouroutine;
    private WaterRising WR;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        WR = FindFirstObjectByType<WaterRising>();

        if (TimerCouroutine == null)
        {
            TimerCouroutine = StartCoroutine(TimerGoing());
        }
    }


    public IEnumerator TimerGoing()
    {
        while (true)
        {
            water.fillAmount = WR.WaterPercent;
            yield return null;
        }
    }

    public IEnumerator PauseTimer(int Pause)
    {
        StopCoroutine(TimerCouroutine);
        yield return new WaitForSeconds(Pause);
        TimerCouroutine = StartCoroutine(TimerGoing());
    }
/*
    public void UpdateWater()
    {
        water.fillAmount = CurrentTime / MaxTime;
    }
*/
}
