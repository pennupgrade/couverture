using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [HideInInspector] public LivesManager livesManager;
    public string CurrentLevel;
    public float respawnTime;
    public int totalLives;

    private TankStats tankStats = null;

    void Awake()
    {
        if (Instance == null)
        {
            Debug.Log("Initialize game manager");

            Instance = this;
            livesManager = new LivesManager(respawnTime, totalLives);

            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject); //  delete dupes
        }
    }

    public void SwitchSublevel(float transitionTime, string sceneName)
    {
        StartCoroutine(TimerToRestart(transitionTime, sceneName, StoreTankStats));
    }

    public void Respawn() // me when I dedicate a whole function to call a coroutine
    {
        StartCoroutine(TimerToRestart(livesManager.GetRespawnTime(), SceneManager.GetActiveScene().name, () => {}));
        tankStats = null;
    }

    public void RestartLevel()
    {
        livesManager.ResetLives();
        StartCoroutine(TimerToRestart(livesManager.GetRespawnTime(), CurrentLevel, () => {}));
        tankStats = null;
    }

    // Duration should be at least 0.1 seconds (necessary for PlayerCamera.SmoothMoveCamera)
    private IEnumerator TimerToRestart(float duration, string sceneName, Action func)
    {
        var elapsedTime = 0f;

        var operation = SceneManager.LoadSceneAsync(sceneName);
        operation!.allowSceneActivation = false;

        while (elapsedTime <= duration)
        {
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        func();
        Debug.Log($"Restart to scene '{sceneName}'");
        operation.allowSceneActivation = true;
    }

    private void StoreTankStats() {
        Tank t = GameObject.FindWithTag("Player").GetComponent<Tank>();
        tankStats = new TankStats(t);
    }

    public static void TransferStats(Tank t) {
        if (Instance.tankStats != null) {
            Instance.tankStats.TransferStats(t);
        }
        Instance.tankStats = null;
    }

    public static void LoseLife() {
        Instance.livesManager.LoseLife();
        
        if (Instance.livesManager.GetLives() <= 0) {
            Instance.RestartLevel();
            return;
        }

        Instance.Respawn();
    }
}
