using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Artemis_Idle : G1_Idle
{
    public Artemis_Idle(Enemy enemy) : base(enemy) {    
        ((Artemis)enemy).stopTurnA = false;
        enemy.cSpeed = enemy.speed;
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

            return new Artemis_Alert(enemy);
        }
        return this;
    }
}
