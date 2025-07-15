// SPDX-FileCopyrightText: 2017 Brakeys<business@brackeys.com>
// SPDX-FileCopyrightText: 2024 The Catanks Contributors
//
// SPDX-License-Identifier: MPL-2.0

using UnityEngine.Audio;
using System;
using UnityEngine;
using Unity.VisualScripting;
using System.Collections.Generic;


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
            if (instance != null) {
			    Destroy(gameObject);
                return;
            } else {
                instance = this;
                DontDestroyOnLoad(gameObject);
		    }
        }

        playedSounds = new List<Sound>();


        foreach (Sound s in sounds)
        {
            //GameObject newObj = Instantiate(new GameObject(), null);

            s.source = gameObject.AddComponent<AudioSource>();
            s.source.playOnAwake = false;
            s.source.clip = s.clip;

            UpdateSoundVolume(s);
            s.source.pitch = s.pitch;
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

    public void Stop(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);

        s.source.Stop();
    }

    private void UpdateSoundVolume(Sound s) {
        s.source.volume = s.volume * (s.isBGM ? SaveStateManagerGameObject.GetMusicVolume() : SaveStateManagerGameObject.GetSFXVolume());
    }

    public void UpdateAllSoundVolume() {
        foreach (Sound s in sounds) {
            UpdateSoundVolume(s);
        }
        
        foreach (Sound s in playedSounds) {
            UpdateSoundVolume(s);
        }
    }

    void OnEnable() {
        // ensures that sound volume is updated when audiomanager is enabled after being disabled during a volume change
        UpdateAllSoundVolume();
    }
}

