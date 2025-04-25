using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ClassicUIManager : MonoBehaviour
{
    [Header("Overlay")]
    [SerializeField] private RectTransform overlay;
    [SerializeField] private Image overlayImage;
    [SerializeField] private Image leftWaveImage;
    [SerializeField] private Image rightWaveImage;

    [Header("Other stuff")]
    [SerializeField] private RectTransform secondOverlay;
    [SerializeField] private GameObject restartQuitCanvasPrefab;
    [SerializeField] private RectTransform missionPanel;
    [SerializeField] private TMP_Text missionText;
    [SerializeField] private RectTransform enemyCountBar;
    [SerializeField] private TMP_Text enemyCountText;
    [SerializeField] private RectTransform missionCompleteBar;
    [SerializeField] private RectTransform[] stickers;
    [SerializeField] private RectTransform missionFailed;
    [SerializeField] private TMP_Text missionsBeatenText;
    [SerializeField] private RectTransform missionsBeatenRt;
    [SerializeField] private RectTransform optionsPanel;

    private void Start() {
        Reset();
    }

    public bool IsAnimating {
        get {
            var stickersAreTweening = stickers.Aggregate(false, (acc, sticker) => acc || LeanTween.isTweening(sticker));

            return LeanTween.isTweening(missionPanel) || LeanTween.isTweening(overlay) ||
                   LeanTween.isTweening(enemyCountBar) || LeanTween.isTweening(secondOverlay) ||
                   LeanTween.isTweening(missionFailed) || LeanTween.isTweening(optionsPanel) || stickersAreTweening;
        }
    }

    public void Reset() {
        LeanTween.moveX(missionPanel, 2100f, 0f).setIgnoreTimeScale(true);
        LeanTween.moveX(enemyCountBar, 2100f, 0f).setIgnoreTimeScale(true);
        LeanTween.moveY(enemyCountBar, 159f, 0f).setIgnoreTimeScale(true);
        LeanTween.scale(enemyCountBar, new Vector3(0.5f, 0.5f, 0.5f), 0f).setIgnoreTimeScale(true);
        LeanTween.moveX(missionCompleteBar, 2100f, 0f).setIgnoreTimeScale(true);
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

    private void WinScreenEnter() { }

    public void MissionCompleteEnter(bool beatClassicMode) {
        if (beatClassicMode) {
            // if lvl 50 complete then display the end game screen instead
            WinScreenEnter();
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

    public void UpdateEnemyCount(int count) {
        enemyCountText.text = $"Enemies left <b>\u00d7 {count}</b>";
    }

    public void DeathScreenEnter(int levelNum) {
        missionsBeatenText.text = $"Reached Mission {levelNum} out of 50";

        LeanTween.moveX(overlay, 2100f, 0f).setIgnoreTimeScale(true);
        overlayImage.color = CatPFP.Brown;
        leftWaveImage.color = CatPFP.Brown;
        rightWaveImage.color = CatPFP.Brown;

        LeanTween.moveX(overlay, 0f, 1f).setEaseOutExpo().setIgnoreTimeScale(true);
        LeanTween.moveX(missionFailed, 0f, 1f).setDelay(0.15f).setEaseOutExpo().setIgnoreTimeScale(true);
        LeanTween.moveX(missionsBeatenRt, 0f, 1f).setDelay(0.30f).setEaseOutExpo().setIgnoreTimeScale(true);
        LeanTween.moveX(optionsPanel, 0f, 1f).setDelay(0.45f).setEaseOutExpo().setIgnoreTimeScale(true);
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