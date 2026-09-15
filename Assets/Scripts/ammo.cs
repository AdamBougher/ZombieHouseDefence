
using Sirenix.OdinInspector;

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
