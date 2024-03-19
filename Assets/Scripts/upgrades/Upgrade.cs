using System;
using System.Collections.Generic;
using UnityEngine;

public class Upgrade
{
    public readonly string Name;
    public readonly Sprite Icon;

    private const double RateOdFireUp = 0.05;

    public static int 
        AmmoUpAmt = 3, 
        SpeedUpAmt = 1, 
        HpUpAmt = 5, 
        DamageUpAmt = 3,
        ProjectileSpeedUpAmt = 5,
        HpRegenUpAmt = 1,
        LuckUpAmt = 1;

    private Upgrade(string name, Sprite icon)
    {
        Name = name;
        Icon = icon;
    }

    private const string 
        RateOfFireUp = "Firerate",
        AmmoUp = "Ammo",
        Speedup = "SpeedUp",
        HpUp = "HpUp",
        DamageUp = "DamageUP",
        ProjectileSpeedUp = "ProjectileSpdUp",
        HpRegen = "RegenUp",
        Luck = "LuckUp",
        MultiShotUp = "MultiShotUp";

    private static readonly List<Upgrade> UpgradeList = new()
    {
        { new Upgrade(RateOfFireUp, Resources.Load<Sprite>("icons/bullet")) },
        { new Upgrade(AmmoUp, Resources.Load<Sprite>("icons/Ammo")) },
        { new Upgrade(Speedup, Resources.Load<Sprite>("icons/speed")) },
        { new Upgrade(HpUp, Resources.Load<Sprite>("icons/hp")) },
        { new Upgrade(DamageUp, Resources.Load<Sprite>("icons/damage")) },
        { new Upgrade(ProjectileSpeedUp, Resources.Load<Sprite>("icons/bullet")) },
        { new Upgrade(HpRegen, Resources.Load<Sprite>("icons/hp") )},
        { new Upgrade(Luck, Resources.Load<Sprite>("icons/luck"))},
        { new Upgrade(MultiShotUp, Resources.Load<Sprite>("icons/bullet") )}
    };
    
    public static readonly Dictionary<string, Action<Player>> UpgradeActions = new()
    {
        { RateOfFireUp, (player) => player.weaponHandler.fireCooldown -= RateOdFireUp },
        { AmmoUp, (player) => player.weaponHandler.MagSizeUp(AmmoUpAmt) },
        { Speedup, (player) => player.speed += SpeedUpAmt },
        { HpUp, (player) => 
            {
                player.Hp.IncreaseMax(HpUpAmt);
                player.Hp.SetCurrentToMax();
                UserInterface.UI.UpdateHp(player.Hp.GetCurrent());
            }
        },
        { DamageUp, (player) => player.weaponHandler.Damage.IncreaseBounsDamage(DamageUpAmt) },
        { ProjectileSpeedUp, (player) => player.weaponHandler.bulletSpeed += ProjectileSpeedUpAmt },
        { HpRegen, (player) => player.hpRegenAmt += HpRegenUpAmt },
        { Luck, (player) => player.luck += LuckUpAmt },
        { MultiShotUp, (player) => player.weaponHandler.Shots += 1 }
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

}