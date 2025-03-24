using System;

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
        t.health = health;
        t.moveSpeed = moveSpeed;

        // handle transfering number of bullets
        t.numBullets = numBullets;
        if (numBullets < Tank.MAX_BULLETS) {
            t.StartCoroutine(t.tankState.Reload());
        }
    }
}