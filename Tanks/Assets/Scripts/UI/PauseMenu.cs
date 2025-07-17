// SPDX-FileCopyrightText: 2024 The Catanks Contributors
//
// SPDX-License-Identifier: MPL-2.0

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
    [SerializeField] private GameObject optionsCanvasObj;

    private RectTransform panelRt;
    private CanvasGroup panelCg;
    private OptionsMenu optionsMenu;

    private const float PanelAnimTime = 0.35f;

    private void Awake() {
        panelRt = panel.GetComponent<RectTransform>();
        panelCg = panel.GetComponent<CanvasGroup>();
        optionsMenu = optionsCanvasObj.GetComponent<OptionsMenu>();
    }

    private void Start() {
        if (RoomManager.Instance != null) {
            restartButtonText.text = "Restart from Mission 1";

            var rt = restartButtonText.gameObject.transform.parent.GetComponent<RectTransform>();
            var initSizeDelta = rt.sizeDelta;
            rt.sizeDelta = new Vector2(290f, initSizeDelta.y);

            quitButtonText.text = "Back to main menu";
            currentStatus.text = $"Mission {RoomManager.LevelNum}";
        }
    }

    public bool IsAnimating => LeanTween.isTweening(panel) || LeanTween.isTweening(panelRt) || optionsMenu.IsAnimating;

    public void ShowPanel() {
        panel.SetActive(true);

        LeanTween.value(panel, value => {
            panelCg.alpha = value;
            overlay.alpha = value;
        }, 0f, 1f, PanelAnimTime).setEaseOutExpo().setIgnoreTimeScale(true);

        LeanTween.moveY(panelRt, -200f, 0f).setIgnoreTimeScale(true);
        LeanTween.moveY(panelRt, 0f, PanelAnimTime).setEaseOutExpo().setIgnoreTimeScale(true);
    }

    private void HidePanel()
    {
        LeanTween.value(panel, value =>
        {
            panelCg.alpha = value;
            overlay.alpha = value;
        }, 1f, 0f, PanelAnimTime).setEaseInExpo().setIgnoreTimeScale(true);

        LeanTween.moveY(panelRt, -200f, PanelAnimTime).setEaseInExpo()
                 .setIgnoreTimeScale(true)
                 .setOnComplete(() => panel.SetActive(false));
    }
    
    public void HandleHidingPanels()
    {
        HidePanel();
        optionsMenu.CloseOptionsMenu();
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
        }
        else {
            // Classic mode
            RoomManager.Instance.paused = false;
            RoomManager.Instance.ResumeGame();
        }

        HandleHidingPanels();
    }

    public void HandleRestartLevel() {
        if (GameManager.Instance != null) {
            // Campaign mode
            UIManager.Instance.pauseMenu.HandleHidingPanels();
            GameManager.Instance.RestartLevel(PanelAnimTime + 0.01f);
        }
        else {
            // Classic does not have restart
            UIManager.Instance.pauseMenu.HandleHidingPanels();
            RoomManager.Instance.ResetToLevelOne();
        }
    }

    public void HandleOptions() {
        if (optionsMenu.IsAnimating) return;

        optionsCanvasObj.SetActive(true);
    }

    public void HandleQuitToLevelSelect() {
        SaveStateManagerGameObject.ExitLevel();
        SaveStateManagerGameObject.SaveToFile();

        if (RoomManager.Instance != null) {
            UIManager.Instance.pauseMenu.HandleHidingPanels();
            RoomManager.Instance.ReturnToMainMenu();
        }
        else {
            StartCoroutine(HandleQuitToLevelSelectFromCampaign());
        }

        Time.timeScale = 1;
    }

    private static IEnumerator HandleQuitToLevelSelectFromCampaign() {
        UIManager.Instance.pauseMenu.HandleHidingPanels();

        yield return new WaitForSecondsRealtime(PanelAnimTime);

        SceneTransition.I.Appear(SceneTransition.TransitionType.Fade);

        var operation = SceneManager.LoadSceneAsync("LevelSelect")!;
        operation.allowSceneActivation = false;

        yield return new WaitWhile(() => SceneTransition.I.IsAnimating);

        operation.allowSceneActivation = true;
    }
}