using UnityEngine;
using UnityEngine.UI;

public class ClassicRestartQuitCanvas : MonoBehaviour
{
    [SerializeField] private RectTransform rt;
    [SerializeField] private Image overlayImage;
    [SerializeField] private Image leftWaveImage;
    [SerializeField] private Image rightWaveImage;

    private static readonly Color32 Brown = new(78, 63, 56, 255);
    private static readonly Color32 Cream = new(255, 238, 229, 255);

    public bool IsAnimating => LeanTween.isTweening(rt) || LeanTween.isTweening(overlayImage.gameObject);

    public void Restart() {
        LeanTween.moveX(rt, 2100f, 0f).setIgnoreTimeScale(true);
        LeanTween.moveX(rt, 0f, 1f).setEaseOutExpo().setIgnoreTimeScale(true);
    }

    public void Quit() {
        LeanTween.moveX(rt, 2100f, 0f).setIgnoreTimeScale(true);
        LeanTween.moveX(rt, 0f, 1f).setEaseOutExpo().setIgnoreTimeScale(true);

        LeanTween.value(overlayImage.gameObject, value => {
            overlayImage.color = value;
            leftWaveImage.color = value;
            rightWaveImage.color = value;
        }, Cream, Brown, 1.1f).setIgnoreTimeScale(true);
    }
}