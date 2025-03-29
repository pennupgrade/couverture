using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Aegis_Idle : G3_Idle
{
    public Aegis_Idle(Enemy enemy) : base(enemy) {
        enemy.cSpeed = enemy.speed;
        frameTimer = 1;
    }

    public override Enemy_State Patrol(Vector3 playerPos)
    {
        frameTimer--;
        if (frameTimer > 0) {
            return this;
        }
        frameTimer = 6;

        if (checkIfPlayerDetected(true)) {
            if (enemy.idleTurretCor != null) {
                enemy.StopCoroutine(enemy.idleTurretCor);
                enemy.idleTurretCor = null;
            } if (enemy.wayPointUpdate != null) {
                enemy.StopCoroutine(enemy.wayPointUpdate);
                enemy.wayPointUpdate = null;
            }

            return new Aegis_Alert(enemy);
        }
        return this;
    }
}
