// SPDX-FileCopyrightText: 2024 The Catanks Contributors
//
// SPDX-License-Identifier: MPL-2.0

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class G4_Alert : G2_Alert
{
    public G4_Alert(Enemy enemy) : base(enemy) {
        enemy.numBullets = 2;
        leadPlayer = false;
    }

    public override Enemy_State Move(Vector3 _)
    {
        if (enemy.wayPointUpdate == null) {
            enemy.wayPointUpdate = enemy.StartCoroutine(recalcPath());
        } else if (hasReachedDest()) {
            if (enemy.getHealth() < 200 && Random.value < 0.5f) {
                enemy.destination = getRandomHidePoint(8);
            } else if (((Guardian4)enemy).defensive) {
                if (Vector3.Distance(((Guardian4)enemy).homePoint, enemy.rb.position) > 6) {
                    enemy.destination = getRandomNavPoint(((Guardian4)enemy).homePoint, 4);
                } else {
                    enemy.destination = getRandomPoint(5);
                }
            } else if (getNumEnemies(9) < 2) {
                enemy.destination = getLOSPoint(enemy.playerRB.position, 9, 4f);
            } else {
                enemy.destination = getLOSPoint(enemy.playerRB.position, 6, 2.5f);
            }
            enemy.agent.SetDestination(enemy.destination);
        }
        turnTowardsVectorOmni(enemy.agent.desiredVelocity, 300);
        return this;
    }
    private IEnumerator recalcPath() {
        while (true) {
            for (int i = 0; i < 3; i++) {
                if (getDist() < 3.5f && lineOfSightCheck()) {
                    Vector3 dir = (enemy.rb.position - enemy.playerRB.position).normalized;
                    dir.y = 0;
                    enemy.destination = getRandomNavPointAwayFromPlayer(enemy.rb.position + 4 * dir, 5, 3);
                } else if (enemy.getHealth() < 200 && Random.value < 0.5f) {
                    enemy.destination = getRandomHidePoint(8);
                } else if (i == 0 && ((Guardian4)enemy).defensive) {
                    if (Vector3.Distance(((Guardian4)enemy).homePoint, enemy.rb.position) > 6) {
                        enemy.destination = getRandomNavPoint(((Guardian4)enemy).homePoint, 4);
                    } else {
                        enemy.destination = getRandomPoint(5);
                    }
                } else if (i == 0 && getNumEnemies(9) < 2) {
                    enemy.destination = getLOSPoint(enemy.playerRB.position, 9, 4.5f);
                } else if (i == 0){
                    enemy.destination = getLOSPoint(enemy.playerRB.position, 6, 2.5f);
                }
                enemy.agent.SetDestination(enemy.destination);
                yield return new WaitForSeconds(2.4f);
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
                return new G4_Idle(enemy);
            }
        }
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
                enemy.numBullets = enemy.magSize;
            } else {
                yield return null;
            }
        }
    }
    private IEnumerator shootCor() {
        yield return new WaitForSeconds(0.16f);
        while (true) {   
            if (enemy.numBullets > 0 && lineOfSightCheck() && isAimed() && getDist() < enemy.gunRange && checkFriendlyFire(5)) {
                fire(12);
                enemy.numBullets--;
                leadPlayer = Random.value < enemy.leadChance;
                yield return new WaitForSeconds(enemy.cooldownTime);
            } else {
                yield return new WaitForSeconds(0.16f);
            }
        }
    }
}
