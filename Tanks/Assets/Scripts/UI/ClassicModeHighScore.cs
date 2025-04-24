using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ClassicModeHighScore : MonoBehaviour
{
    private const string HIGH_SCORE_TEXT = "High score: ";
    [SerializeField] private TMP_Text scoreText;

    // Start is called before the first frame update
    void Start()
    {
        ClassicModeInfo info = new();
        scoreText.text = HIGH_SCORE_TEXT + info.GetClassicModeHighScore();
    }
}
