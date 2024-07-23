
using UnityEngine;

public class TankIdleState : TankState
{
    public TankIdleState(Tank tank) : base(tank) {}

    public override TankState HandleMovement(Vector2 dir)
    {  
        if (dir.magnitude > 0.1f) return new TankMoveState(tank);
        else return this;
    }

    public override TankState HandleShoot()
    {
        // Debug.Log("Shoot from IdleState");
        if (tank.numBullets <= 0) {
            return this;
        }

        tank.numBullets--;
        GameObject bullet = Object.Instantiate(tank.bulletPrefab, tank.gunShotPos.position, Quaternion.identity);
        bullet.GetComponent<Rigidbody>().velocity = tank.gun.transform.right * tank.bulletSpeed;

        return new TankShotCooldownState(tank);
    }
}