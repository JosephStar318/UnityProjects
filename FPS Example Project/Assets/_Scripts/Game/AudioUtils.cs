using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class AudioUtils
{
    public static void CreateSFX(AudioClip sfx, Vector3 pos)
    {
        GameObject sfxObject = new GameObject(sfx.name);
        sfxObject.transform.position = pos;
        AudioSource audioSource = sfxObject.AddComponent<AudioSource>();
        audioSource.clip = sfx;
        audioSource.Play();
        TimedSelfDestruct selfDestruct = sfxObject.AddComponent<TimedSelfDestruct>();
        selfDestruct.LifeTime = sfx.length;
    }
}
