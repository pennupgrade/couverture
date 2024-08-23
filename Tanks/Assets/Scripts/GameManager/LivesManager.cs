using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LivesManager : MonoBehaviour
{
    public static LivesManager Instance { get; private set; }
    public string CurrentLevel = "";

    [SerializeField] private float respawnTime;
    [SerializeField] private int totalLives;

    private int lives = 0;

    private void Awake() {
        lives = totalLives;

        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Prevents the object from being destroyed when changing scenes
        }
        else {
            Destroy(gameObject); // destroys copies
        }
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
            RestartLevel();
            return;
        }

        RestartSublevel();
    }

    private void RestartSublevel() // me when I dedicate a whole function to call a coroutine
    {
        StartCoroutine(TimerToRestart(respawnTime, SceneManager.GetActiveScene().name));
    }

    private void RestartLevel() {
        lives = totalLives; // reset num of lives
        StartCoroutine(TimerToRestart(respawnTime, CurrentLevel));
    }

    private IEnumerator TimerToRestart(float duration, string sceneName) {
        var elapsedTime = 0f;

        while (elapsedTime < duration) {
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        Debug.Log("restart scene");
        SceneManager.LoadScene(sceneName);
    }
}