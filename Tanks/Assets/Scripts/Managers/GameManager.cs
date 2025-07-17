// SPDX-FileCopyrightText: 2024 The Catanks Contributors
//
// SPDX-License-Identifier: MPL-2.0

using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using TransitionType = SceneTransition.TransitionType;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public bool IsPaused { get; set; }

    private string currentLevel;
    public float respawnTime;
    private Tank player;

    private void Awake() {
        Debug.Log("Initialize game manager");

        Instance = this;
        currentLevel = SceneManager.GetActiveScene().name;

        // this is OK because SaveStateManagerGameObject has execution order -1, Tank has execution order -2
        player = Tank.FindPlayer();
        SaveStateManagerGameObject.DebugLoadSave();
        SaveStateManagerGameObject.LoadLevel(currentLevel);
    }

    private void Update() {
        if (SceneTransition.I.IsAnimating || UIManager.Instance.pauseMenu.IsAnimating) return;

        // Only enable toggling pause if the cat selection panel isn't open
        if (UIManager.Instance.Cat_Selection_Panel.activeInHierarchy) return;

        if (Input.GetKeyDown(KeyCode.Escape)) {
            IsPaused = !IsPaused;

            if (IsPaused) {
                PauseGame();
                UIManager.Instance.pauseMenu.ShowPanel();
            }
            else {
                ResumeGame();
                UIManager.Instance.pauseMenu.HandleHidingPanels();
            }
        }
    }

    public void GoToNextLevel(float transitionTime, string nextSceneName,
                              TransitionType transitionType = TransitionType.Level) =>
        StartCoroutine(_GoToNextLevel(transitionTime, nextSceneName, transitionType));

    private static IEnumerator _GoToNextLevel(float duration, string nextSceneName,
                                              TransitionType transitionType) {
        var sceneNameToSave = nextSceneName;
        if (sceneNameToSave == "CreditsScreen") {
            sceneNameToSave = null;
        }

        SaveStateManagerGameObject.FinishLevel(sceneNameToSave, true);

        if (sceneNameToSave == null) {
            SaveStateManagerGameObject.ExitCurrentSave();
        }

        var levelNumber = SaveStateManagerGameObject.GetLevelNumberFromSceneName(nextSceneName);

        SceneTransition.I.Appear(transitionType, levelNumber);

        var operation = SceneManager.LoadSceneAsync(nextSceneName)!;
        operation.allowSceneActivation = false;

        // Wait on the max between duration and the scene transition duration
        yield return new WaitForSecondsRealtime(duration);
        yield return new WaitWhile(() => SceneTransition.I.IsAnimating);

        operation.allowSceneActivation = true;
    }

    public void Respawn() {
        SaveStateManagerGameObject.PlayerDied();
        StartCoroutine(TimerToRestart(respawnTime, SceneManager.GetActiveScene().name, true));
    }

    public void RestartLevel() {
        StartCoroutine(TimerToRestart(respawnTime, currentLevel));
    }

    public void RestartLevel(float duration) => StartCoroutine(TimerToRestart(duration, currentLevel));

    private IEnumerator TimerToRestart(float duration, string sceneName, bool respawn = false) {
        var operation = SceneManager.LoadSceneAsync(sceneName)!;
        operation.allowSceneActivation = false;

        if (respawn) {
            SceneTransition.I.SetType(TransitionType.None);
        }
        else {
            // Only do scene transition if we're not respawning (aka we're entering new level)
            SceneTransition.I.UpdateIrisPosition(player.gameObject);
            SceneTransition.I.Appear(TransitionType.Fade);
        }

        // Duration should be at least 0.1 seconds (necessary for PlayerCamera.SmoothMoveCamera)
        var actualDuration = Mathf.Max(0.1f, duration);

        // Wait on the max between duration and the scene transition duration
        yield return new WaitForSecondsRealtime(actualDuration);
        yield return new WaitWhile(() => SceneTransition.I.IsAnimating);

        operation.allowSceneActivation = true;
    }

    public float GetRespawnTime() => respawnTime;

    public void PauseGame() {
        Time.timeScale = 0;
        player.Freeze(false);
    }

    public void ResumeGame() {
        Time.timeScale = 1;
        player.Unfreeze();
    }
}