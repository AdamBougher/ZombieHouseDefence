using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameTime
{
    private int hour, minute = 0, second = 0;

    private int secondWorth = 1, hourWorth = 1000;

    private bool isTimeStopped = false;

    public static UnityEvent minuteEvent;

    public IEnumerator Time()
    {
        while (!GameManager.GameOver)
        {
            yield return new WaitForSeconds(1f);

            //waite while isTimeStopped is true
            yield return new WaitUntil(() => isTimeStopped == false);

            second++;
            GameManager.AddToScore(secondWorth);

            if (second > 59)
            {
                second = 0;
                AddMinute();
            }
            //same as above, but giving code a chance to catch it before the next waite
            yield return new WaitUntil(() => isTimeStopped == false);
        }
    }

    private void AddMinute()
    {
        Enemy.LevelUp();
        //Player.GetEXP((Player.Level/2) + Player.Level);
        minute++;

        if (minute > 59)
        {
            minute = 0;
            hour++;
            GameManager.AddToScore(hourWorth);
            hourWorth = hour * 1000;
        }
    }

    private string Formatted(int x)
    {
        if (x < 10)
        {
            return "0" + x.ToString();
        }
        else
        {
            return x.ToString();
        }
    }

    public void ToggleTimeStopped()
    {
        isTimeStopped = !isTimeStopped;
        GameManager.GamePaused = isTimeStopped;
    }
    public int GetHour()
    {
        return hour;
    }
    public int GetMinute()
    {
        return minute;
    }
    

    public int getSecond()
    {
        return second;
    }

    public override string ToString()
    {
        if (hour < 1)
        {
            return Formatted(minute) + ":" + Formatted(second);
        }
        else
        {
            return hour.ToString() + ":" + Formatted(minute) + ":" + Formatted(second);
        }

    }
}
