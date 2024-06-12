using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultishotUpgrade : Upgrade
{
    
    public MultishotUpgrade(Sprite icon, int maxAmount) : base(icon, maxAmount, "Multishot Upgrade")
    {
    }

    public override void ApplyUpgrade(Player player)
    {
        player.weaponHandler.Shots += 1;
        base.ApplyUpgrade(player);
    }
}   