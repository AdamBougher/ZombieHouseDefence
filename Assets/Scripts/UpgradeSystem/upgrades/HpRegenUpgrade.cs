using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HpRegenUpgrade : Upgrade
{

    private const int HpRegenUpAmt = 1;

    public HpRegenUpgrade(Sprite icon, int maxAmount) : base(icon, maxAmount, "Hp Regen Upgrade")
    {
    }

    public override void ApplyUpgrade(Player player)
    {
        player.hpRegenAmt += HpRegenUpAmt;
        base.ApplyUpgrade(player);
    }
}
