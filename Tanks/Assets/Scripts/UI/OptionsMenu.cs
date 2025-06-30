using UnityEngine;
using UnityEngine.UI;

public class OptionsMenu : MonoBehaviour
{
    [SerializeField] private CanvasGroup overlay;

    [Header("Panel")]
    [SerializeField] private RectTransform panelRt;
    [SerializeField] private CanvasGroup panelCg;

    [Header("Buttons")]
    [SerializeField] private Button openButton;
    [SerializeField] private Button closeButton;

    public bool IsAnimating => LeanTween.isTweening(overlay.gameObject) || LeanTween.isTweening(panelRt);

    private void Start()
    {
        overlay.alpha = 0f;
    }

    private void OnEnable()
    {
        closeButton.interactable = false;

        LeanTween.value(overlay.gameObject, value =>
        {
            overlay.alpha = value;
            panelCg.alpha = value;
        }, 0f, 1f, 0.35f).setOnComplete(() => overlay.alpha = 1f).setEaseOutExpo().setIgnoreTimeScale(true);

        LeanTween.moveY(panelRt, -200f, 0f).setIgnoreTimeScale(true);
        LeanTween.moveY(panelRt, 0f, 0.35f).setEaseOutExpo().setIgnoreTimeScale(true);

        closeButton.interactable = true;
    }

    public void CloseOptionsMenu()
    {
        closeButton.interactable = false;
        openButton.interactable = false;

        LeanTween.value(overlay.gameObject, value =>
        {
            overlay.alpha = value;
            panelCg.alpha = value;
        }, 1f, 0f, 0.35f).setOnComplete(() => overlay.alpha = 0f).setEaseInExpo().setIgnoreTimeScale(true);

        LeanTween.moveY(panelRt, -200f, 0.35f).setEaseInExpo().setOnComplete(() =>
        {
            closeButton.interactable = true;
            openButton.interactable = true;

            gameObject.SetActive(false);
        }).setIgnoreTimeScale(true);
    }
}
