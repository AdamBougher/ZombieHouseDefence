using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameTime
{
    private int _hour, _minute = 0, _second = 0;

    private int _secondWorth = 1, _hourWorth = 1000;

    private bool _isTimeStopped = false;

    public delegate void SecondTick();

    public static event SecondTick OnSecTick;

    public IEnumerator Time()
    {
        while (!GameManager.GameOver)
        {
            yield return new WaitForSeconds(1f);

            //waite while isTimeStopped is true
            yield return new WaitUntil(() => _isTimeStopped == false);

            _second++;
            GameManager.AddToScore(_secondWorth);
            OnSecTick?.Invoke();

            if (_second > 59)
            {
                _second = 0;
                AddMinute();
            }
            //same as above, but giving code a chance to catch it before the next waite
            yield return new WaitUntil(() => _isTimeStopped == false);
        }
    }

    private void AddMinute()
    {
        Enemy.LevelUp();
        //Player.GetEXP((Player.Level/2) + Player.Level);
        _minute++;

        if (_minute > 59)
        {
            _minute = 0;
            _hour++;
            GameManager.AddToScore(_hourWorth);
            _hourWorth = _hour * 1000;
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
        _isTimeStopped = !_isTimeStopped;
        GameManager.GamePaused = _isTimeStopped;
    }
    public int GetHour()
    {
        return _hour;
    }
    public int GetMinute()
    {
        return _minute;
    }
    

    public int GetSecond()
    {
        return _second;
    }

    public override string ToString()
    {
        if (_hour < 1)
        {
            return Formatted(_minute) + ":" + Formatted(_second);
        }
        else
        {
            return _hour.ToString() + ":" + Formatted(_minute) + ":" + Formatted(_second);
        }

    }
}
