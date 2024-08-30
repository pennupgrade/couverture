using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Phantom_Idle : G1_Idle
{
    public Phantom_Idle(Enemy enemy) : base(enemy) {}
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

            return new G1_Alert(enemy);
        }
        return this;
    }
}
