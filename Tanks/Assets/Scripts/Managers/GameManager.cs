using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    // [HideInInspector] public LivesManager livesManager;
    private string currentLevel;
    public float respawnTime;
    // public int totalLives;

    void Awake()
    {
        Debug.Log("Initialize game manager");

        Instance = this;
        currentLevel = SceneManager.GetActiveScene().name;
        // livesManager = new LivesManager(respawnTime, totalLives);
    }

    void Start() {
        SaveStateManagerGameObject.DebugLoadSave();
        SaveStateManagerGameObject.LoadLevel(currentLevel);
    }

    public void SwitchSublevel(float transitionTime, string sceneName)
    {
        StartCoroutine(TimerToRestart(transitionTime, sceneName));
    }

    public void Respawn() // me when I dedicate a whole function to call a coroutine
    {
        StartCoroutine(TimerToRestart(respawnTime, SceneManager.GetActiveScene().name));
    }

    public void RestartLevel()
    {
        // livesManager.ResetLives();
        StartCoroutine(TimerToRestart(respawnTime, currentLevel));
    }

    // Duration should be at least 0.1 seconds (necessary for PlayerCamera.SmoothMoveCamera)
    private IEnumerator TimerToRestart(float duration, string sceneName)
    {
        var elapsedTime = 0f;

        var operation = SceneManager.LoadSceneAsync(sceneName);
        operation!.allowSceneActivation = false;

        while (elapsedTime <= duration)
        {
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        Debug.Log($"Restart to scene '{sceneName}'");
        operation.allowSceneActivation = true;
    }

    public float GetRespawnTime() {
        return respawnTime;
    }
}
