using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleScreen : MonoBehaviour
{
    [SerializeField] private TMP_Text highScore;

    public void StartCampaign() => StartCoroutine(_StartCampaign());

    private static IEnumerator _StartCampaign() {
        SceneTransition.I.Appear(SceneTransition.TransitionType.Fade);

        var operation = SceneManager.LoadSceneAsync("Save Select")!;
        operation.allowSceneActivation = false;

        yield return new WaitWhile(() => SceneTransition.I.IsAnimating);

        operation.allowSceneActivation = true;
    }

    public void StartClassicMode() {
        // SceneTransition should not exist in classic mode
        SceneManager.sceneLoaded -= SceneTransition.I.OnSceneLoaded;
        Destroy(SceneTransition.I.gameObject);
        SceneManager.LoadScene("Classic1");
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