using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Meteor_Idle : EnemyIdleState
{
    private int frameTimer;
    public Meteor_Idle(Enemy enemy) : base(enemy) {
        frameTimer = 1;
    }

    public override Enemy_State Patrol(Vector3 playerPos)
    {
        frameTimer--;
        if (frameTimer > 0) {
            return this;
        }
        frameTimer = 6;

        if (enemy.player != null && getDist() < enemy.gunRange) {
            if (enemy.idleTurretCor != null) {
                enemy.StopCoroutine(enemy.idleTurretCor);
                enemy.idleTurretCor = null;
            }
            return new Meteor_Alert(enemy);
        }
        return this;
    }

    public override Enemy_State RotateTurret(Vector3 playerPos) {
        if (enemy.idleTurretCor == null) {
            enemy.idleTurretCor = enemy.StartCoroutine(idleTurretTurn());
        }
        return this;
    }
}
