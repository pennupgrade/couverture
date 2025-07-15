// SPDX-FileCopyrightText: 2024 The Catanks Contributors
//
// SPDX-License-Identifier: MPL-2.0

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class Wasp_Idle : EnemyIdleState
{
    private int frameTimer, wpIndex;
    public Wasp_Idle(Enemy enemy) : base(enemy) {
        frameTimer = 1;
        wpIndex = -1;
    }

    public override Enemy_State Move(Vector3 _)
    {
        if (((PatrollingEnemyOmni)enemy).followWaypoints && wpIndex == -1) {
            wpIndex = 0;
            enemy.destination = getRandomNavPoint(((PatrollingEnemyOmni)enemy).waypoints[wpIndex].position, 1);
            wpIndex = ((PatrollingEnemyOmni)enemy).increment(wpIndex);
            enemy.agent.SetDestination(enemy.destination);
        } else if (!((PatrollingEnemyOmni)enemy).followWaypoints && enemy.wayPointUpdate == null) {
            enemy.wayPointUpdate = enemy.StartCoroutine(recalcPath());
        } else if (hasReachedDest()) {
            if (((PatrollingEnemyOmni)enemy).followWaypoints) {
                enemy.destination = getRandomNavPoint(((PatrollingEnemyOmni)enemy).waypoints[wpIndex].position, 1);
                wpIndex = ((PatrollingEnemyOmni)enemy).increment(wpIndex);
            } else {
                enemy.destination = getRandomPoint(8);
            }
            enemy.agent.SetDestination(enemy.destination);
        }
        turnTowardsVectorOmni(enemy.agent.desiredVelocity, 300);
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
            return new Wasp_Alert(enemy);
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
