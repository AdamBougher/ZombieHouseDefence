using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

[System.Serializable]
public abstract class Gun : Weapon
{
    //–– Data from GunData ScriptableObject ––
    public Ammo ammo { get; private set; }
    public int bulletSpawnIndex { get; private set; }
    public GameObject bulletTrailPrefab { get; private set; }
    public float shootDelay { get; private set; }
    public AudioClip fireClip { get; private set; }
    public AudioClip emptyClip { get; private set; }
    public AudioClip[] reloadClips { get; private set; }

    //–– Convenience flags ––
    public bool CanFire   => ammo.GetCurrentMag() > 0;
    public bool CanReload => ammo.GetTotalAmmo() > 0;

    public abstract void Primary(Transform owner, Vector3 spawnPoint, int damageMod);
    public abstract IEnumerator Reload();

    public Gun(GunData data)
    {
        // Ammo setup
        ammo = new Ammo(data.magazineCapacity, data.initialMagazineCount)
        {
            usesAmmo = data.usesAmmo
        };

        // Weapon visuals & timing
        bulletSpawnIndex    = data.bulletSpawnIndex;
        bulletTrailPrefab   = data.bulletTrailPrefab;
        shootDelay          = data.shootDelay;

        // Damage
        if (data.damageIsRandom)
            SetDamage(data.damageRange);
        else
            SetDamage(data.fixedDamage);

        // Audio
        fireClip    = data.fireClip;
        emptyClip   = data.emptyClip;
        reloadClips = data.reloadClips;
    }
}

[System.Serializable]
public class Ammo
{
    [SerializeField] private int magazineSize;
    [SerializeField] private int currentMagazine;
    [SerializeField] private int totalAmmo;

    public bool usesAmmo;

    public Ammo(int magSize, int spareMags)
    {
        magazineSize    = magSize;
        currentMagazine = magSize;
        totalAmmo       = magSize * spareMags;
        usesAmmo        = true;  // Enable ammo tracking by default
    }

    public Ammo() { }

    public void Use()
    {
        if (usesAmmo && currentMagazine > 0)
            currentMagazine--;
    }

    public void Reload()
    {
        if (usesAmmo)
        {
            if (totalAmmo >= magazineSize)
            {
                totalAmmo -= magazineSize;
                totalAmmo += currentMagazine;
                currentMagazine = magazineSize;
            }
            else if (totalAmmo > 0)
            {
                currentMagazine = totalAmmo;
                totalAmmo = 0;
            }
        }
        else
        {
            currentMagazine = magazineSize;
        }
    }

    public int GetCurrentMag() => currentMagazine;
    public int GetTotalAmmo()   => totalAmmo;
    public int GetMagSize()     => magazineSize;
    public void AddAmmo(int amt)       => totalAmmo += amt;
    public void AddToMagSize(int amt)  => magazineSize += amt;

    public override string ToString() =>
        usesAmmo
            ? $"{currentMagazine}/{totalAmmo}"
            : currentMagazine.ToString();
}
