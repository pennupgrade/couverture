using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleScreen : MonoBehaviour
{
    [SerializeField] private TMP_Text highScore;

    public void StartCampaign() => SceneManager.LoadScene("Save Select");

    public void StartClassicMode() {
        // SceneTransition should not exist in classic mode
        Destroy(SceneTransition.I.gameObject);
        SceneManager.LoadScene("Classic1");
    }

    public void OpenOptions() => SceneManager.LoadScene("CreditsScreen");

    public void Quit() => Application.Quit();
}