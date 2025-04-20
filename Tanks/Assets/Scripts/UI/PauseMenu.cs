using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private GameObject panel;
    [SerializeField] private TMP_Text currentStatus;

    [SerializeField] private TMP_Text restartButtonText;
    [SerializeField] private TMP_Text quitButtonText;
    
    void Start() {
        if (RoomManager.Instance != null) {
            restartButtonText.text = "Reset to Level 1";
            quitButtonText.text = "Back to Main Menu";
        }
    }
    public void ShowPanel() => panel.SetActive(true);
    public void HidePanel() {
        // Panel will be null if we're returning to level select UI
        if (panel == null) return;
        panel.SetActive(false);
    }

    public void SetStatus(string sceneName) {
        if (GameManager.Instance != null) {
            // Campaign mode
            var levelNumber = -1;
            if (sceneName.Contains('1')) {
                levelNumber = 1;
            } else if (sceneName.Contains('2')) {
                levelNumber = 2;
            } else if (sceneName.Contains('3')) {
                levelNumber = 3;
            } else if (sceneName.Contains('4')) {
                levelNumber = 4;
            } else if (sceneName.Contains('5')) {
                levelNumber = 5;
            } else if (sceneName.Contains('6')) {
                levelNumber = 6;
            }
            else {
                Debug.LogWarning("SetStatus(): could not find level number!");
            }

            currentStatus.text = $"Level {levelNumber}";
        }
        else {
            // Classic mode
            currentStatus.text = $"Level {RoomManager.LevelNum}";
        }
    }

    public void HandleResume() {
        if (GameManager.Instance != null) {
            // Campaign mode
            GameManager.Instance.paused = false;
            GameManager.Instance.ResumeGame();
            HidePanel();
        }
        else {
            // Classic mode
            HidePanel();
            RoomManager.Instance.ResumeGame();
        }
    }

    public void HandleRestartLevel() {
        if (GameManager.Instance != null) {
            // Campaign mode
            SceneTransition.I.RestartClickedFromPauseMenu = true;
            GameManager.Instance.RestartLevel();
        } else {
            // Classic does not have restart
            RoomManager.Instance.ResetToLevelOne();
        }
    }

    public void HandleOptions() {
        Debug.LogWarning("HandleOptions(): TODO");
    }

    public void HandleQuitToLevelSelect() {
        SaveStateManagerGameObject.ExitLevel();
        SaveStateManagerGameObject.SaveToFile();

        if (RoomManager.Instance != null) {
            RoomManager.Instance.destroyIt();
            EnemySpawner.reset();
            SceneManager.LoadScene("TitleScreen");
        } else {
            SceneManager.LoadScene("LevelSelect");
        }
        Time.timeScale = 1;
    }
}
