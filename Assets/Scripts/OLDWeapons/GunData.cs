using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(menuName = "Gun Data", fileName = "NewGunData")]
public class GunData : ScriptableObject
{
    [Header("Ammo Settings")]
    [Tooltip("Bullets per magazine.")]
    public int magazineCapacity = 30;
    [Tooltip("How many magazines you start with.")]
    public int initialMagazineCount = 3;
    [Tooltip("Does this weapon consume ammo?")]
    public bool usesAmmo = true;
    [Tooltip("Time (s) between consecutive shots.")]
    public float shootDelay = 0.5f;

    [Header("Damage Settings")]
    [Tooltip("If true, damage is randomized between damageRange.x and damageRange.y.")]
    public bool damageIsRandom = false;
    [Tooltip("Min/Max damage when randomizing.")]
    public Vector2 damageRange = new Vector2(10, 20);
    [Tooltip("Fixed damage per shot when not randomizing.")]
    public int fixedDamage = 15;

    [Header("Bullet & Trail")]
    [Tooltip("Prefab for bullet trail or effect.")]
    public GameObject bulletTrailPrefab;
    [Tooltip("Index of the child transform used as the bullet spawn point.")]
    public int bulletSpawnIndex = 0;

    [Header("Audio Clips")]
    [Tooltip("Clip played when firing.")]
    public AudioClip fireClip;
    [Tooltip("Clip played when attempting to fire on empty.")]
    public AudioClip emptyClip;
    [Tooltip("Clips to choose from when reloading.")]
    public AudioClip[] reloadClips;
}
