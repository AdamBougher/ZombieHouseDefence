using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class turretWeapon
{
    public float RotationSpeed { get; set; }
    public float RayDistance { get; set; }
    public float FireRate { get; set; }
    public Sprite WeaponSprite { get; set; }
    public AudioClip WeaponSound { get; set; }


    public turretWeapon(float rotationSpeed, float rayDistance, float fireRate,
        Sprite weaponSprite, AudioClip weaponSound)
    {
        RotationSpeed = rotationSpeed;
        RayDistance = rayDistance;
        WeaponSprite = weaponSprite;
        WeaponSound = weaponSound;
        FireRate = fireRate;
        
    }
}
