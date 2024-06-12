using UnityEngine;

public class ProjectileSpeedUpgrade : Upgrade
{
    private const int ProjectileSpeedUpAmt = 5;

    public ProjectileSpeedUpgrade(Sprite icon, int maxAmount) : base(icon, maxAmount, "Projectile Speed Upgrade")
    {
    }

    public override void ApplyUpgrade(Player player)
    {
        player.weaponHandler.bulletSpeed += ProjectileSpeedUpAmt;
    }
}