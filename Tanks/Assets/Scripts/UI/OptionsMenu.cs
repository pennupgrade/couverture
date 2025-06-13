using UnityEngine;
using UnityEngine.UI;

public class OptionsMenu : MonoBehaviour
{
    [SerializeField] private CanvasGroup overlay;

    [Header("Buttons")]
    [SerializeField] private Button openButton;
    [SerializeField] private Button closeButton;

    public bool IsAnimating => LeanTween.isTweening(overlay.gameObject);

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
        }, 0f, 1f, 0.2f).setIgnoreTimeScale(true);
        overlay.alpha = 1f;

        closeButton.interactable = true;
    }

    public void CloseOptionsMenu()
    {
        closeButton.interactable = false;
        openButton.interactable = false;

        LeanTween.value(overlay.gameObject, value =>
        {
            overlay.alpha = value;
        }, 1f, 0f, 0.2f).setIgnoreTimeScale(true);
        overlay.alpha = 0f;

        closeButton.interactable = true;
        openButton.interactable = true;
    }
}
