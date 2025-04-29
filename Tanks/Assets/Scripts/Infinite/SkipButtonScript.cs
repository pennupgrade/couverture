using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkipButtonScript : Activatable
{
    public Sentry sentryEnemy;
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
        if (sentryEnemy == null) return;
        RoomManager.Instance.roomTransition(true);
        
        sentryEnemy.unsubscribeDeathEvents();
        sentryEnemy.takeDamage(300);
    }
}
