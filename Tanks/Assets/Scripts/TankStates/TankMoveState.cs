using UnityEngine;

public class TankMoveState : TankState
{
    public TankMoveState(Tank tank) : base(tank) {}

    public override TankState HandleMovement(Vector2 dir)
    {  
        if (dir.magnitude < 0.1f) return new TankIdleState(tank);

        float rot = dir.x * tank.rotSpeed;
        float move = dir.y * tank.moveSpeed;

        tank.rb.angularVelocity = new(0, rot, 0);
        tank.rb.velocity = move * tank.transform.right;

        return this;
    }

    public override TankState HandleShoot()
    {
        // Debug.Log("Shoot from MoveState");

        GameObject bullet = Object.Instantiate(tank.bulletPrefab, tank.gunShotPos.position, Quaternion.identity);
        bullet.GetComponent<Rigidbody>().velocity = tank.gun.transform.right * tank.bulletSpeed;

        return new TankShotCooldownState(tank);
    }
}