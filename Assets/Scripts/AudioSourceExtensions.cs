using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class AudioSourceExtensions
{
    public static bool PlaySound(this AudioSource audioSource, AudioClip clip)
    {
        if (audioSource == null)
        {
            Debug.LogError("AudioSourceExtensions.PlaySound: AudioSource is null.");
            return false;
        }
        
        if (clip == null)
        {
            Debug.LogError("AudioSourceExtensions.PlaySound: AudioClip is null.");
            return false;
        }

        audioSource.clip = clip;
        audioSource.Play();
        return true;
    }
}
