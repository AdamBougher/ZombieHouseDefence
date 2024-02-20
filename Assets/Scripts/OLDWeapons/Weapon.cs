using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class Weapon {

    [ShowInInspector]
    protected float WeaponRange = 100f;

    [ShowInInspector]
    protected bool DamageIsRange;
    [ShowIf("damageIsRange"), ShowInInspector]
    private Vector2 _damageRange;
    [HideIf("damageIsRange"), ShowInInspector]
    private int _damageAmount;

    private AudioSource _audioSource;

    protected AudioClip Fire, Empty;
    protected AudioClip[] ReloadSfx;

    public int GetDamage()
    {
        if(!DamageIsRange)
        {
            return _damageAmount;
        }else{
            return (int)Random.Range(_damageRange.x,_damageRange.y);
        }
    }

    public void SetDamage(int amount)
    {
        DamageIsRange = false;
        _damageAmount = amount;
    }
    public void SetDamage(Vector2 amount)
    {
        DamageIsRange = true;
        _damageRange = amount;
    }


    public void PlaySound(AudioClip clip)
    {
        _audioSource.clip = clip;
        _audioSource.Play();
    }

    public bool IsPlaying
    {
        get
        {
            return _audioSource.isPlaying;
        }
    }


    public void SetAudioSource(AudioSource aSrc)
    {
        _audioSource = aSrc;
    }
}
