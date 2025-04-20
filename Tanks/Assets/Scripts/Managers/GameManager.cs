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
    private Tank player;
    public bool paused;

    void Awake()
    {
        Debug.Log("Initialize game manager");

        Instance = this;
        currentLevel = SceneManager.GetActiveScene().name;
        // livesManager = new LivesManager(respawnTime, totalLives);

        // this is OK because SaveStateManagerGameObject has execution order -1, Tank has execution order -2
        player = GameObject.FindWithTag("Player").GetComponent<Tank>();
        SaveStateManagerGameObject.DebugLoadSave();
        SaveStateManagerGameObject.LoadLevel(currentLevel);

        AudioManager audioManager = GetComponent<AudioManager>();
        audioManager.Play("Level Music");
    }

    private void Update() {
        if (SceneTransition.I.IsAnimating) return;
        
        if (Input.GetKeyDown(KeyCode.Escape)) {
            paused = !paused;
            
            // Only enable toggling pause if the cat selection panel isn't open
            if (UIManager.instance.Cat_Selection_Panel.activeInHierarchy) return;
                
            if (paused) {
                PauseGame();
                UIManager.instance.pauseMenu.ShowPanel();
            }
            else {
                ResumeGame();
                UIManager.instance.pauseMenu.HidePanel();
            }
        }
    }

    public void GoToNextLevel(float transitionTime, string sceneName)
    {
        SaveStateManagerGameObject.FinishLevel(sceneName, true);
        StartCoroutine(TimerToRestart(transitionTime, sceneName));
    }

    public void Respawn() // me when I dedicate a whole function to call a coroutine
    {
        SaveStateManagerGameObject.PlayerDied();
        StartCoroutine(TimerToRestart(respawnTime, SceneManager.GetActiveScene().name, true));
    }

    public void RestartLevel()
    {
        // livesManager.ResetLives();
        StartCoroutine(TimerToRestart(respawnTime, currentLevel));
    }

    // Duration should be at least 0.1 seconds (necessary for PlayerCamera.SmoothMoveCamera)
    private IEnumerator TimerToRestart(float duration, string sceneName, bool respawn = false)
    {
        var operation = SceneManager.LoadSceneAsync(sceneName)!;
        operation.allowSceneActivation = false;

        // Only do scene transition if we're not respawning (aka we're entering new level)
        if (!respawn) {
            SceneTransition.I.UpdatePosition();
            SceneTransition.I.Appear();
        }
        
        // Wait on the max between duration and the scene transition duration
        yield return new WaitForSecondsRealtime(duration);
        yield return new WaitWhile(() => SceneTransition.I.IsAnimating);

        operation.allowSceneActivation = true;
    }

    public float GetRespawnTime() {
        return respawnTime;
    }

    public void PauseGame() {
        Time.timeScale = 0;
        player.Freeze(false);
    }

    public void ResumeGame() {
        Time.timeScale = 1;
        player.Unfreeze();
    }
}
