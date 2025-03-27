
using UnityEngine;

public class TankIdleState : TankState
{
    public TankIdleState(Tank tank) : base(tank) {}

    public override TankState HandleMovement(Vector2 dir)
    {
        tank.Velocity = Vector3.zero;
        if (dir.magnitude > 0.1f) return new TankMoveState(tank);
        else return this;
    }
}