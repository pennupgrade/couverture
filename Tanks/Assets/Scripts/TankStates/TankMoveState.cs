
using UnityEngine;

public class TankMoveState : TankState
{
    public TankMoveState(Tank tank) : base(tank) {}

    public override TankState HandleMovement(Vector2 dir)
    {  
        float rot = dir.x * tank.rotSpeed;
        float move = dir.y * tank.moveSpeed;

        tank.rb.angularVelocity = new(0, rot, 0);
        tank.rb.velocity = move * tank.transform.right;

        return this;
    }

    public override TankState HandleGunRotation(float val)
    {
        Vector3 rot = tank.gun.transform.rotation.eulerAngles;

        tank.gun.transform.rotation = Quaternion.Euler(rot.x, rot.y + (val * tank.gunRotSpeed * 0.1f), rot.z);
        return this;
    }
}