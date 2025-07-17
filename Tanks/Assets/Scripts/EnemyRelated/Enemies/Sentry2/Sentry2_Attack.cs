// SPDX-FileCopyrightText: 2024 The Catanks Contributors
//
// SPDX-License-Identifier: MPL-2.0

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Sentry2_Attack : EnemyAlertState
{
    private bool playerGone, leadPlayer;
    public Sentry2_Attack(Enemy enemy) : base(enemy) {
        leadPlayer = false;
        ((Sentry2)enemy).pauseRot = false;
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
                return new Sentry2_Active(enemy);
            }
        }
        return this;
    }
    private IEnumerator alertPatroller() {
        while (true) {
            yield return new WaitForSeconds(3.5f);
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
        return this;
    }
    private IEnumerator shootCor() {
        yield return new WaitForSeconds(0.16f);
        while (true) {       
            if (isAimed() && getDist() < enemy.gunRange && checkFriendlyFire(4)) {
                while (enemy.numBullets > 0 && lineOfSightCheck()) {
                    fire(20);
                    enemy.numBullets--;
                    yield return new WaitForSeconds(enemy.cooldownTime);
                }
            }

            if (enemy.numBullets < enemy.magSize) {
                yield return new WaitForSeconds(enemy.reload - enemy.cooldownTime);
                enemy.numBullets = enemy.magSize;
                leadPlayer = Random.value < enemy.leadChance;
            } else {
                yield return new WaitForSeconds(0.2f);
            }
        }
    }
}
