using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using UnityEngine.Serialization;
public class PlayerWeaponHandler : MonoBehaviour
{
    private Player Player => this.transform.parent.GetComponent<Player>();

    public List<Transform> bulletSpawnLocations = new();

    private AudioSource _audioSource;
    private static UserInterface Ui => UserInterface.UI;
    private InputActionAsset _actions;

    [FormerlySerializedAs("BulletTrail")] public GameObject bulletTrail;
    public GameObject bulletPrefab;
    
    [FormerlySerializedAs("Fire")] public AudioClip fire;
    [FormerlySerializedAs("Empty")] public AudioClip empty;
    [FormerlySerializedAs("ReloadSFX")] public AudioClip[] reloadSfx;

    private ObjectPool<Bullet> _bulletPool;

    private bool _isReloading;

    public double fireCooldown;

    public Ammo ammo;

    public Damage Damage;

    public int bulletSpeed;

    private bool _canFire = true;
    private bool HasAmmo => ammo.GetCurrentMag() > 0;
    private bool IsPlaying => _audioSource.isPlaying;

    public int shots = 1;
    
    public void Initialize(InputActionAsset actionMap, AudioSource audio)
    {
        _actions = actionMap;
        _actions.FindActionMap("Player").Enable();
        _actions.FindActionMap("Player").FindAction("Fire").performed += OnFire;
        _actions.FindActionMap("Player").FindAction("Reload").performed += OnReload;

        ammo = new Ammo(9,2);
        Damage = new Damage(2);

        _bulletPool = ObjectPool<Bullet>.SharedInstance;

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
        if (_isReloading) return;

        if (HasAmmo && _canFire)
        {
            _canFire = false;
            Projectile(spawn);
            StartCoroutine(FireCooldown());
        }
        else if (!HasAmmo && !_isReloading)
        {
            _isReloading = true;
            StartCoroutine(Reload());
        }
    }

    private void Raycast(Transform spawn)
    {
        var hit = Physics2D.Raycast(spawn.position, spawn.right);

        if (hit.collider == null)
        {
            hit.point = spawn.position + (transform.right * 10);
        }

        StartCoroutine(FireCooldown());
    }

    private void Projectile(Transform spawn)
    {
        _audioSource.PlaySound(fire);

        if (_bulletPool.GetPooledObject().TryGetComponent<Bullet>(out var bullet))
        {
            bullet.transform.SetPositionAndRotation(spawn.position, spawn.rotation);
            bullet.gameObject.SetActive(true);
        }

        bullet.StartBullet(spawn.right, bulletSpeed, Damage.GetDamage());
    }
    
    private IEnumerator Reload()
    {
        ammo.SetReload(true);

        yield return PlaySoundAndWait(reloadSfx[0]);
        yield return PlaySoundAndWait(reloadSfx[1]);

        _isReloading = false;
        
        ammo.Reload();

        Ui.UpdateAmmoDisplays(ammo.ToString());
    }

    private IEnumerator PlaySoundAndWait(AudioClip clip)
    {
        _audioSource.PlaySound(clip);
        yield return new WaitForSeconds(clip.length);
        
        while (GameManager.GamePaused)
        {
            yield return null;
        }
    }
    
    private IEnumerator FireCooldown() 
    {
        yield return new WaitForSeconds((float)fireCooldown);
        _canFire = true;
    }

}