using System.Collections;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ClassicUIManager : MonoBehaviour
{
    [SerializeField] private RectTransform overlay;
    [SerializeField] private RectTransform secondOverlay;
    [SerializeField] private GameObject restartQuitCanvasPrefab;
    [SerializeField] private RectTransform missionPanel;
    [SerializeField] private TMP_Text missionText;
    [SerializeField] private RectTransform enemyCountBar;
    [SerializeField] private TMP_Text enemyCountText;
    [SerializeField] private RectTransform missionCompleteBar;
    [SerializeField] private RectTransform[] stickers;

    public TMP_Text levelsBeatenText;
    public CanvasGroup endGameScreen;
    public CanvasGroup deathScreen;
    public Button backToHome;
    public Button backToHome_fromDeath;

    private void Start() {
        Reset();
        backToHome.onClick.AddListener(ReturnToHome);
    }

    public bool IsAnimating {
        get {
            var stickersAreTweening = stickers.Aggregate(false, (acc, sticker) => acc || LeanTween.isTweening(sticker));

            return LeanTween.isTweening(missionPanel) || LeanTween.isTweening(overlay) ||
                   LeanTween.isTweening(enemyCountBar) || LeanTween.isTweening(secondOverlay) || stickersAreTweening;
        }
    }

    public void Reset() {
        LeanTween.moveX(missionPanel, 2100f, 0f).setIgnoreTimeScale(true);
        LeanTween.moveX(enemyCountBar, 2100f, 0f).setIgnoreTimeScale(true);
        LeanTween.moveY(enemyCountBar, 159f, 0f).setIgnoreTimeScale(true);
        LeanTween.scale(enemyCountBar, new Vector3(0.5f, 0.5f, 0.5f), 0f).setIgnoreTimeScale(true);
        LeanTween.moveX(missionCompleteBar, 2100f, 0f).setIgnoreTimeScale(true);

        levelsBeatenText.alpha = 1;
        endGameScreen.alpha = 0;
        endGameScreen.gameObject.SetActive(false);
        deathScreen.alpha = 0;
        deathScreen.gameObject.SetActive(false);
        backToHome.onClick.AddListener(ReturnToHome);
        backToHome_fromDeath.onClick.AddListener(ReturnToHome);
    }

    public void ReturnToHome() {
        if (RoomManager.Instance != null) RoomManager.Instance.DestroyIt();
        EnemySpawner.reset();
        SceneManager.LoadScene("TitleScreen");
    }

    public void StartScreenEnter(int missionNum) {
        missionText.text = "Mission " + missionNum;

        LeanTween.moveX(missionPanel, 0f, 1f).setDelay(0.05f).setEaseOutExpo().setIgnoreTimeScale(true);
        LeanTween.moveX(enemyCountBar, 0f, 1f).setDelay(0.4f).setEaseOutExpo().setIgnoreTimeScale(true);
    }

    public void StartScreenLeave() {
        LeanTween.moveX(missionPanel, -2100f, 1f).setEaseInExpo().setIgnoreTimeScale(true);
        LeanTween.moveX(overlay, -2100f, 1f).setDelay(0.15f).setEaseInExpo().setIgnoreTimeScale(true);

        LeanTween.moveY(enemyCountBar, 470f, 1f).setEaseOutExpo().setIgnoreTimeScale(true);
        LeanTween.scale(enemyCountBar, new Vector3(0.35f, 0.35f, 0.35f), 1f).setEaseOutExpo().setIgnoreTimeScale(true);
    }

    public void MissionCompleteEnter(bool beatClassicMode) {
        if (beatClassicMode) {
            // if lvl 50 complete then display the end game screen instead
            StartCoroutine(Fade(endGameScreen, true));
            return;
        }

        secondOverlay.gameObject.SetActive(true);
        LeanTween.moveX(secondOverlay, 2100f, 0f).setIgnoreTimeScale(true);
        LeanTween.moveX(secondOverlay, 0f, 1f).setEaseOutExpo().setIgnoreTimeScale(true).setOnComplete(() => {
            LeanTween.moveY(enemyCountBar, 700f, 0f).setIgnoreTimeScale(true);
            LeanTween.moveX(overlay, 0f, 0f).setIgnoreTimeScale(true);
            secondOverlay.gameObject.SetActive(false);
        });

        LeanTween.moveX(missionCompleteBar, 2100f, 0f).setIgnoreTimeScale(true);
        LeanTween.moveX(missionCompleteBar, 0f, 1f).setDelay(0.15f).setEaseOutExpo().setIgnoreTimeScale(true);

        for (var i = 0; i < stickers.Length; i++) {
            var sticker = stickers[i];
            LeanTween.scale(sticker, Vector3.one, 1f).setDelay((i + 1f) * 0.1f + 0.33f).setEaseOutExpo()
                     .setIgnoreTimeScale(true);
        }
    }

    public void MissionCompleteLeave() {
        LeanTween.moveX(missionCompleteBar, -2100f, 1f).setDelay(0.05f).setEaseInExpo().setIgnoreTimeScale(true);

        for (var i = stickers.Length - 1; i >= 0; i--) {
            var sticker = stickers[i];
            LeanTween.scale(sticker, Vector3.zero, 1f).setDelay((i + 1f) * 0.04f + 0.07f).setEaseInExpo()
                     .setIgnoreTimeScale(true);
        }
    }

    public IEnumerator Fade(CanvasGroup screen, bool fadeInorOut) {
        if (fadeInorOut) {
            screen.gameObject.SetActive(true);
            var t = 0f;
            while (t < 1f) {
                t += Time.deltaTime;
                screen.alpha = t / 1f;
                yield return null;
            }

            screen.alpha = 1f;
        }
        else {
            var t = 0f;
            while (t < 1f) {
                t += Time.deltaTime;
                screen.alpha = 1 - t / 1f;
                yield return null;
            }

            screen.alpha = 0f;
            screen.gameObject.SetActive(false);
        }
    }

    public void UpdateEnemyCount(int count) {
        enemyCountText.text = $"Enemies left <b>\u00d7 {count}</b>";
    }

    public void DeathScreenEnter(int levelNum) {
        // display the end game screen, showing _/60 levels beat,
        // and whether it is a high score or not, return to menu button
        endGameScreen.gameObject.SetActive(false);
        levelsBeatenText.text = "Levels Beat: " + (levelNum - 1) + "/50";
        StartCoroutine(Fade(deathScreen, true));
    }

    public ClassicRestartQuitCanvas Restart() {
        var obj = Instantiate(restartQuitCanvasPrefab);
        var restartQuit = obj.GetComponent<ClassicRestartQuitCanvas>();

        restartQuit.Restart();

        return restartQuit;
    }

    public ClassicRestartQuitCanvas Quit() {
        var obj = Instantiate(restartQuitCanvasPrefab);
        var restartQuit = obj.GetComponent<ClassicRestartQuitCanvas>();

        restartQuit.Quit();

        return restartQuit;
    }
}