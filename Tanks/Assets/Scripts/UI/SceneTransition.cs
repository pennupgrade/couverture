using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneTransition : MonoBehaviour
{
    public enum TransitionType
    {
        Iris,
        Fade,
        Level,
        None
    }

    public static SceneTransition I;
    public TransitionType type;

    private CanvasGroup fadeOverlay;
    private Image irisOverlay;
    private Material irisMat;
    private RectTransform nowEntering;
    private CanvasGroup nowEnteringCg;
    private RectTransform levelNumberRt;
    private TMP_Text levelNumberText;
    private CanvasGroup levelNumberCg;

    private static readonly int SizeId = Shader.PropertyToID("_Size");
    private static readonly int PositionXId = Shader.PropertyToID("_Position_X");
    private static readonly int PositionYId = Shader.PropertyToID("_Position_Y");

    private void Awake() {
        if (I == null) {
            I = this;
            DontDestroyOnLoad(gameObject);
        }
        else {
            Destroy(gameObject);
            return;
        }

        fadeOverlay = transform.Find("Fade Overlay").GetComponent<CanvasGroup>();
        irisOverlay = transform.Find("Iris Overlay").GetComponent<Image>();
        irisMat = irisOverlay.material;

        var levelNumberObj = transform.Find("Level Number");
        levelNumberRt = levelNumberObj.GetComponent<RectTransform>();
        levelNumberText = levelNumberObj.GetComponent<TMP_Text>();
        levelNumberCg = levelNumberObj.GetComponent<CanvasGroup>();

        var nowEnteringObj = transform.Find("Now Entering");
        nowEntering = nowEnteringObj.GetComponent<RectTransform>();
        nowEnteringCg = nowEnteringObj.GetComponent<CanvasGroup>();

        var levelNumber = SaveStateManager.GetLevelNumberFromSceneName(SceneManager.GetActiveScene().name);
        levelNumberText.text = $"Level {levelNumber}";

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    public void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
        // If we're currently in a campaign level
        if (UIManager.Instance != null) {
            UpdateIrisPosition(GameObject.FindGameObjectWithTag("Player"));
            UIManager.Instance.pauseMenu.SetStatus(scene.name);

            // These calls have no effect if the game isn't currently paused
            UIManager.Instance.pauseMenu.HidePanel();
            GameManager.Instance.ResumeGame();
        }

        Disappear(type);
    }

    public void UpdateIrisPosition(GameObject obj) {
        var viewportPos = Camera.main!.WorldToViewportPoint(obj.transform.position);

        irisMat.SetFloat(PositionXId, Mathf.Clamp01(viewportPos.x));
        irisMat.SetFloat(PositionYId, Mathf.Clamp01(viewportPos.y));
    }

    public void UpdateIrisPosition(float x, float y) {
        irisMat.SetFloat(PositionXId, Mathf.Clamp01(x));
        irisMat.SetFloat(PositionYId, Mathf.Clamp01(y));
    }

    public bool IsAnimating =>
        LeanTween.isTweening(irisOverlay.gameObject) || LeanTween.isTweening(fadeOverlay.gameObject) ||
        LeanTween.isTweening(nowEntering) || LeanTween.isTweening(levelNumberRt) ||
        LeanTween.isTweening(nowEntering.gameObject);

    private void IrisAppear() => LeanTween.value(irisOverlay.gameObject, value => {
        irisMat.SetFloat(SizeId, value);
    }, 4.5f, 0f, 0.8f).setEaseOutExpo().setIgnoreTimeScale(true);

    private void IrisDisappear() => LeanTween.value(irisOverlay.gameObject, value => {
        irisMat.SetFloat(SizeId, value);
    }, 0f, 4.5f, 2f).setEaseInOutExpo().setIgnoreTimeScale(true);

    public void SetType(TransitionType newType) => type = newType;

    public void Appear(TransitionType newType) => Appear(newType, -1);

    public void Appear(TransitionType newType, int levelNumber) {
        type = newType;
        levelNumberText.text = $"Level {levelNumber}";

        switch (type) {
        case TransitionType.Fade:
            LeanTween.value(fadeOverlay.gameObject, value => {
                fadeOverlay.alpha = value;
            }, 0f, 1f, 0.2f).setIgnoreTimeScale(true);
            break;

        case TransitionType.Iris:
            IrisAppear();
            break;

        case TransitionType.Level:
            IrisAppear();

            nowEnteringCg.alpha = 1;
            levelNumberCg.alpha = 1;
            LeanTween.moveY(nowEntering, 300f, 0f).setIgnoreTimeScale(true);
            LeanTween.moveY(levelNumberRt, 300f, 0f).setIgnoreTimeScale(true);

            LeanTween.moveY(nowEntering, -360f, 1.2f).setDelay(0.3f).setEaseOutExpo().setIgnoreTimeScale(true);
            LeanTween.moveY(levelNumberRt, -250f, 1.2f).setDelay(0.55f).setEaseOutExpo().setIgnoreTimeScale(true);
            break;

        case TransitionType.None:
            break;

        default:
            throw new ArgumentOutOfRangeException(type.ToString());
        }
    }

    public void Disappear(TransitionType newType) {
        type = newType;

        switch (type) {
        case TransitionType.Fade:
            LeanTween.value(fadeOverlay.gameObject, value => {
                fadeOverlay.alpha = value;
            }, 1f, 0f, 0.3f).setEaseInExpo().setIgnoreTimeScale(true);
            break;

        case TransitionType.Iris:
            IrisDisappear();
            break;

        case TransitionType.Level:
            IrisDisappear();

            LeanTween.moveY(nowEntering, -360f, 0f).setIgnoreTimeScale(true);
            LeanTween.moveY(levelNumberRt, -250f, 0f).setIgnoreTimeScale(true);

            LeanTween.value(nowEntering.gameObject, value => {
                nowEnteringCg.alpha = value;
                levelNumberCg.alpha = value;
            }, 1f, 0f, 2f).setDelay(1f).setIgnoreTimeScale(true);
            break;

        case TransitionType.None:
            break;

        default:
            throw new ArgumentOutOfRangeException(type.ToString());
        }
    }
}