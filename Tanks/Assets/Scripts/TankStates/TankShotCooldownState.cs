
using System.Collections;
using UnityEngine;

public class TankShotCooldownState : TankMoveState
{
    public TankShotCooldownState(Tank tank) : base(tank) {
        tank.StartCoroutine(endCooldown());
        // Debug.Log("Cooldown begins");
    }

    public override TankState HandleMovement(Vector2 dir)
    {
        base.HandleMovement(dir); // impurity: cannot alternate between TankMoveState and TankIdleState's movement functions
        return this;
    }

    public override TankState HandleShoot()
    {
        return this;
    }

    private IEnumerator endCooldown() {
        yield return new WaitForSeconds(tank.shotCooldownTime);
        // Debug.Log("Cooldown over");
        tank.tankState = new TankIdleState(tank);
    }
}