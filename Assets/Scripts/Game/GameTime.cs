using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class GameTime
{
    private int Hour { get; set; }
    private int Minute { get; set; } = 0;
    private int Second { get; set; } = 0;

    private const int SecondWorth = 1;
    private int _hourWorth = 1000;
    private bool _isTimeStopped = false;

    public static event EventHandler OnMinuetTick;
    public static event EventHandler OnFifthMinuteTick;

    public IEnumerator Time()
    {
        yield return new WaitForSeconds(1f);

        // Wait while isTimeStopped is true
        yield return new WaitUntil(() => !_isTimeStopped);

        Second++;

        if (Second > 59)
        {
            Second = 0;
            AddMinute();
        }

        // Same as above, but giving code a chance to catch it before the next wait
        yield return new WaitUntil(() => !_isTimeStopped);
        
    }

    private void AddMinute()
    {
        Minute++;
        OnMinuetTick?.Invoke(this, EventArgs.Empty);
        
        if (Minute % 5 == 0)
        {
            OnFifthMinuteTick?.Invoke(this, EventArgs.Empty);
        }
        
        if (Minute < 60) 
            return;
        
        Minute = 0;
        Hour++;
        _hourWorth = Hour * 1000;
    }

    private static string FormatTime(int timeUnit)
    {
        return timeUnit < 10 ? "0" + timeUnit.ToString() : timeUnit.ToString();
    }

    public void ToggleTimeStopped()
    {
        _isTimeStopped = !_isTimeStopped;
        // Note: GameManager.GamePaused is now managed by GameManager.PauseGame()
        // This method only controls time flow; pause logic is centralized in GameManager
    }

    public override string ToString()
    {
        return Hour < 1 ? $"{FormatTime(Minute)}:{FormatTime(Second)}" : $"{Hour}:{FormatTime(Minute)}:{FormatTime(Second)}";
    }
}