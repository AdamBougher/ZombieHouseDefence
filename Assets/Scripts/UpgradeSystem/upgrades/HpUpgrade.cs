using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HpUpgrade : Upgrade
{

    private const int HpUpAmt = 5;

    public HpUpgrade(Sprite icon, int maxAmount) : base(icon, maxAmount, "Hp Upgrade")
    {
    }

    public override void ApplyUpgrade(Player player)
    {
        player.Hp.IncreaseMax(HpUpAmt);
        player.Hp.SetCurrentToMax();
        UserInterface.UI.UpdateHp(player.Hp.GetCurrent());
        base.ApplyUpgrade(player);
    }
}