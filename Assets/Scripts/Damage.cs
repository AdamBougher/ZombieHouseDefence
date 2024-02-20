using UnityEngine;

public class Damage
{
    private bool _damageIsRange = false;

    private Vector2Int _damageValue;

    private int _damageBouns;


    public Damage(int amt)
    {
        _damageValue.x = amt;
        _damageValue.y = -1;
    }

    public Damage(Vector2Int amt)
    {
        _damageIsRange = true;
        _damageValue = amt;
    }

    public int GetDamage()
    {
        if (!_damageIsRange)
        {
            return _damageValue.x + _damageBouns;
        }
        else
        {
            return (int)Random.Range(_damageValue.x, _damageValue.y) + _damageBouns;
        }
    }

    public void IncreaseBounsDamage(int amt)
    {
        _damageBouns += amt;
    }
    public void DecreaseBounsDamage(int amt)
    {
        _damageBouns -= amt;
    }
    public void SetBounsDamage(int i)
    { _damageBouns = i; }

    public void SetRange(Vector2Int x)
    {
        _damageValue = x;
    }

    public void IncreaseRangeEven(int amt)
    {
        _damageValue.x += amt;
        _damageValue.y += amt;
    }
    public void IncreaseUpperRange(int amt)
    {
        _damageValue.y += amt;
    }
    public void IncreaseLowerRange(int amt)
    {
        _damageValue.x += amt;
    }

    public void DecreaseRangeEven(int amt)
    {
        _damageValue.x -= amt;
        _damageValue.y -= amt;
    }
    public void DecreaseUpperRange(int amt)
    {
        _damageValue.y -= amt;
    }
    public void DecreaseLowerRange(int amt)
    {
        _damageValue.x -= amt;
    }
}
