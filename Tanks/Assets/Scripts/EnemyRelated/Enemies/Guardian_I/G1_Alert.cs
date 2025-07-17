// SPDX-FileCopyrightText: 2024 The Catanks Contributors
//
// SPDX-License-Identifier: MPL-2.0

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class G1_Alert : EnemyAlertState
{
    private bool playerGone, leadPlayer;
    public G1_Alert(Enemy enemy) : base(enemy) {
        enemy.numBullets = 3;
        leadPlayer = false;
    }

    public override Enemy_State Move(Vector3 _)
    {
        if (enemy.wayPointUpdate == null) {
            enemy.wayPointUpdate = enemy.StartCoroutine(recalcPath());
        } else if (hasReachedDest()) {
            enemy.destination = getLOSPoint(enemy.playerRB.position, 7, 3.5f);
            enemy.agent.SetDestination(enemy.destination);
        }
        turnTowardsVector(enemy.agent.desiredVelocity, 300);
        return this;
    }
    private IEnumerator recalcPath() {
        while (true) {
            for (int i = 0; i < 3; i++) {
                if (getDist() < 3.2f && lineOfSightCheck()) {
                    Vector3 dir = (enemy.rb.position - enemy.playerRB.position).normalized;
                    dir.y = 0;
                    enemy.destination = getRandomNavPointAwayFromPlayer(enemy.rb.position + 4 * dir, 5, 3);
                } else if (i == 0 && lineOfSightCheck() && getDist() < 5) {
                    enemy.destination = getRandomPoint(4);
                } else if (i == 0){
                    enemy.destination = getLOSPoint(enemy.playerRB.position, 7, 3.5f);
                }
                enemy.agent.SetDestination(enemy.destination);
                yield return new WaitForSeconds(2.5f);
            }
        }
    }

    public override Enemy_State Patrol(Vector3 playerPos)
    {
        if (enemy.alertPatrol == null) {
            playerGone = false;
            enemy.alertPatrol = enemy.StartCoroutine(alertPatroller());
        } else {
            if (playerGone) {
                if (enemy.alertPatrol != null) {
                    enemy.StopCoroutine(enemy.alertPatrol);
                    enemy.alertPatrol = null;
                }   if (enemy.activeShootPeriodically != null) {
                    enemy.StopCoroutine(enemy.activeShootPeriodically);
                    enemy.activeShootPeriodically = null;
                }   if (enemy.wayPointUpdate != null) {
                    enemy.StopCoroutine(enemy.wayPointUpdate);
                    enemy.wayPointUpdate = null;
                }   if (enemy.reloadCor != null) {
                    enemy.StopCoroutine(enemy.reloadCor);
                    enemy.reloadCor = null;
                }
                return new G1_Idle(enemy);
            }
        }
        return this;
    }
    private IEnumerator alertPatroller() {
        while (true) {
            yield return new WaitForSeconds(12);
            playerGone = !checkIfPlayerDetected(false);
        }
    }

    public override Enemy_State RotateTurret(Vector3 _) {
        turnTurretTowardPlayer(leadPlayer);
        return this;
    }
    public override Enemy_State Shoot(Vector3 _) {
        if (enemy.activeShootPeriodically == null) {
            enemy.activeShootPeriodically = enemy.StartCoroutine(shootCor());
        }
        if (enemy.reloadCor == null) {
            enemy.reloadCor = enemy.StartCoroutine(reloader());
        }
        return this;
    }
    private IEnumerator reloader() {
        yield return new WaitForSeconds(0.2f);
        while (true) {
            if (enemy.numBullets < enemy.magSize) {
                yield return new WaitForSeconds(enemy.reload);
                if (enemy.numBullets == 1 || Random.value < 0.82f) {
                    enemy.numBullets++;
                } else {
                    enemy.numBullets = enemy.magSize;
                }
            } else {
                yield return new WaitForSeconds(0.1f);
            }
        }
    }
    private IEnumerator shootCor() {
        yield return new WaitForSeconds(0.16f);
        while (true) {   
            if (enemy.numBullets > 0 && lineOfSightCheck() && isAimed() && getDist() < enemy.gunRange && checkFriendlyFire(4)) {
                if (enemy.numBullets == enemy.magSize && getDist() < 5 && Random.value < 0.6f) {
                    int left = (Random.value) < 0.5f ? 1 : -1;
                    for (int i = 0; i < enemy.magSize; i++) {
                        fire(left * (-10 + 10 * i), false);
                        yield return new WaitForSeconds(enemy.cooldownTime / 2);
                    }
                    yield return new WaitForSeconds(enemy.cooldownTime / 2);
                    enemy.numBullets = 1;
                } else if (enemy.numBullets == enemy.magSize || Random.value < 0.8f){
                    fire(30);
                    enemy.numBullets--;
                    leadPlayer = Random.value < enemy.leadChance;
                    yield return new WaitForSeconds(enemy.cooldownTime);
                } else {
                    yield return new WaitForSeconds(enemy.reload);
                }
            } else {
                yield return new WaitForSeconds(0.16f);
            }
        }
    }
}
