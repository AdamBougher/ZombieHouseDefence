using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageUpgrade : Upgrade
{
    public new Sprite Icon { get; private set; }
    public new int MaxAmount { get; }

    private const int DamageUpAmt = 3;

    public DamageUpgrade(Sprite icon, int maxAmount) : base(icon, maxAmount, "Damage Upgrade")
    {
        Icon = icon;
        MaxAmount = maxAmount;
    }

    public override void ApplyUpgrade(Player player)
    {
        player.weaponHandler.Damage.IncreaseBounsDamage(DamageUpAmt);
    }
}
