using UnityEngine;

public class TankMoveState : TankState
{

    public TankMoveState(Tank tank) : base(tank) {
    }

    public override TankState HandleMovement(Vector2 dir)
    {
        bool isAirborne = false;
        //isAirborne = tank.tankProps.IsAirborne(); // TODO: When gravity gets added, consider this

        if (dir.magnitude < 0.1f && !isAirborne) return new TankIdleState(tank);

        // There is input, move tank
        tank.tankController.MoveTank(dir);

        return this;
    }

    public override TankState HandleShoot()
    {
        //Debug.Log("Shoot from MoveState");
        if (tank.numBullets <= 0) {
            return this;
        }

        tank.numBullets--;
        GameObject bullet = Object.Instantiate(tank.bulletPrefab, tank.gunShotPos.position, Quaternion.identity);
        bullet.GetComponent<Rigidbody>().velocity = Quaternion.AngleAxis(30 * (Random.value - 0.5f), Vector3.up)
         * (tank.gun.transform.right * tank.bulletSpeed);

        return new TankShotCooldownState(tank);
    }
}