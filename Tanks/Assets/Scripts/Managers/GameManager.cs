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

    public void GoToSubLevel(float transitionTime, string sceneName)
    {
        StartCoroutine(TimerToRestart(transitionTime, sceneName));
    }

    public void RestartSublevel() // me when I dedicate a whole function to call a coroutine
    {
        GoToSubLevel(livesManager.GetRespawnTime(), SceneManager.GetActiveScene().name);
    }

    public void RestartLevel()
    {
        livesManager.ResetLives();
        GoToSubLevel(livesManager.GetRespawnTime(), CurrentLevel);
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
}
