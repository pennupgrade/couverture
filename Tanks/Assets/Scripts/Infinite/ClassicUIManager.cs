using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class ClassicUIManager : MonoBehaviour
{
    public CanvasGroup startScreen;
    public TMP_Text levelText;
    public TMP_Text levelsBeatenText;
    public CanvasGroup completeScreen;
    public CanvasGroup endGameScreen;
    public CanvasGroup deathScreen;
    public Button backToHome;

    private void Start()
    {
        startScreen.alpha = 0;
        completeScreen.alpha = 0;
        completeScreen.gameObject.SetActive(false);
        endGameScreen.alpha = 0;
        endGameScreen.gameObject.SetActive(false);
        deathScreen.alpha = 0;
        deathScreen.gameObject.SetActive(false);
        backToHome.onClick.AddListener(ReturnToHome);
    }

    public void ReturnToHome()
    {
        Debug.Log("loading title screen");
        gameObject.SetActive(false);
        SceneManager.LoadScene("TitleScreen");
    }

    public void StartScreenTextFadeIn(int levelNum) {
        //the background image for the start screen and the complete screen is the exact same, just one canvas
        //so its only the text that changes
        // make text display the current level number
        levelText.text = "Mission: " + levelNum;
        StartCoroutine(Fade(startScreen, true)) ;
    }

    public IEnumerator Fade(CanvasGroup screen, bool fadeInorOut)
    {
        Debug.Log(screen.gameObject.name + ": " + fadeInorOut);
        if (fadeInorOut)
        {
            screen.gameObject.SetActive(true);
            Debug.Log("setactive is true");
            float t = 0f;
            while (t < 1f)
            {
                t += Time.deltaTime;
                screen.alpha = t / 1f;
                yield return null;
            }
            screen.alpha = 1f;
        }
        else
        {
            float t = 0f;
            while (t < 2f)
            {
                t += Time.deltaTime;
                screen.alpha = 1 - t / 2f;
                yield return null;
            }
            screen.alpha = 0f;
            screen.gameObject.SetActive(false);
        }

    }

    public void StartScreenFadeOut() {
        // some transition that removes the start canvas
        StartCoroutine(Fade(startScreen, false));
    }


    public void LevelCompleteScreenFadeIn(bool lvl60Complete) {
        // fade in the level complete screen and text
        // if lvl60Complete then display the end game screen instead
        if (lvl60Complete)
        {
            StartCoroutine(Fade(endGameScreen, true));
        }
        else
        {
            StartCoroutine(Fade(completeScreen, true));
        }
    }


    public void LevelCompleteScreenTextFadeOut() {
        // fade out the level complete text
        StartCoroutine(Fade(completeScreen, false));
    }

    public void DeathScreenFadeIn(int levelNum) {
        // display the end game screen, showing _/60 levels beat,
        // and whether it is a high score or not, return to menu button
        startScreen.gameObject.SetActive(false);
        completeScreen.gameObject.SetActive(false);
        endGameScreen.gameObject.SetActive(false);
        levelsBeatenText.text = "Levels Beat: " + levelNum + "/60";
        StartCoroutine(Fade(deathScreen, true));
    }
}
