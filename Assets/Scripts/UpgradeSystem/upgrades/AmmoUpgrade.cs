using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoUpgrade : Upgrade
{
    private const int AmmoUpAmt = 3;

    public AmmoUpgrade(Sprite icon, int maxAmount) : base(icon, maxAmount, "Ammo Upgrade")
    {
    }

    public override void ApplyUpgrade(Player player)
    {
        player.weaponHandler.MagSizeUp(AmmoUpAmt);
        base.ApplyUpgrade(player);
    }
}