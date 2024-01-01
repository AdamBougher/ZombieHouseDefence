using UnityEngine;

public class Damage
{
    private bool damageIsRange = false;

    private Vector2Int damageValue;

    private int damageBouns;


    public Damage(int Amt)
    {
        damageValue.x = Amt;
        damageValue.y = -1;
    }

    public Damage(Vector2Int amt)
    {
        damageIsRange = true;
        damageValue = amt;
    }

    public int GetDamage()
    {
        if (!damageIsRange)
        {
            return damageValue.x + damageBouns;
        }
        else
        {
            return (int)Random.Range(damageValue.x, damageValue.y) + damageBouns;
        }
    }

    public void IncreaseBounsDamage(int amt)
    {
        damageBouns += amt;
    }
    public void DecreaseBounsDamage(int amt)
    {
        damageBouns -= amt;
    }
    public void SetBounsDamage(int i)
    { damageBouns = i; }

    public void SetRange(Vector2Int x)
    {
        damageValue = x;
    }

    public void IncreaseRangeEven(int amt)
    {
        damageValue.x += amt;
        damageValue.y += amt;
    }
    public void IncreaseUpperRange(int amt)
    {
        damageValue.y += amt;
    }
    public void IncreaseLowerRange(int amt)
    {
        damageValue.x += amt;
    }

    public void DecreaseRangeEven(int amt)
    {
        damageValue.x -= amt;
        damageValue.y -= amt;
    }
    public void DecreaseUpperRange(int amt)
    {
        damageValue.y -= amt;
    }
    public void DecreaseLowerRange(int amt)
    {
        damageValue.x -= amt;
    }
}
