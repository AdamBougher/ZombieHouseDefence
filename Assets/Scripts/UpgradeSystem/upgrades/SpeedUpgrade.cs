using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedUpgrade : Upgrade
{

    private const float SpeedUpAmt = 0.5f;

    public SpeedUpgrade(Sprite icon, int maxAmount) : base(icon, maxAmount, "Speed Upgrade")
    {
    }

    public override void ApplyUpgrade(Player player)
    {
        player.speedMod += SpeedUpAmt;
    }
}
