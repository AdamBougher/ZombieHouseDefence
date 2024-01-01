using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

[System.Serializable]
public abstract class Gun : Weapon
{
    public Ammo ammo = new Ammo();
    
    public bool canReload
    {
        get
        {
            if(ammo.getTotalAmmo() > 0)
            {
                return true;
            }else{
                return false;
            }
        }
    }
    public bool canFire
    {
        get{
            if(ammo.getCurrentMag() > 0)
            {
                return true;
            }else{
                return false;
            }
        }
    }

    public GameObject BulletTrail;

    public int bulletSpawnIndex;

    private float ShootDelay = 0.5f;

    public abstract void Primary(Transform transform,Vector3 spawn,int damageMod);
    public abstract IEnumerator Reload();

    public Gun(GunData data)
    {
        //create gun from gun data
        ammo = new Ammo(data.Size,data.StartingMagazines);
        bulletSpawnIndex = data.bulletSpawnIndex;
        BulletTrail = data.BulletTrail;
        ShootDelay = data.ShootDelay;
        if(data.weaponDamageIsRange)
        {
            setDamage(data.DamageRange);
        }else{
            setDamage(data.Damage);
        }
        ammo.usesAmmo = data.usesAmmo;

        setAudioSource(data.audioSource);

        Fire = data.Fire;
        Empty = data.Empty;
        ReloadSFX = data.Reload;
    }
}

[System.Serializable]
public class Ammo
{
    [ShowInInspector]
    private int magazineSize;
    [ShowInInspector]
    private int currentMagazine;
    [ShowInInspector]
    private int TotalAmmo;

    private bool reloading = false;

    public bool usesAmmo;

    public void AddAmmo(int amount) => TotalAmmo += amount;
    
    public Ammo(int magSize, int spareMags)
    {
        magazineSize = magSize;
        currentMagazine = magazineSize;
        TotalAmmo = magazineSize*spareMags;
    }

    public Ammo(){}

    public void Use()
    {
        currentMagazine--; 
    }
    
    /// <summary>
    /// Reloads the Gun
    /// </summary>
    public void Reload()
    {
        if (usesAmmo)
        {
            if (TotalAmmo >= magazineSize)
            {   //as long as there is at least 1 magazine left reload by mag size
                TotalAmmo -= magazineSize;
                TotalAmmo += currentMagazine;
                currentMagazine = magazineSize;

            }
            else if (TotalAmmo > 0)
            {
                //if there is less then one magazine but still more the 0 add the remaining ammo
                currentMagazine = TotalAmmo;
                TotalAmmo = 0;
            }

        }
        else{
            currentMagazine = magazineSize;
        }

   
        
        setReload(false);
    }

    public int getCurrentMag() => currentMagazine;
    public int getTotalAmmo() => TotalAmmo;
    public void addToMaxAmmo(int amt)
    {
        magazineSize += amt;
    }
    public int getMagSize() => magazineSize;
    public void setReload(bool state) => reloading = state;

    public override string ToString()
    {
        if(usesAmmo)
        {
            return string.Format("{0}/{1}", currentMagazine, TotalAmmo);
        }
        else
        {
            return getCurrentMag().ToString();
        }
        
    }
}
