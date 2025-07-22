using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirerateUpgrade : Upgrade
{
    private const double RateOfFireUp = 0.05;

    public FirerateUpgrade(Sprite icon, int maxAmount) : base(icon, maxAmount,"Fire rate Upgrade")
    {
    }

    public override void ApplyUpgrade(Player player)
    {
        player.weaponHandler.fireCooldownTime -= (float)RateOfFireUp;
        base.ApplyUpgrade(player);
    }
}
