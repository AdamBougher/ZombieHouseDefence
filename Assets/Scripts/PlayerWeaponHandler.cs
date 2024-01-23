using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using UnityEngine.Pool;

public class PlayerWeaponHandler : MonoBehaviour
{
    public List<Transform> BulletSpawnLocations = new();

    private AudioSource audioSource;
    private UserInterface Ui
    {
        get { return UserInterface.UI;  }
    }
    private InputActionAsset actions;

    public GameObject BulletTrail;
    public GameObject bulletPrefab;
    
    public AudioClip Fire, Empty;
    public AudioClip[] ReloadSFX;

    ObjectPool bulletpool;

    public double cooldown;

    public Ammo ammo;

    public Damage damage;

    public int bulletSpeed;

    private bool canFire = true;
    private bool HasAmmo
    {
        get
        {
            if (ammo.getCurrentMag() > 0)
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
            return audioSource.isPlaying;
        }
    }



    public void Initialize(InputActionAsset actionMap)
    {
        actions = actionMap;
        actions.FindActionMap("Player").Enable();
        actions.FindActionMap("Player").FindAction("Fire").performed += OnFire;
        actions.FindActionMap("Player").FindAction("Reload").performed += OnReload;

        ammo = new(9,2);
        damage = new(2);

        bulletpool = GetComponent<BulletPool>();

    }

    private void OnEnable()
    {
        audioSource = GetComponentInChildren<AudioSource>();
    }

    public void MagSizeUp(int amt)
    {
       ammo.addToMaxAmmo(amt);
    }

    private void OnFire(InputAction.CallbackContext context)
    {
        if (!GameManager.GamePaused)
        {
            Primary(BulletSpawnLocations[0]);
            Ui.UpdateAmmoDisplays(ammo.ToString());
        }
    }

    public void OnReload(InputAction.CallbackContext context)
    {
        StartCoroutine(Reload());
    }

    private void Primary(Transform spawn)
    {
        if (HasAmmo && canFire)
        {
            canFire = false;
            PlaySound(Fire);
            ammo.Use();

            projectile();

            canFire = true;

        }
        else if(!HasAmmo)
        {
            PlaySound(Empty);
        }

        #pragma warning disable CS8321 // Local function is declared but never used
        void raycast()
        {

            RaycastHit2D hit = Physics2D.Raycast(spawn.position, spawn.right);

            if (hit.collider == null)
            {
                hit.point = spawn.position + (transform.right * 10);
            }

            var trail = Instantiate(BulletTrail, spawn.position, transform.rotation);

           /* transform.gameObject.GetComponent<PlayerWeaponHandler>().
                StartCoroutine(SpawnBullet(trail.GetComponent<TrailRenderer>(), hit));
           */
            StartCoroutine(FireCooldown());
        }
        #pragma warning restore CS8321 // Local function is declared but never used

        void projectile()
        {

            if (bulletpool.GetPooledObject().TryGetComponent<Bullet>(out var bullet))
            {
                bullet.transform.SetPositionAndRotation(spawn.position, spawn.rotation);
                bullet.gameObject.SetActive(true);
            }

            bullet.StartBullet(spawn.right, bulletSpeed,damage.GetDamage());
        }
    }

    private void PlaySound(AudioClip clip)
    {
        audioSource.clip = clip;
        audioSource.Play();
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
        ammo.setReload(true);

        PlaySound(ReloadSFX[0]);

        yield return new WaitWhile(() => IsPlaying);

        PlaySound(ReloadSFX[1]);

        yield return new WaitWhile(() => IsPlaying);

        ammo.Reload();

        Ui.UpdateAmmoDisplays(ammo.ToString());
    }
    private IEnumerator FireCooldown() 
    {
        yield return new WaitForSeconds((float)cooldown);
        canFire = true;
    }

}