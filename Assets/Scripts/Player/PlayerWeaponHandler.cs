using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(PlayerArmsManager))]
public class PlayerWeaponHandler : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private List<Transform> bulletSpawnLocations = new();
    [SerializeField] private GameObject bulletPrefab;
    public Sprite weaponSprite; // Made public
    [SerializeField]
    private PlayerArmsManager armsManager
    {
        get
        {
            if (_armsManager == null)
            {
                _armsManager = FindFirstObjectByType<PlayerArmsManager>();
            }
            return _armsManager;
        }
    }
    private PlayerArmsManager _armsManager;

    [Header("Audio Clips")]
    [SerializeField] private AudioClip fireClip;
    [SerializeField] private AudioClip emptyClip;
    [SerializeField] private AudioClip[] reloadClips;

    [Header("Stats")]
    [SerializeField] public int magazineSize = 9;
    [SerializeField] public int spareMagazines = 2;
    [SerializeField] public int bulletSpeed = 15;
    [SerializeField] public float fireCooldownTime = 0.5f;
    [SerializeField] private int damageAmount = 2;

    [Header("Upgrades")]
    [Tooltip("How many bullets to fire per trigger pull")]
    [SerializeField] public int shots = 1;

    private Ammo ammo;
    public Damage damage;
    private AudioSource audioSrc;
    private ObjectPool<Bullet> bulletPool;
    private bool isReloading;
    private bool canFire = true;

    private UserInterface Ui => UserInterface.UI;

    private void Start()
    {
        FindFirstObjectByType<Player>().OnBuildModeChanged += HandleBuildModeChanged;
    }


    private void Awake()
    {
        ammo = new Ammo(magazineSize, spareMagazines);
        damage = new Damage(damageAmount);
        bulletPool = ObjectPool<Bullet>.SharedInstance;

        // Validate injected dependencies
        if (armsManager == null)
        {
            Debug.LogError("PlayerArmsManager is not assigned in PlayerWeaponHandler.");
        }
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

        float[] spreadAngles = CalculateSpreadAngles(shots, 5f);

        foreach (float angle in spreadAngles)
        {
            var spawn = bulletSpawnLocations[0];
            if (bulletPool.GetPooledObject().TryGetComponent<Bullet>(out var b))
            {
                b.transform.SetPositionAndRotation(
                    spawn.position,
                    spawn.rotation * Quaternion.Euler(0, 0, angle)
                );
                b.gameObject.SetActive(true);
                b.StartBullet(b.transform.right, bulletSpeed, damage.GetDamage());
            }
        }

        ammo.Use();
        Ui.UpdateAmmoDisplays(ammo.ToString());
        StartCoroutine(FireCooldown());
    }

    private float[] CalculateSpreadAngles(int shots, float spreadMultiplier)
    {
        float[] angles = new float[shots];
        for (int i = 0; i < shots; i++)
        {
            angles[i] = (i - (shots - 1) * 0.5f) * spreadMultiplier;
        }
        return angles;
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
        if (armsManager == null)
        {
            Debug.LogError("Cannot set arms sprite because PlayerArmsManager is missing.");
            return;
        }

        armsManager.SetArmsSprite(weaponSprite);
    }
    private void HandleBuildModeChanged(bool isBuildMode)
    {
        if (!isBuildMode)
            SetArms();
    }
}