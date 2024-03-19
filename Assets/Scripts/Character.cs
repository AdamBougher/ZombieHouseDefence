using System;
using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.InputSystem;

public abstract class Character : MonoBehaviour , IHittable
{
    [BoxGroup("Character"),]
    public CharacterResource Hp;
    [BoxGroup("Character")]
    public float speed;
    [ShowInInspector, ReadOnly, BoxGroup("Character")]
    public static float SpeedMod = 0;
    [BoxGroup("experance")]
    public int level = 0;
    [BoxGroup("experance"),SerializeField]
    private int experance, nextLevel;


    protected float GetSpeed()
    {
        return speed + SpeedMod;
    }

    public virtual void Damage(int amt)
    {
        Hp.DecreaseCurrent(amt);
    }

    protected void GainExperance(int amt, CharacterResource hp, GameManager gm)
    {

        experance += amt;
        UserInterface.UI.xpBar.fillAmount = GetExpPercentage();

        if(experance >= nextLevel)
        {
            hp.SetCurrentToMax();
            while (experance >= nextLevel)
            {
                level++;
                experance -= nextLevel;
                nextLevel += nextLevel / 3;

                UserInterface.UI.Updatelevel(level.ToString());


                gm.PauseGame("Level");
            }
            UserInterface.UI.xpBar.fillAmount = 0;
        }
    }


    private float GetExpPercentage()
    {
        return (float)experance / (float)nextLevel;
    }

}