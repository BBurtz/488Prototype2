using System.Collections;
using UnityEngine;

public class Timer : Singleton<Timer>
{
    [SerializeField][ReadOnly] private float currentTime;
    [SerializeField] private float maxTime;
    [SerializeField] private bool timerPaused;
    [SerializeField][ReadOnly] private float tickSpeedDuration;
    [SerializeField][Tooltip("How fast the timer counts, not how fast the game runs")] private float tickSpeed;

    public Coroutine TimerCouroutine;

    void Start()
    {
        currentTime = 0;
        TimerCouroutine = StartCoroutine(TimerGoing());
    }

    public float GetCurrentTime()
    {
        return currentTime;
    }
    public float GetMaxTime()
    {
        return maxTime;
    }
    public bool GetTimerPaused()
    {
        return timerPaused;
    }
    /// <summary>
    /// Get current time/max time on a scale from 0 to 1
    /// </summary>
    /// <returns></returns>
    public float GetNormalizedTime()
    {
        return currentTime / maxTime;
    }
    private IEnumerator TimerGoing()
    {
        while (!timerPaused)
        {
            if (tickSpeedDuration <= 0)
            {
                tickSpeedDuration = 0;
                tickSpeed = 1;
                currentTime += Time.deltaTime * tickSpeed;
            }
            else
            {
                tickSpeedDuration -= Time.deltaTime;
            }
            yield return null;
        }
    }
    public void PauseGameTimeScale()
    {
        Time.timeScale = 0;
    }
    public void UnPauseGameTimeScale()
    {
        Time.timeScale = 1;
    }

    public void PauseGameTimer()
    {
        timerPaused = !timerPaused;
    }

    public void HalveTickSpeedForDuration(float timeToAdd)
    {
        tickSpeed /= 2;
        tickSpeedDuration += timeToAdd;
    }

    /// <summary>
    /// DONT USE THIS. Jk but fr it's just a setter, use HalveTickSpeed for things that
    /// speed up sinking.
    /// </summary>
    /// <param name="ts"></param>
    public void SetTickSpeed(float ts)
    {
        tickSpeed = ts;
    }

    /// <summary>
    /// Close the distance between the game ending and current time
    /// </summary>
    public void RemoveTimeFromGameTimer(float timeToRemove)
    {
        currentTime += timeToRemove;
    }
}
