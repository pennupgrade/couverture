using System;
using UnityEngine;

[Serializable]
public class TankStats {
    public int health;
    public int numBullets;
    public float moveSpeed;

    // very specific usecases, dont use often!
    public TankStats() { 
        health = 0;
    }

    public TankStats(Tank t) {
        this.health = t.health;
        this.numBullets = t.numBullets;
        this.moveSpeed = t.moveSpeed;
    }

    public void TransferStats(Tank t) {
        //don't transfer for classic mode
        if (RoomManager.Instance != null) return;

        t.health = health;
        t.moveSpeed = moveSpeed;

        // handle transfering number of bullets
        t.numBullets = numBullets;
        if (numBullets < Tank.MAX_BULLETS) {
            if (t.reloadCoroutine != null) {
                MonoBehaviour.print("Tank reloadCoroutine is not null!");
                t.StopCoroutine(t.reloadCoroutine);
            }
            t.reloadCoroutine = t.StartCoroutine(t.tankState.Reload());
        }
    }
}