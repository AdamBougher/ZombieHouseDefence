using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using UnityEngine.Serialization;

public class PlayerWeaponHandler : MonoBehaviour
{
    [FormerlySerializedAs("BulletSpawnLocations")] public List<Transform> bulletSpawnLocations = new();

    private AudioSource _audioSource;
    private static UserInterface Ui => UserInterface.UI;
    private InputActionAsset _actions;

    [FormerlySerializedAs("BulletTrail")] public GameObject bulletTrail;
    public GameObject bulletPrefab;
    
    [FormerlySerializedAs("Fire")] public AudioClip fire;
    [FormerlySerializedAs("Empty")] public AudioClip empty;
    [FormerlySerializedAs("ReloadSFX")] public AudioClip[] reloadSfx;

    private ObjectPool _bulletPool;

    private bool _isReloading;

    public double fireCooldown;

    public Ammo ammo;

    public Damage Damage;

    public int bulletSpeed;

    private bool _canFire = true, hasDisplayed = false;
    private bool HasAmmo => ammo.GetCurrentMag() > 0;
    private bool IsPlaying => _audioSource.isPlaying;

    public int Shots = 1;

    public void Initialize(InputActionAsset actionMap, AudioSource audio)
    {
        _actions = actionMap;
        _actions.FindActionMap("Player").Enable();
        _actions.FindActionMap("Player").FindAction("Fire").performed += OnFire;
        _actions.FindActionMap("Player").FindAction("Reload").performed += OnReload;

        ammo = new Ammo(9,2);
        Damage = new Damage(2);

        _bulletPool = GetComponent<BulletPool>();

        UserInterface.OnLoaded += OnUILoad;

        _audioSource = audio;

    }

    private void OnUILoad()
    {
        UserInterface.UI.UpdateAmmoDisplays(ammo.GetCurrentMag().ToString());
    }

    public void MagSizeUp(int amt)
    {
       ammo.AddToMaxAmmo(amt);
    }

    private void OnFire(InputAction.CallbackContext context)
    {
        if (GameManager.GamePaused || _isReloading) return;
        
        Primary(bulletSpawnLocations[0]);
        Ui.UpdateAmmoDisplays(ammo.ToString());
    }

    public void OnReload(InputAction.CallbackContext context)
    {
        // ReSharper disable once InvertIf
        if (!_isReloading || GameManager.GamePaused)
        {
            _isReloading = true;
            StartCoroutine(Reload());
        }
        
    }

    private void Primary(Transform spawn)
    {
        if(_isReloading) return;
        
        if (HasAmmo && _canFire)
        {
            _canFire = false;
            StartCoroutine(FireGun());


        }else if(!HasAmmo && !_isReloading){
            _isReloading = true;
            StartCoroutine(EmptyReload());
        }

        return;
        
        void Raycast()
        {

            var hit = Physics2D.Raycast(spawn.position, spawn.right);

            if (hit.collider == null)
            {
                hit.point = spawn.position + (transform.right * 10);
            }

            //var trail = Instantiate(bulletTrail, spawn.position, transform.rotation);

           /* transform.gameObject.GetComponent<PlayerWeaponHandler>().
                StartCoroutine(SpawnBullet(trail.GetComponent<TrailRenderer>(), hit));
           */
            StartCoroutine(FireCooldown());
        }


        void Projectile()
        {
            PlaySound(fire);

            if (_bulletPool.GetPooledObject().TryGetComponent<Bullet>(out var bullet))
            {
                bullet.transform.SetPositionAndRotation(spawn.position, spawn.rotation);
                bullet.gameObject.SetActive(true);
            }

            bullet.StartBullet(spawn.right, bulletSpeed,Damage.GetDamage());
            
        }

        IEnumerator FireGun()
        {
            for (var i = 0; i < Shots; i++)
            {
                yield return new WaitWhile(() => GameManager.GamePaused);
                ammo.Use();
                Projectile();
                yield return new WaitWhile(() => GameManager.GamePaused);
                
                yield return new WaitForSeconds((float)fireCooldown*0.45f);
            }
            
            StartCoroutine(FireCooldown());
        }

        IEnumerator EmptyReload()
        {
            if (!hasDisplayed)
            {
                hasDisplayed = false;
                UserInterface.UI.Jumbotron("Out of Ammo! Reloading...");
            }
            
            
            PlaySound(empty);
            yield return new WaitWhile(() => _audioSource.isPlaying);
            
            StartCoroutine(Reload());
        }
    }

    private void PlaySound(AudioClip clip)
    {
        _audioSource.clip = clip;
        _audioSource.Play();
    }

    /* private IEnumerator SpawnBullet(TrailRenderer trail, RaycastHit2D hit2D)
    {
        float time = 0;
        Vector2 startPos = trail.transform.position.WithAxis(Axis.z, 1);

        while (time < 1)
        {
            trail.transform.position = Vector2.Lerp(startPos, hit2D.point, time);
            time += Time.deltaTime / trail.time;

            yield return null;
        }
        //damage target
        if (hit2D.collider != null && hit2D.collider.TryGetComponent<IHittable>(out IHittable target))
        {
            target.Damage(damage.GetDamage());
        }

        Destroy(trail.gameObject, time);
    }
    */
    
    private IEnumerator Reload()
    {
        ammo.SetReload(true);

        PlaySound(reloadSfx[0]);

        yield return new WaitWhile(() => IsPlaying);

        PlaySound(reloadSfx[1]);

        yield return new WaitWhile(() => IsPlaying);

        _isReloading = false;
        
        ammo.Reload();

        Ui.UpdateAmmoDisplays(ammo.ToString());
    }
    private IEnumerator FireCooldown() 
    {
        yield return new WaitForSeconds((float)fireCooldown);
        _canFire = true;
    }

}