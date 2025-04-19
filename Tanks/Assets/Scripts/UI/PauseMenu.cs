using TMPro;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private GameObject panel;
    [SerializeField] private TMP_Text currentStatus;
    
    public void ShowPanel() => panel.SetActive(true);
    public void HidePanel() => panel.SetActive(false);

    public void HandleResume() {
        if (GameManager.Instance != null) {
            // Campaign mode
            GameManager.Instance.paused = false;
            GameManager.Instance.ResumeGame();
            HidePanel();
        }
        else {
            // Classic mode
            Debug.LogWarning("HandleResume(): Classic mode TODO");
        }
    }

    public void HandleOptions() {
        Debug.LogWarning("HandleOptions(): TODO");
    }

    public void HandleQuitToLevelSelect() {
        Debug.LogWarning("HandleQuitToLevelSelect(): TODO");
    }
}
