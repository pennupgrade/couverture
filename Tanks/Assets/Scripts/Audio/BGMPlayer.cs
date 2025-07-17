// SPDX-FileCopyrightText: 2024 The Catanks Contributors
//
// SPDX-License-Identifier: MPL-2.0

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGMPlayer : MonoBehaviour
{
    private AudioManager audioManager;
    public static BGMPlayer Instance { get; private set; }
    private bool menuBGMPlaying;


    // Start is called before the first frame update
    void Start()
    {
        menuBGMPlaying = false;
        if (Instance == null)
        {
            audioManager = GetComponent<AudioManager>();
            Instance = this;
            PlayMusic();
            DontDestroyOnLoad(this);
        }
        else if (Instance != this)
        {
            Instance.PlayMusic();
            Destroy(gameObject);
        }
    }
    public void PlayMusic()
    {
        if (RoomManager.Instance != null)
        {
            Destroy(gameObject);
            Instance = null;
        }
        else if (GameManager.Instance != null)
        {
            if (menuBGMPlaying)
            {
                audioManager.Stop("Menu");
                menuBGMPlaying = false;
            }
            audioManager.Play("BGM");
        }
        else
        {
            if (!menuBGMPlaying)
            {
                audioManager.Play("Menu");
                menuBGMPlaying = true;
                audioManager.Stop("BGM");
            }
        }
    }

    public void StopMusic()
    {
        audioManager.Stop("Menu");
        menuBGMPlaying = false;
        audioManager.Stop("BGM");
    }
}
