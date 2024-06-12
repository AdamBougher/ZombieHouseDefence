using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LuckUpgrade : Upgrade
{

    private const int LuckUpAmt = 1;

    public LuckUpgrade(Sprite icon, int maxAmount) : base(icon, maxAmount, "Luck Upgrade")
    {
    }

    public new void ApplyUpgrade(Player player)
    {
        player.luck += LuckUpAmt;
        base.ApplyUpgrade(player);
    }
}
