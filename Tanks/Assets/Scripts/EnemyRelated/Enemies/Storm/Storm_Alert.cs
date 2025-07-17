// SPDX-FileCopyrightText: 2024 The Catanks Contributors
//
// SPDX-License-Identifier: MPL-2.0

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Storm_Alert : G2_Alert
{
    private bool left, rotateLeft;
    public Storm_Alert(Enemy enemy) : base(enemy)
    {
        enemy.numBullets = enemy.magSize;
        left = false;
        rotateLeft = false;
    }

    public override Enemy_State Move(Vector3 _)
    {
        if (enemy.wayPointUpdate == null) {
            enemy.wayPointUpdate = enemy.StartCoroutine(recalcPath());
        } else if (hasReachedDest()) {
            if (enemy.getHealth() < 200 && Random.value < 0.5f) {
                enemy.destination = getRandomHidePoint(8);
            } else if (Random.value < 0.4f) {
                enemy.destination = getRandomNavPointAwayFromPlayer(enemy.transform.position, 7, 2.5f);
            } else {
                enemy.destination = getLOSPoint(enemy.playerRB.position, 7, 2.5f);
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
                } else if (i == 0 && Random.value < 0.5f) {
                    enemy.destination = getRandomNavPointAwayFromPlayer(enemy.transform.position, 7, 2.5f);
                } else if (i == 0) {
                    enemy.destination = getLOSPoint(enemy.playerRB.position, 7, 2.5f);
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
                return new Storm_Idle(enemy);
            }
        }
        return this;
    }
    public override Enemy_State RotateTurret(Vector3 _) {
        if (enemy.playerRB == null) {
            return this;
        }
        enemy.TargetDir = (enemy.playerRB.position - enemy.rb.position).normalized;

        float dir = Vector3.Dot(-enemy.gun.transform.right, enemy.TargetDir);
        if (dir < -0.6f) {
            rotateLeft = false;
        } else if (dir > 0.6f) {
            rotateLeft = true;
        }
        
        if (rotateLeft)
        {
            enemy.cTurretTurn = Mathf.Max(-enemy.rotSpeed, enemy.cTurretTurn - 700 * Time.deltaTime);
        }
        else
        {
            enemy.cTurretTurn = Mathf.Min(enemy.rotSpeed, enemy.cTurretTurn + 700 * Time.deltaTime);
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
            if (enemy.numBullets > 0 && lineOfSightCheck() && getDist() < enemy.gunRange && checkFriendlyFire(5)) {
                ((Storm)enemy).fireRocket(left);
                if (enemy.numBullets == enemy.magSize) enemy.playSound("Laser");
                left = !left;
                enemy.numBullets--;
                yield return new WaitForSeconds(enemy.cooldownTime);
            } else {
                yield return new WaitForSeconds(0.2f);
            }
        }
    }
}
