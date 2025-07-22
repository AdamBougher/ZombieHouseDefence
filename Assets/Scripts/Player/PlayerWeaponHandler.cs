using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class PlayerWeaponHandler : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private List<Transform> bulletSpawnLocations = new();
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Sprite weaponSprite;

    [Header("Audio Clips")]
    [SerializeField] private AudioClip fireClip;
    [SerializeField] private AudioClip emptyClip;
    [SerializeField] private AudioClip[] reloadClips;

    [Header("Stats")]
    [SerializeField] public int magazineSize       = 9;
    [SerializeField] public int spareMagazines     = 2;
    [SerializeField] public int bulletSpeed        = 15;
    [SerializeField] public float fireCooldownTime = 0.5f;
    [SerializeField] private int damageAmount       = 2;

    [Header("Upgrades")]
    [Tooltip("How many bullets to fire per trigger pull")]
    [SerializeField] public int shots = 1;

    private Ammo                ammo;
    public  Damage              damage;
    private AudioSource         audioSrc;
    private ObjectPool<Bullet>  bulletPool;
    private bool                isReloading;
    private bool                canFire = true;

    private UserInterface Ui => UserInterface.UI;

    private void Awake()
    {
        // this will never return null now
        audioSrc    = GetComponent<AudioSource>();

        ammo        = new Ammo(magazineSize, spareMagazines);
        damage      = new Damage(damageAmount);
        bulletPool  = ObjectPool<Bullet>.SharedInstance;
    }

    /// <summary>
    /// Called by input system to fire primary weapon.
    /// </summary>
    public void Primary()
    {
        if (isReloading) return;

        if (ammo.GetCurrentMag() > 0 && canFire)
        {
            Fire();
        }
        else if (ammo.GetCurrentMag() == 0)
        {
            StartReload();
        }
    }

    private void Fire()
    {
        canFire = false;
        PlaySound(fireClip);

        // fire `shots` bullets
        for (int i = 0; i < shots; i++)
        {
            // pick a spawn point or default to [0]
            var spawn = bulletSpawnLocations.Count > 1
                ? bulletSpawnLocations[i % bulletSpawnLocations.Count]
                : bulletSpawnLocations[0];

            if (bulletPool.GetPooledObject().TryGetComponent<Bullet>(out var b))
            {
                // optional simple spread:
                float spreadAngle = (i - (shots-1) * 0.5f) * 5f;
                b.transform.SetPositionAndRotation(
                    spawn.position,
                    spawn.rotation * Quaternion.Euler(0,0, spreadAngle)
                );

                b.gameObject.SetActive(true);
                b.StartBullet(b.transform.right, bulletSpeed, damage.GetDamage());
            }
        }

        // consume one bullet per shot?
        for (int i = 0; i < shots; i++)
            ammo.Use();

        Ui.UpdateAmmoDisplays(ammo.ToString());
        StartCoroutine(FireCooldown());
    }

    private IEnumerator FireCooldown()
    {
        yield return new WaitForSeconds(fireCooldownTime);
        yield return new WaitWhile(() => GameManager.GamePaused);
        canFire = true;
    }

    public void StartReload()
    {
        if (isReloading || ammo.GetTotalAmmo() == 0) return;
        StartCoroutine(ReloadRoutine());
    }

    private IEnumerator ReloadRoutine()
    {
        isReloading = true;

        // play each reload clip in sequence, respecting pause
        foreach (var clip in reloadClips)
        {
            PlaySound(clip);
            yield return new WaitForSeconds(clip.length);
            yield return new WaitWhile(() => GameManager.GamePaused);
        }

        ammo.Reload();
        Ui.UpdateAmmoDisplays(ammo.ToString());
        isReloading = false;
    }

    private void PlaySound(AudioClip clip)
    {
        if (audioSrc != null && clip != null)
            audioSrc.PlayOneShot(clip);
    }

    public void SetArms()
    {
        GetComponent<SpriteRenderer>().sprite = weaponSprite;
    }
}