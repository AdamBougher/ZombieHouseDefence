using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

[System.Serializable]
public abstract class Gun : Weapon
{
    public Ammo ammo = new Ammo();
    
    public bool CanReload
    {
        get
        {
            if(ammo.GetTotalAmmo() > 0)
            {
                return true;
            }else{
                return false;
            }
        }
    }
    public bool CanFire
    {
        get{
            if(ammo.GetCurrentMag() > 0)
            {
                return true;
            }else{
                return false;
            }
        }
    }

    [FormerlySerializedAs("BulletTrail")] public GameObject bulletTrail;

    public int bulletSpawnIndex;

    private float _shootDelay = 0.5f;

    public abstract void Primary(Transform transform,Vector3 spawn,int damageMod);
    public abstract IEnumerator Reload();

    public Gun(GunData data)
    {
        //create gun from gun data
        ammo = new Ammo(data.size,data.startingMagazines);
        bulletSpawnIndex = data.bulletSpawnIndex;
        bulletTrail = data.bulletTrail;
        _shootDelay = data.shootDelay;
        if(data.weaponDamageIsRange)
        {
            SetDamage(data.damageRange);
        }else{
            SetDamage(data.damage);
        }
        ammo.usesAmmo = data.usesAmmo;

        SetAudioSource(data.audioSource);

        Fire = data.fire;
        Empty = data.empty;
        ReloadSfx = data.reload;
    }
}

[System.Serializable]
public class Ammo
{
    [ShowInInspector]
    private int _magazineSize;
    [ShowInInspector]
    private int _currentMagazine;
    [ShowInInspector]
    private int _totalAmmo;

    private bool _reloading = false;

    public bool usesAmmo;

    public void AddAmmo(int amount) => _totalAmmo += amount;
    
    public Ammo(int magSize, int spareMags)
    {
        _magazineSize = magSize;
        _currentMagazine = _magazineSize;
        _totalAmmo = _magazineSize*spareMags;
    }

    public Ammo(){}

    public void Use()
    {
        _currentMagazine--; 
    }
    
    /// <summary>
    /// Reloads the Gun
    /// </summary>
    public void Reload()
    {
        if (usesAmmo)
        {
            if (_totalAmmo >= _magazineSize)
            {   //as long as there is at least 1 magazine left reload by mag size
                _totalAmmo -= _magazineSize;
                _totalAmmo += _currentMagazine;
                _currentMagazine = _magazineSize;

            }
            else if (_totalAmmo > 0)
            {
                //if there is less then one magazine but still more the 0 add the remaining ammo
                _currentMagazine = _totalAmmo;
                _totalAmmo = 0;
            }

        }
        else{
            _currentMagazine = _magazineSize;
        }

   
        
        SetReload(false);
    }

    public int GetCurrentMag() => _currentMagazine;
    public int GetTotalAmmo() => _totalAmmo;
    public void AddToMaxAmmo(int amt)
    {
        _magazineSize += amt;
    }
    public int GetMagSize() => _magazineSize;
    public void SetReload(bool state) => _reloading = state;

    public override string ToString()
    {
        if(usesAmmo)
        {
            return string.Format("{0}/{1}", _currentMagazine, _totalAmmo);
        }
        else
        {
            return GetCurrentMag().ToString();
        }
        
    }
}
