using UnityEngine.Audio;
using System;
using UnityEngine;
using Unity.VisualScripting;
using System.Collections.Generic;

//Credit to Brackeys youtube tutorial on Audio managers, as the majority of this code and learning how to use it was made by him.

public partial class AudioManager : MonoBehaviour
{
    public Sound[] sounds;

    public static AudioManager instance;

    public bool singleton;

    public List<Sound> playedSounds;

    void Awake()
    {
        if (singleton)
        {
            if (instance == null)
                instance = this;
            else
            {
                Destroy(gameObject);
                return;
            }

            DontDestroyOnLoad(gameObject);
        }

        playedSounds = new List<Sound>();

        foreach (Sound s in sounds)
        {
            //GameObject newObj = Instantiate(new GameObject(), null);

            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;

            s.source.volume = s.volume;
            s.source.pitch = 1f;
            s.source.loop = s.loop;
            s.source.spatialBlend = s.spatialBlend;

            if (s.playOnAwake)
            {
                Play(s.name);
            }

            if (!s.source.loop && s.destroyOnComplete)
            {
                Destroy(s.source, s.source.clip.length);
            }

            if (s.pool)
                playedSounds.Add(s);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Play("Sound");
            print("HI");
        }
    }

    public void Play(string name)
    {
        Sound s;

        Sound foundS = playedSounds.Find(sound => sound.name == name);

        if (foundS != null)
        {
            s = foundS;
        }
        else
        {
            s = Array.Find(sounds, sound => sound.name == name);
        }

        if (s == null)
        {
            Debug.LogWarning("Sound: " + name + " not found");
            return;
        }

        s.source.Play();

        foreach (SoundEffect effect in s.effects)
        {
            effect.Invoke(s);
        }
    }

    //this addition to the code was made by me, the rest was from Brackeys tutorial
    public void Stop(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);

        s.source.Stop();
    }
}

