using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Javelin_Idle : G1_Idle
{
    public Javelin_Idle(Enemy enemy) : base(enemy) {
        frameTimer = 1;
    }

    public override Enemy_State Move(Vector3 _)
    {
        if (enemy.wayPointUpdate == null) {
            enemy.wayPointUpdate = enemy.StartCoroutine(recalcPath());
        } else if (hasReachedDest()) {
            enemy.destination = getRandomPoint(7);
            enemy.agent.SetDestination(enemy.destination);
        }
        turnTowardsVectorOmni(enemy.agent.desiredVelocity, 300);
        return this;
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

            return new Javelin_Attack(enemy);
        }
        return this;
    }
    public override Enemy_State RotateTurret(Vector3 _) {
        if (enemy.idleTurretCor == null) {
            enemy.idleTurretCor = enemy.StartCoroutine(idleTurretTurn());
        }
        return this;
    }
}

