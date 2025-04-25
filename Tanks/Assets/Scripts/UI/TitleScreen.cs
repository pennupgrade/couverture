using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TitleScreen : MonoBehaviour
{
    [SerializeField] private TMP_Text highScore;

    [Header("Classic mode transition")]
    [SerializeField] private GraphicRaycaster secondCanvasRaycaster;
    [SerializeField] private RectTransform overlay;
    [SerializeField] private RectTransform classicText;
    [SerializeField] private RectTransform highScorePanel;
    [SerializeField] private RectTransform catanks;
    [SerializeField] private Image overlayImage;
    [SerializeField] private Image leftWaveImage;
    [SerializeField] private Image rightWaveImage;

    public void StartCampaign() => StartCoroutine(_StartCampaign());

    private static IEnumerator _StartCampaign() {
        SceneTransition.I.Appear(SceneTransition.TransitionType.Fade);

        var operation = SceneManager.LoadSceneAsync("Save Select")!;
        operation.allowSceneActivation = false;

        yield return new WaitWhile(() => SceneTransition.I.IsAnimating);

        operation.allowSceneActivation = true;
    }

    public void StartClassicMode() => StartCoroutine(_StartClassicMode());

    private IEnumerator _StartClassicMode() {
        secondCanvasRaycaster.enabled = true;

        // SceneTransition should not exist in classic mode
        SceneManager.sceneLoaded -= SceneTransition.I.OnSceneLoaded;
        Destroy(SceneTransition.I.gameObject);

        var operation = SceneManager.LoadSceneAsync("Classic1")!;
        operation.allowSceneActivation = false;

        LeanTween.moveX(overlay, 0f, 1f).setEaseOutExpo();
        LeanTween.moveX(classicText, 0f, 1f).setDelay(0.1f).setEaseOutExpo();
        LeanTween.moveX(highScorePanel, 0f, 1f).setDelay(0.2f).setEaseOutExpo();

        yield return new WaitForSeconds(3f);

        LeanTween.moveX(catanks, -2100f, 1f).setEaseInExpo();
        LeanTween.moveX(classicText, -2100f, 1f).setDelay(0.1f).setEaseInExpo();
        LeanTween.moveX(highScorePanel, -2100f, 1f).setDelay(0.2f).setEaseInExpo();

        var brown = new Color32(78, 63, 56, 255);
        var cream = new Color32(255, 238, 229, 255);
        LeanTween.value(overlay.gameObject, value => {
            overlayImage.color = value;
            leftWaveImage.color = value;
            rightWaveImage.color = value;
        }, brown, cream, 1f).setDelay(0.55f);

        yield return new WaitWhile(() => LeanTween.isTweening(overlay.gameObject));

        operation.allowSceneActivation = true;
    }

    public void OpenOptions() => StartCoroutine(_OpenOptions());

    private static IEnumerator _OpenOptions() {
        SceneTransition.I.Appear(SceneTransition.TransitionType.Fade);

        var operation = SceneManager.LoadSceneAsync("CreditsScreen")!;
        operation.allowSceneActivation = false;

        yield return new WaitWhile(() => SceneTransition.I.IsAnimating);

        operation.allowSceneActivation = true;
    }

    public void Quit() => Application.Quit();
}