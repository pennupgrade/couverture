using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Sound
{
    public string name;

    public AudioClip clip;

    [Range(0f, 1f)]
    public float volume = 0.3f;

    //[Range(0.1f, 3f)]
    //public float pitch = 1f;
    public bool loop;

    public bool playOnAwake;

    public bool destroyOnComplete;

    public bool pool;

    public float pitch = 1.0f;

    [HideInInspector]
    public AudioSource source;

    [Range(0f, 1f)]
    public float spatialBlend = 1f;

    public SoundEffect[] effects;

    public bool isBGM;
}
