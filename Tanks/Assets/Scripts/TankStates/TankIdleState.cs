
using UnityEngine;

public class TankIdleState : TankState
{
    public TankIdleState(Tank tank) : base(tank) {}

    public override TankState HandleMovement(Vector2 dir)
    {
        bool FrontHit = SimpleRaycastWheel(tank.FrontWheel);
        bool BackHit = SimpleRaycastWheel(tank.BackWheel);

        if (dir.magnitude > 0.1f || (!FrontHit && !BackHit && tank.enableExperimentalGravity)) return new TankMoveState(tank);
        else return this;
    }

    // Nearly identical function from the moveState function
    bool SimpleRaycastWheel(GameObject wheel)
    {
        RaycastHit hit;
        Vector3 origin = wheel.transform.position;
        Vector3 direction = -tank.transform.up;
        Physics.Raycast(origin, direction, out hit, tank.wheelMaxDist);

        if (hit.collider != null)
        {
            Vector3 hitPoint = hit.point;
            float hitDist = Vector3.Distance(hitPoint, origin);

            return hitDist <= tank.groundMargin + 0.01f;
        }

        return false;
    }

    public override TankState HandleShoot()
    {
        // Debug.Log("Shoot from IdleState");
        if (tank.numBullets <= 0) {
            return this;
        }

        tank.numBullets--;
        GameObject bullet = Object.Instantiate(tank.bulletPrefab, tank.gunShotPos.position, Quaternion.identity);
        bullet.GetComponent<Rigidbody>().velocity = -tank.gun.transform.right * tank.bulletSpeed;

        return new TankShotCooldownState(tank);
    }
}