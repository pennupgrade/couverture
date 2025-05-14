using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkipButtonScript : BulletActivatorButton
{
    public int levelReq;
    public GameObject buttonGroup;
    private bool disabled;

    void Start() {
        disabled = false;
        ClassicModeInfo info = new();
        int i = info.GetClassicModeHighScore();
        if (i < levelReq) {
            buttonGroup.SetActive(false);
        } 
    }

    protected override void buttonPressed() {
        if (audioManager != null) {
            audioManager.Play("Press");
        }
        foreach (GameObject g in toChange) {
            if (g.activeInHierarchy) {
                if (g.TryGetComponent<Activatable>(out Activatable aObj)) {
                    aObj.activate();
                } else if (g.TryGetComponent<SkipButtonScript>(out SkipButtonScript button)) {
                    button.activate();
                } else {
                    g.SetActive(false);
                }
            }
        }
        StartCoroutine(transformButton());
    }
    public void activate() {
        if (disabled) return;
        disabled = true;
        StartCoroutine(transformButton());
    }
}
