using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointButton : ActivatorButton
{
    protected override void buttonPressed() {
        foreach (GameObject g in toChange) {
            if (g.TryGetComponent<Activatable>(out Activatable aObj)) {
                aObj.activate();
            } else {
                //for spawning enemies
                g.gameObject.SetActive(!g.activeSelf);
            }
        }
    }
}
