using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(menuName ="Gun data", fileName ="New Gun Data")]
public class GunData : ScriptableObject
{
    [FormerlySerializedAs("Size")] [TabGroup("Ammo and Damage")]
    public int size;

    [FormerlySerializedAs("StartingMagazines")] [TabGroup("Ammo and Damage")]
    public int startingMagazines;

    [FormerlySerializedAs("BulletTrail")] [TabGroup("Other")]
    public GameObject bulletTrail;
    [FormerlySerializedAs("ShootDelay")] [TabGroup("Ammo and Damage")]
    public float shootDelay = 0.5f;
    [TabGroup("Ammo and Damage")]
    public bool weaponDamageIsRange;
    [FormerlySerializedAs("DamageRange")] [ShowIf("weaponDamageIsRange"), TabGroup("Ammo and Damage")]
    public Vector2 damageRange;
    [FormerlySerializedAs("Damage")] [HideIf("weaponDamageIsRange"), TabGroup("Ammo and Damage")]
    public int damage;
    [TabGroup("Other")]
    public int bulletSpawnIndex;
    [TabGroup("Other")]
    public bool usesAmmo;


    [FormerlySerializedAs("Fire")] [TabGroup("Audio")]
    public AudioClip fire;

    [FormerlySerializedAs("Empty")] [TabGroup("Audio")]
    public AudioClip empty;

    [FormerlySerializedAs("Reload")] [TabGroup("Audio")]
    public AudioClip[] reload;

    [ShowInInspector, TabGroup("Audio")]
    public AudioSource audioSource;

}
