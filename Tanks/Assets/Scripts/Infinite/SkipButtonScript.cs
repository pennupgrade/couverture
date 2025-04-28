using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkipButtonScript : Activatable
{
    bool alreadyPressed;

    void Start() {
        alreadyPressed = false;
        
        
        ClassicModeInfo info = new();
        int i = info.GetClassicModeHighScore();
        if (i < 25) {
            gameObject.SetActive(false);
        }
        
    }
    public override void activate() {
        if (alreadyPressed) return;
        alreadyPressed = true;
        RoomManager.Instance.roomTransition(true);
    }
}
