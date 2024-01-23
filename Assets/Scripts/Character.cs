using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;

public abstract class Character : MonoBehaviour , IHittable
{
    [BoxGroup("Character"),]
    public CharacterResource hp;
    [BoxGroup("Character")]
    public float speed;
    [ShowInInspector, ReadOnly, BoxGroup("Character")]
    public static float speedMod = 0;
    [BoxGroup("experance")]
    public int level = 0;
    [BoxGroup("experance"),SerializeField]
    private int experance, nextLevel;


    public float GetSpeed()
    {
        return speed + speedMod;
    }

    public virtual void Damage(int amt)
    {
        hp.DecreaseCurrent(amt);
    }

    public void GainExperance(int amt, CharacterResource hp, GameManager gm)
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


                gm.Pause("Level");
            }
            UserInterface.UI.xpBar.fillAmount = 0;
        }
    }


    public float GetExpPercentage()
    {
        return (float)experance / (float)nextLevel;
    }

}