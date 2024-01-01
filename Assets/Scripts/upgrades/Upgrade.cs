using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;


public class Upgrade
{
    public string Name;
    public Sprite Icon;

    public static double FirerateUpAmt = 0.05;
    public static int 
        AmmoUpAmt = 3, 
        SpeedUpAmt = 1, 
        HpUpAmt = 5, 
        DamageUpAmt = 3;

    public Upgrade(string name, Sprite icon)
    {
        Name = name;
        Icon = icon;
    }

    private const string 
        Firerateup = "Firerate",
        Ammoup = "Ammo",
        Speedup = "SpeedUp",
        Hpup = "HpUp"
                                        ;

    private static List<Upgrade> UpgradeList = new List<Upgrade>()
    {
        { new(Firerateup,Resources.Load<Sprite>("icons/bullet")) },
        { new(Ammoup,Resources.Load<Sprite>("icons/Ammo") )},
        { new(Speedup,Resources.Load<Sprite>("icons/speed") )},
        { new(Hpup,Resources.Load<Sprite>("icons/hp") )}
    };

    public static Stack<Upgrade> GetUpgrades(int amtToGet)
    {

        Stack<Upgrade> arr = new();

        Upgrade upgrade;

        UnityEngine.Random.InitState(System.DateTime.Now.Millisecond + (int)System.DateTime.Now.Ticks + 420);

        for(int i = 0; i < amtToGet; i++)
        {
            //make sure you only add new items
            do
            {
                upgrade = UpgradeList[UnityEngine.Random.Range(0, UpgradeList.Count)];
            } while (arr.Contains(upgrade) );

            arr.Push(upgrade);
        }

        return arr;
    }

    public static void ApplyUpgrade(Player player, Upgrade upgrade)
    {
        switch(upgrade.Name)
        {
            case Firerateup: 
                UpgradeFirerate(player);
                break;
            case Ammoup: 
                UpgradeMaxAmmo(player); 
                break;
            case Speedup: 
                UpgradeSpeed(player);
                break;
            case Hpup: 
                UpgradeHP(player);
                break;
            default: break;

        }
    }

    private static void UpgradeFirerate(Player player)
    {
        player.weaponHandler.cooldown -= FirerateUpAmt;
    }

    private static void UpgradeMaxAmmo(Player player) => player.weaponHandler.MagSizeUp(AmmoUpAmt);
    
    private static void UpgradeSpeed(Player player) => player.speed += SpeedUpAmt;

    private static void UpgradeDamage(Player player) => player.weaponHandler.damage.IncreaseBounsDamage(DamageUpAmt);

    public static void UpgradeHP(Player player)
    {
        player.hp.IncreaseMax(HpUpAmt);
        player.hp.SetCurrentToMax();
    }
}
