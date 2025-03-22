using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    // [HideInInspector] public LivesManager livesManager;
    public string CurrentLevel;
    public float respawnTime;
    // public int totalLives;

    void Awake()
    {
        Debug.Log("Initialize game manager");

        Instance = this;
        // livesManager = new LivesManager(respawnTime, totalLives);
    }

    void Start() {
        SaveStateManagerGameObject.LoadLevel(SceneManager.GetActiveScene().name);
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
        // TODO: MUST ACCOUNT FOR LEVEL RESTARTING (AKA MUST LOAD SAVE DATA AGAIN, HOW WILL WE DO THIS?)
        // SaveStateManagerGameObject.RestartLevel();
        StartCoroutine(TimerToRestart(respawnTime, CurrentLevel));
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
