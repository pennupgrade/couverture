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

    private const float IrisInitSize = 0f;
    private const float IrisFinalSize = 4.5f;

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
        }

        fadeOverlay = transform.Find("Fade Overlay").GetComponent<CanvasGroup>();
        irisOverlay = transform.Find("Iris Overlay").GetComponent<Image>();
        irisMat = irisOverlay.material;

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
        Debug.Log($"Scene loaded: {scene.name}");

        if (IsInACampaignLevel) {
            UpdatePosition();
            UIManager.Instance.pauseMenu.SetStatus(scene.name);

            // These calls have no effect if the game isn't currently paused
            UIManager.Instance.pauseMenu.HidePanel();
            GameManager.Instance.ResumeGame();
        }

        Disappear(type);
    }

    public void UpdatePosition() {
        var tankObj = GameObject.FindGameObjectWithTag("Player");
        var viewportPos = Camera.main!.WorldToViewportPoint(tankObj.transform.position);

        irisMat.SetFloat(PositionXId, Mathf.Clamp01(viewportPos.x));
        irisMat.SetFloat(PositionYId, Mathf.Clamp01(viewportPos.y));
    }

    public bool IsAnimating => LeanTween.isTweening(irisOverlay.gameObject);
    public static bool IsInACampaignLevel => UIManager.Instance != null;

    public void Appear(TransitionType newType) {
        type = newType;

        switch (type) {
        case TransitionType.Fade:
            Debug.Log("TODO fade appear!");
            break;

        case TransitionType.Iris:
            LeanTween.value(irisOverlay.gameObject, value => {
                irisMat.SetFloat(SizeId, value);
            }, IrisFinalSize, IrisInitSize, 2f).setEaseInOutExpo().setIgnoreTimeScale(true);
            break;

        default:
            throw new ArgumentOutOfRangeException(type.ToString());
        }
    }

    public void Disappear(TransitionType newType) {
        type = newType;

        switch (type) {
        case TransitionType.Fade:
            Debug.Log("TODO fade disappear!");
            break;

        case TransitionType.Iris:
            LeanTween.value(irisOverlay.gameObject, value => {
                irisMat.SetFloat(SizeId, value);
            }, IrisInitSize, IrisFinalSize, 2f).setEaseInOutExpo().setIgnoreTimeScale(true);
            break;

        default:
            throw new ArgumentOutOfRangeException(type.ToString());
        }
    }
}