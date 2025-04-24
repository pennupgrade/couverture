using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private GameObject panel;
    [SerializeField] private TMP_Text currentStatus;
    [SerializeField] private CanvasGroup overlay;

    [SerializeField] private TMP_Text restartButtonText;
    [SerializeField] private TMP_Text quitButtonText;

    private RectTransform panelRt;
    private CanvasGroup panelCg;

    private const float AnimationTime = 0.35f;

    private void Awake() {
        panelRt = panel.GetComponent<RectTransform>();
        panelCg = panel.GetComponent<CanvasGroup>();
    }

    private void Start() {
        if (RoomManager.Instance != null) {
            restartButtonText.text = "Reset to Level 1";
            quitButtonText.text = "Back to Main Menu";
            currentStatus.text = $"Level {RoomManager.LevelNum}";
        }
    }

    public bool IsAnimating => LeanTween.isTweening(panel) || LeanTween.isTweening(panelRt);

    public void ShowPanel() {
        panel.SetActive(true);

        LeanTween.value(panel, value => {
            panelCg.alpha = value;
            overlay.alpha = value;
        }, 0f, 1f, AnimationTime).setEaseOutExpo().setIgnoreTimeScale(true);

        LeanTween.moveY(panelRt, -200f, 0f).setIgnoreTimeScale(true);
        LeanTween.moveY(panelRt, 0f, AnimationTime).setEaseOutExpo().setIgnoreTimeScale(true);
    }

    public void HidePanel() {
        LeanTween.value(panel, value => {
            panelCg.alpha = value;
            overlay.alpha = value;
        }, 1f, 0f, AnimationTime).setEaseInExpo().setIgnoreTimeScale(true);

        LeanTween.moveY(panelRt, -200f, AnimationTime).setEaseInExpo()
                 .setIgnoreTimeScale(true)
                 .setOnComplete(() => panel.SetActive(false));
    }

    public void SetStatus(string sceneName) {
        // Campaign mode only
        if (GameManager.Instance == null) return;

        var levelNumber = SaveStateManagerGameObject.GetLevelNumberFromSceneName(sceneName);
        currentStatus.text = $"Level {levelNumber}";
    }

    public void HandleResume() {
        if (GameManager.Instance != null) {
            // Campaign mode
            GameManager.Instance.IsPaused = false;
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
            GameManager.Instance.RestartLevel();
        }
        else {
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
        }
        else {
            StartCoroutine(HandleQuitToLevelSelectFromCampaign());
        }

        Time.timeScale = 1;
    }

    private static IEnumerator HandleQuitToLevelSelectFromCampaign() {
        SceneTransition.I.Appear(SceneTransition.TransitionType.Fade);

        var operation = SceneManager.LoadSceneAsync("LevelSelect")!;
        operation.allowSceneActivation = false;

        yield return new WaitWhile(() => SceneTransition.I.IsAnimating);

        operation.allowSceneActivation = true;
    }
}