using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivateOnEnemiesDefeat : MonoBehaviour
{

    public List<GameObject> enemies;
    public List<GameObject> activateObjects;
    private bool triggered = false;

    void Update()
    {
        enemies.RemoveAll((x) => x == null);
        if (enemies.Count == 0 && !triggered) {
            triggered = true;
            gameObject.GetComponent<Collider>().enabled = false;
            Activate();
        }
    }

    void Activate()
    {
        foreach(GameObject gameObject in activateObjects) {
            Activatable activatable = gameObject.GetComponent<Activatable>();
            if (activatable)
            {
                activatable.activate();
            }
        }
    }
}
