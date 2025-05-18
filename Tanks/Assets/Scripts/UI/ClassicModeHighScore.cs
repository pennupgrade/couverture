using TMPro;
using UnityEngine;

public class ClassicModeHighScore : MonoBehaviour
{
    [SerializeField] private string prefix;
    [SerializeField] private TMP_Text scoreText;

    // Start is called before the first frame update
    private void Start() {
        ClassicModeInfo info = new();
        scoreText.text = $"{prefix} {Mathf.Clamp(info.GetClassicModeHighScore(), 0, 50)}";
    }
}