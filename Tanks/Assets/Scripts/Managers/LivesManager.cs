// SPDX-FileCopyrightText: 2024 The Catanks Contributors
//
// SPDX-License-Identifier: MPL-2.0

using System;
using System.Collections;
using UnityEngine;

public class LivesManager
{
    [SerializeField] private float respawnTime;
    [SerializeField] private int totalLives;
    private int lives = 0;

    public LivesManager(float respawnTime, int totalLives)
    {
        this.respawnTime = respawnTime;
        this.totalLives = totalLives;
        lives = totalLives;
    }

    public int GetLives() {
        return lives;
    }

    public float GetRespawnTime() {
        return respawnTime;
    }

    public void LoseLife() {
        lives -= 1;
        Debug.Log("Lives: " + lives);
    }

    public void ResetLives()
    {
        lives = totalLives;
    }

    public void SetLives(int numLives) {
        if (numLives > totalLives || numLives <= 0) {
            throw new ArgumentException();
        }
        lives = numLives;
    }
}