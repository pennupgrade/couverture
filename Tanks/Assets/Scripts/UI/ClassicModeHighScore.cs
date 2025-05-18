using TMPro;
using UnityEngine;

public class ClassicModeHighScore : MonoBehaviour
{
    [SerializeField] private string prefix;
    [SerializeField] private TMP_Text scoreText;

    // Start is called before the first frame update
    private void Start() {
        scoreText.text = $"{prefix} {Mathf.Clamp(SaveStateManagerGameObject.GetClassicModeHighScore(), 0, 50)}";
    }
}