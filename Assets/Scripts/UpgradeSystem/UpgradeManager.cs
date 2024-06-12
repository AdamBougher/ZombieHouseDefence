using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class UpgradeManager : MonoBehaviour
{
    private List<Upgrade> _upgrades;
    private Player _player;
    public static Dictionary<Upgrade, int> CurrentUpgrades = new Dictionary<Upgrade, int>();

    private List<UpgradeChoice> _levelOpOptions;

    private static List<Upgrade> upgradeList;

    private void Start()
    {
        _player = FindObjectOfType<Player>();

        upgradeList = new List<Upgrade>()
        {
            new LuckUpgrade(Resources.Load<Sprite>("icons/luck"), 10),
            new DamageUpgrade(Resources.Load<Sprite>("icons/damage"), 10),
            new FirerateUpgrade(Resources.Load<Sprite>("icons/bullet"), 10),
            new HpUpgrade(Resources.Load<Sprite>("icons/hp"), 10),
            new SpeedUpgrade(Resources.Load<Sprite>("icons/speed"), 10),
            new AmmoUpgrade(Resources.Load<Sprite>("icons/ammo"), 10),
            new HpRegenUpgrade(Resources.Load<Sprite>("icons/hp"), 10),
            new ProjectileSpeedUpgrade(Resources.Load<Sprite>("icons/bullet"), 10),
            new MultishotUpgrade(Resources.Load<Sprite>("icons/bullet"), 10)
        };
    }

    private static Stack<Upgrade> GetUpgrades(int amtToGet)
    {
        var temp = new Stack<Upgrade>();
        var tempUpgradeList = new List<Upgrade>(upgradeList);
        for (var i = 0; i < amtToGet; i++)
        {
            var index = Random.Range(0, tempUpgradeList.Count);
            if (CurrentUpgrades.ContainsKey(tempUpgradeList[index]))
            {
                while (CurrentUpgrades[tempUpgradeList[index]] >= tempUpgradeList[index].MaxAmount)
                {
                    tempUpgradeList.RemoveAt(index);
                }
            }
            temp.Push(tempUpgradeList[index]);
            tempUpgradeList.RemoveAt(index);
            
        }

        return temp;
    }

    public void LevelUpMenuSetup(UserInterface ui)
    {
        _levelOpOptions = ui.levelUpOption;
        //show level up menu
        ui.levelUpMenu.SetActive(true);


        //initialize upgrades
        var temp = GetUpgrades(3);

        Random.InitState(System.DateTime.Now.Millisecond + (int)System.DateTime.Now.Ticks + 420);

        foreach (var obj in _levelOpOptions)
        {
            obj.SetUpgrade(temp.Pop(), _player);
        }

    }
    
}