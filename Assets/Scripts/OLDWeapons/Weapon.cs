using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class Weapon {

    [ShowInInspector]
    protected float weaponRange = 100f;

    [ShowInInspector]
    protected bool damageIsRange;
    [ShowIf("damageIsRange"), ShowInInspector]
    private Vector2 damageRange;
    [HideIf("damageIsRange"), ShowInInspector]
    private int damageAmount;

    private AudioSource audioSource;

    protected AudioClip Fire, Empty;
    protected AudioClip[] ReloadSFX;

    public int getDamage()
    {
        if(!damageIsRange)
        {
            return damageAmount;
        }else{
            return (int)Random.Range(damageRange.x,damageRange.y);
        }
    }

    public void setDamage(int amount)
    {
        damageIsRange = false;
        damageAmount = amount;
    }
    public void setDamage(Vector2 amount)
    {
        damageIsRange = true;
        damageRange = amount;
    }


    public void PlaySound(AudioClip clip)
    {
        audioSource.clip = clip;
        audioSource.Play();
    }

    public bool isPlaying
    {
        get
        {
            return audioSource.isPlaying;
        }
    }


    public void setAudioSource(AudioSource aSrc)
    {
        audioSource = aSrc;
    }
}
