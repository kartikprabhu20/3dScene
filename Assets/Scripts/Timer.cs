//Reference written by Daniel Keele - 6/6/2019
using System;
using UnityEngine;

public class Timer
{
    private float currentTime = 0f;
    public bool isRunning = false;

    public void Update()
    {
        if (isRunning)
        {
            currentTime = currentTime + Time.deltaTime;
        }
    }

    public void startTimer()
    {
        if (!isRunning)
        {
            currentTime = 0f;
            isRunning = true;
        }
    }

    public float stopTimer()
    {
        if (isRunning)
        {
            isRunning = false;
        }
        return currentTime;
    }

    public string GetCurrentTime()
    {
        TimeSpan time = TimeSpan.FromSeconds(currentTime);
        return time.ToString(@"\mm\:ss\:fff");
    }

    public int GetMinutes()
    {
        return (int)(currentTime / 60f);
    }

    public int GetSeconds()
    {
        return (int)(currentTime);
    }

    public float GetMilliseconds()
    {
        return (float)(currentTime - System.Math.Truncate(currentTime));
    }
}