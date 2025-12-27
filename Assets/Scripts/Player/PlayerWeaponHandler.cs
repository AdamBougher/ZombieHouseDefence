using UnityEngine;
using ZombieHouseDefense.core;

[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(PlayerArmsManager))]
public class PlayerWeaponHandler : MonoBehaviour
{
    [SerializeField] private Transform firePoint;
    [SerializeField] private AudioClip fireClip;

    private BulletPool bulletPool;

    private void Awake()
    {
        bulletPool = BulletPool.Instance;
    }
    private void OnFire()
    {
        // Ensure pool reference
        if (bulletPool == null)
        {
            bulletPool = BulletPool.Instance;
        }

        if (bulletPool == null)
        {
            Debug.LogError("PlayerWeaponHandler: BulletPool instance is missing in the scene.");
            return;
        }

        var spawnPos = firePoint ? firePoint.position : transform.position;
        var spawnRot = firePoint ? firePoint.rotation : transform.rotation;

        var bullet = bulletPool.GetBullet(null, spawnPos, spawnRot);
        if (bullet == null)
        {
            Debug.LogError("PlayerWeaponHandler: Failed to retrieve a bullet from the pool.");
            return;
        }

        var audio = GetComponent<AudioSource>();
        if (audio && fireClip)
        {
            audio.PlayOneShot(fireClip);
        }
    }
}