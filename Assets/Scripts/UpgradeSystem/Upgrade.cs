using UnityEngine;

public class Upgrade
{
    public string Name { get; }
    public Sprite Icon { get; }
    public int MaxAmount { get; }
    
    protected Upgrade(Sprite icon, int maxAmount, string name)
    {
        Icon = icon;
        MaxAmount = maxAmount;
        Name = name;
    }

    public virtual void ApplyUpgrade(Player player)
    {
        if (UpgradeManager.CurrentUpgrades.ContainsKey(this))
        {
            UpgradeManager.CurrentUpgrades[this]++;
        }
        UpgradeManager.CurrentUpgrades.TryAdd(this,1);
    }
    
}