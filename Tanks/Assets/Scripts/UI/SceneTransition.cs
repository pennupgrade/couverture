using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneTransition : MonoBehaviour
{
    public enum TransitionType
    {
        Iris,
        Fade
    }

    public static SceneTransition I;
    public TransitionType type;

    private CanvasGroup fadeOverlay;
    private Image irisOverlay;
    private Material irisMat;

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
        LeanTween.isTweening(irisOverlay.gameObject) || LeanTween.isTweening(fadeOverlay.gameObject);

    public void Appear(TransitionType newType) {
        type = newType;

        switch (type) {
        case TransitionType.Fade:
            LeanTween.value(fadeOverlay.gameObject, value => {
                fadeOverlay.alpha = value;
            }, 0f, 1f, 0.2f).setIgnoreTimeScale(true);
            break;

        case TransitionType.Iris:
            LeanTween.value(irisOverlay.gameObject, value => {
                irisMat.SetFloat(SizeId, value);
            }, 4.5f, 0f, 2f).setEaseInOutExpo().setIgnoreTimeScale(true);
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
            LeanTween.value(irisOverlay.gameObject, value => {
                irisMat.SetFloat(SizeId, value);
            }, 0f, 4.5f, 2f).setEaseInOutExpo().setIgnoreTimeScale(true);
            break;

        default:
            throw new ArgumentOutOfRangeException(type.ToString());
        }
    }
}