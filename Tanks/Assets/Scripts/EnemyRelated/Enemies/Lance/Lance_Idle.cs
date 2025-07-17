// SPDX-FileCopyrightText: 2024 The Catanks Contributors
//
// SPDX-License-Identifier: MPL-2.0

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lance_Idle : EnemyIdleState
{
    private int frameTimer;
    public Lance_Idle(Enemy enemy) : base(enemy) {
        frameTimer = 1;
    }

    public override Enemy_State Move(Vector3 _)
    {
        if (enemy.wayPointUpdate == null) {
            enemy.wayPointUpdate = enemy.StartCoroutine(recalcPath());
        } else if (hasReachedDest()) {
            if (Vector3.Distance(((Lance)enemy).homePoint, enemy.rb.position) > 7) {
                enemy.destination = getRandomNavPoint(((Lance)enemy).homePoint, 4);
            } else {
                enemy.destination = getRandomPoint(5);
            }
            enemy.agent.SetDestination(enemy.destination);
        }
        turnTowardsVector(enemy.agent.desiredVelocity, 300);
        return this;
    }
    private IEnumerator recalcPath() {
        while (true) {
            for (int i = 0; i < 4; i++) {
                if (i == 0) {
                    if (Vector3.Distance(((Lance)enemy).homePoint, enemy.rb.position) > 8) {
                        enemy.destination = getRandomNavPoint(((Lance)enemy).homePoint, 4);
                    } else {
                        enemy.destination = getRandomPoint(5);
                    }
                }
                enemy.agent.SetDestination(enemy.destination);
                yield return new WaitForSeconds(3.5f);
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

            return new Lance_Alert(enemy);
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
