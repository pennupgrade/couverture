using UnityEngine;

public class TankMoveState : TankState
{
    public TankMoveState(Tank tank) : base(tank) { }

    public override TankState HandleMovement(Vector2 dir) {
        if (tank.stunned || (dir.magnitude < 0.1f)) return new TankIdleState(tank);

        // There is input, move tank
        tank.tankController.MoveTank(dir);

        return this;
    }
}