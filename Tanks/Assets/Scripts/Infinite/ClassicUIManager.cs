using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClassicUIManager : MonoBehaviour
{
    void Start()
    {
        
    }
    public void StartScreenTextFadeIn(int levelNum) {
        //the background image for the start screen and the complete screen is the exact same, just one canvas
        //so its only the text that changes
        // make text display the current level number
    }
    public void StartScreenFadeOut() {
        // some transition that removes the start canvas
    }
    public void LevelCompleteScreenFadeIn(bool lvl60Complete) {
        // fade in the level complete text
        // if lvl60Complete then display the end game screen instead
    }
    public void LevelCompleteScreenTextFadeOut() {
        // ade out the level complete text
    }
    public void DeathScreenFadeIn() {
        // display the end game screen, showing _/60 levels beat
        // whether it is a high score or not, return to menu button
    }
}
