
using UnityEngine;

public class TankIdleState : TankState
{
    public TankIdleState(Tank tank) : base(tank) {}

    public override TankState HandleMovement(Vector2 dir)
    {
        if (dir.magnitude > 0.1f && !tank.stunned) return new TankMoveState(tank);
        else return this;
    }
}