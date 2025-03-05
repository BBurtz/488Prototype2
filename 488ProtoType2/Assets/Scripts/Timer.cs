using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{
    public int CurrentTime;
    public int MaxTime;
    public Image water;

    public Coroutine TimerCouroutine;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        TimerCouroutine = StartCoroutine(TimerGoing());
    }


    public IEnumerator TimerGoing()
    {
        while (true)
        {
            CurrentTime++;
            yield return new WaitForSeconds(1f);
            water.fillAmount = (float)(CurrentTime) / (float)(MaxTime);
        }
    }

    public IEnumerator PuaseTimer(int Pause)
    {
        StopCoroutine(TimerCouroutine);
        yield return new WaitForSeconds(Pause);
        TimerCouroutine = StartCoroutine(TimerGoing());
    }

    public void UpdateWater()
    {
        water.fillAmount = CurrentTime / MaxTime;
    }
}
