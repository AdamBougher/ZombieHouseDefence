using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using UnityEngine.Pool;
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

    ObjectPool _bulletpool;

    private bool _isReloading = false;

    public double fireCooldown;

    public Ammo ammo;

    public Damage Damage;

    public int bulletSpeed;

    private bool _canFire = true;
    private bool HasAmmo
    {
        get
        {
            if (ammo.GetCurrentMag() > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
    private bool IsPlaying
    {
        get
        {
            return _audioSource.isPlaying;
        }
    }

    public void Initialize(InputActionAsset actionMap)
    {
        _actions = actionMap;
        _actions.FindActionMap("Player").Enable();
        _actions.FindActionMap("Player").FindAction("Fire").performed += OnFire;
        _actions.FindActionMap("Player").FindAction("Reload").performed += OnReload;

        ammo = new Ammo(9,2);
        Damage = new Damage(2);

        _bulletpool = GetComponent<BulletPool>();

        UserInterface.OnLoaded += OnUILoad;

    }

    private void OnUILoad()
    {
        UserInterface.UI.UpdateAmmoDisplays(ammo.GetCurrentMag().ToString());
    }

    private void OnEnable()
    {
        _audioSource = GetComponentInChildren<AudioSource>();
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
        if (_isReloading) return;
        
        _isReloading = true;
        StartCoroutine(Reload());

    }

    private void Primary(Transform spawn)
    {
        if (HasAmmo && _canFire)
        {
            _canFire = false;

            Projectile();

        }
        else if(!HasAmmo)
        {
            PlaySound(empty);
        }

        #pragma warning disable CS8321 // Local function is declared but never used
        void Raycast()
        {

            RaycastHit2D hit = Physics2D.Raycast(spawn.position, spawn.right);

            if (hit.collider == null)
            {
                hit.point = spawn.position + (transform.right * 10);
            }

            var trail = Instantiate(bulletTrail, spawn.position, transform.rotation);

           /* transform.gameObject.GetComponent<PlayerWeaponHandler>().
                StartCoroutine(SpawnBullet(trail.GetComponent<TrailRenderer>(), hit));
           */
            StartCoroutine(FireCooldown());
        }
        #pragma warning restore CS8321 // Local function is declared but never used

        void Projectile()
        {
            PlaySound(fire);
            ammo.Use();

            if (_bulletpool.GetPooledObject().TryGetComponent<Bullet>(out var bullet))
            {
                bullet.transform.SetPositionAndRotation(spawn.position, spawn.rotation);
                bullet.gameObject.SetActive(true);
            }

            bullet.StartBullet(spawn.right, bulletSpeed,Damage.GetDamage());

            StartCoroutine(FireCooldown());
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