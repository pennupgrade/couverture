using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class G1_Idle : EnemyIdleState
{
    protected int frameTimer;
    public G1_Idle(Enemy enemy) : base(enemy) {
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
        turnTowardsVector(enemy.agent.desiredVelocity, 300);
        return this;
    }
    private IEnumerator recalcPath() {
        while (true) {
            for (int i = 0; i < 4; i++) {
                if (i == 0) {
                    enemy.destination = getRandomPoint(6);
                }
                enemy.agent.SetDestination(enemy.destination);
                yield return new WaitForSeconds(3);
            }
        }
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

            return new G1_Alert(enemy);
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
