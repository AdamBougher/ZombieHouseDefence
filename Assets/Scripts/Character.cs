using System;
using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public abstract class Character : MonoBehaviour , IHittable
{
    [BoxGroup("Character"),]
    public CharacterResource Hp;
    [BoxGroup("Character")]
    public float speed;
    [ShowInInspector, ReadOnly, BoxGroup("Character")]
    public float speedMod = 0;
    [BoxGroup("experance")]
    public int level = 1;
    
    protected AudioSource AudioSource;
    
    protected float GetSpeed()
    {
        return speed + speedMod;
    }

    public virtual void Damage(int amt)
    {
        Hp.DecreaseCurrent(amt);
    }

    protected IEnumerator PlaySound(AudioClip clip)
    {
        if (AudioSource == null)
        {
            Debug.LogError("Character.PlaySound: AudioSource is not initialized.");
            yield break;
        }

        bool played = AudioSource.PlaySound(clip);
        if (!played)
            yield break;
            
        yield return new WaitWhile(() => AudioSource.isPlaying);
    }
    
}