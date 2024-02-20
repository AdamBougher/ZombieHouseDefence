using System;
using System.Collections.Generic;
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
        Hpup = "HpUp",
        Damageup = "DamageUP";

    private static readonly List<Upgrade> UpgradeList = new()
    {
        { new(Firerateup,Resources.Load<Sprite>("icons/bullet")) },
        { new(Ammoup,Resources.Load<Sprite>("icons/Ammo") )},
        { new(Speedup,Resources.Load<Sprite>("icons/speed") )},
        { new(Hpup,Resources.Load<Sprite>("icons/hp") )},
        { new(Damageup,Resources.Load<Sprite>("icons/damage") )}
    };

    public static void ApplyUpgrade(Player player, Upgrade upgrade)
    {
        switch (upgrade.Name)
        {
            case Firerateup:
                UpgradeFirerate();
                break;
            case Ammoup:
                UpgradeMaxAmmo();
                break;
            case Speedup:
                UpgradeSpeed();
                break;
            case Hpup:
                UpgradeHp();
                break;
            case Damageup:
                UpgradeDamage();
                break;
            default: break;

        }

        void UpgradeFirerate() => player.weaponHandler.cooldown -= FirerateUpAmt;

        void UpgradeMaxAmmo() => player.weaponHandler.MagSizeUp(AmmoUpAmt);

        void UpgradeSpeed() => player.speed += SpeedUpAmt;

        void UpgradeDamage() => player.weaponHandler.Damage.IncreaseBounsDamage(DamageUpAmt);

        void UpgradeHp()
        {
            player.Hp.IncreaseMax(HpUpAmt);
            player.Hp.SetCurrentToMax();
        }
    }

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

}