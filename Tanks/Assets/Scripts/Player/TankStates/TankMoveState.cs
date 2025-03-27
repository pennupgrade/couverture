using UnityEngine;

public class TankMoveState : TankState
{
    private float cTurnSpeed;
    public TankMoveState(Tank tank) : base(tank) { cTurnSpeed = 0;}

    public override TankState HandleMovement(Vector2 dir) {
        if (dir.magnitude < 0.1f) return new TankIdleState(tank);

        Vector3 target = -dir.x * tank.transform.forward + dir.y * tank.transform.right;

        float dot = Vector3.Dot(tank.Roomba.transform.right, target);
        if (dot > 0.0001f) {
            cTurnSpeed = -400;
        } else if (dot < -0.0001f) {
            cTurnSpeed = 400;
        } else {
            cTurnSpeed = 0;
        }
        tank.Roomba.transform.localEulerAngles += cTurnSpeed * Time.deltaTime * Vector3.forward;


        // There is input, move tank
        tank.tankController.MoveTank(dir);

        return this;
    }
}