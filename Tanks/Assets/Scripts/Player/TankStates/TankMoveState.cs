using UnityEngine;

public class TankMoveState : TankState
{
    public TankMoveState(Tank tank) : base(tank) { }

    public override TankState HandleMovement(Vector2 dir) {
        var isAirborne = false;
        //isAirborne = tank.tankProps.IsAirborne(); // TODO: When gravity gets added, consider this

        if (tank.stunned || (dir.magnitude < 0.1f && !isAirborne)) return new TankIdleState(tank);

        // There is input, move tank
        tank.tankController.MoveTank(dir);

        return this;
    }
}