// SPDX-FileCopyrightText: 2024 The Catanks Contributors
//
// SPDX-License-Identifier: MPL-2.0

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldSentry_Alert : EnemyAlertState
{
    private bool playerGone;
    public ShieldSentry_Alert(Enemy enemy) : base(enemy) {
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
                }
                return new ShieldSentry_Idle(enemy);
            }
        }
        return this;
    }
    private IEnumerator alertPatroller() {
        while (true) {
            playerGone = !checkIfPlayerDetected(false);
            yield return new WaitForSeconds(10);
        }
    }

    public override Enemy_State RotateTurret(Vector3 playerPos) {
        turnTurretTowardPlayer(false);
        return this;
    }
    public override Enemy_State Shoot(Vector3 playerPos) {
        if (enemy.activeShootPeriodically == null) {
            enemy.activeShootPeriodically = enemy.StartCoroutine(shootCor());
        }
        return this;
    }
    private IEnumerator shootCor() {
        yield return new WaitForSeconds(0.16f);
        while (true) {            
            if (lineOfSightCheck() && isAimed() && getDist() < enemy.gunRange && checkFriendlyFire(3)) {
                fire(30);
                yield return new WaitForSeconds(enemy.reload);
            } else {
                yield return new WaitForSeconds(0.2f);
            }
        }
    }
}
