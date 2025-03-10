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

        if (lives <= 0) {
            GameManager.Instance.RestartLevel();
            return;
        }

        GameManager.Instance.RestartSublevel();
    }

    public void ResetLives()
    {
        lives = totalLives;
    }
}