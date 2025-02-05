using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Launcher_Idle : EnemyIdleState
{
    private int frameTimer;
    public Launcher_Idle(Enemy enemy) : base(enemy) {
        frameTimer = 1;
    }

    public override Enemy_State Patrol(Vector3 playerPos)
    {
        frameTimer--;
        if (frameTimer > 0) {
            return this;
        }
        frameTimer = 6;

        if (getDist() < 12) {
            return new Launcher_Alert(enemy);
        }
        return this;
    }

    public override Enemy_State RotateTurret(Vector3 playerPos) {
        return this;
    }
}
