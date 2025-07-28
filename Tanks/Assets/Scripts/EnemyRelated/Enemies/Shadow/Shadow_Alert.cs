// SPDX-FileCopyrightText: 2024 The Catanks Contributors
//
// SPDX-License-Identifier: MPL-2.0

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Shadow_Alert : G2_Alert
{
    public Shadow_Alert(Enemy enemy) : base(enemy) {
        leadPlayer = false;
    }

    public override Enemy_State Move(Vector3 _)
    {
        if (enemy.wayPointUpdate == null) {
            enemy.wayPointUpdate = enemy.StartCoroutine(recalcPath());
        } else if (hasReachedDest()) {
            if (Random.value < 0.3f) {
                enemy.destination = getRandomHidePoint(8);
            } else if (getNumEnemies(9) < 3) {
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
                } else if (i == 0 && getNumEnemies(9) < 3) {
                    enemy.destination = getLOSPoint(enemy.playerRB.position, 9, 4.5f);
                } else if (i == 0) {
                    enemy.destination = enemy.destination = getRandomPoint(5);
                }
                enemy.agent.SetDestination(enemy.destination);
                yield return new WaitForSeconds(2.4f);
            }
        }
    }

    public override Enemy_State Patrol(Vector3 playerPos)
    {
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
            while (((Shadow)enemy).flickering || !lineOfSightCheck()) {
                yield return new WaitForSeconds(0.2f);
            }
            ((Shadow)enemy).turnInvis(false);
            yield return new WaitForSeconds(0.5f);
            int r = Random.Range(3, enemy.magSize + 1);
            for (int i = 0; i < r; ++i) {
                if (lineOfSightCheck() && isAimed() && getDist() < enemy.gunRange && checkFriendlyFire(5)) {
                    fire(16);
                    leadPlayer = Random.value < enemy.leadChance;
                    yield return new WaitForSeconds(enemy.cooldownTime);
                } else {
                    yield return new WaitForSeconds(0.16f);
                    --i;
                }
            }
            yield return new WaitForSeconds(1.8f);
            ((Shadow)enemy).turnInvis(true);
            yield return new WaitForSeconds(enemy.reload + 3 * Random.value - 1.8f);
        }
    }
}
