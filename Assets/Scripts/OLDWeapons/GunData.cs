using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(menuName ="Gun data", fileName ="New Gun Data")]
public class GunData : ScriptableObject
{
    [TabGroup("Ammo and Damage")]
    public int Size,StartingMagazines;
    [TabGroup("Other")]
    public GameObject BulletTrail;
    [TabGroup("Ammo and Damage")]
    public float ShootDelay = 0.5f;
    [TabGroup("Ammo and Damage")]
    public bool weaponDamageIsRange;
    [ShowIf("weaponDamageIsRange"), TabGroup("Ammo and Damage")]
    public Vector2 DamageRange;
    [HideIf("weaponDamageIsRange"), TabGroup("Ammo and Damage")]
    public int Damage;
    [TabGroup("Other")]
    public int bulletSpawnIndex;
    [TabGroup("Other")]
    public bool usesAmmo;


    [TabGroup("Audio")]
    public AudioClip Fire, Empty;
    [TabGroup("Audio")]
    public AudioClip[] Reload;

    [ShowInInspector, TabGroup("Audio")]
    public AudioSource audioSource;

}
