public class TankStats {
    private int health;
    private int numBullets;
    private float moveSpeed;

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