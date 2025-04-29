using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class G5_Alert : G2_Alert
{
    public G5_Alert(Enemy enemy) : base(enemy) {
        enemy.numBullets = enemy.magSize;
        leadPlayer = true;
    }

    public override Enemy_State Move(Vector3 _)
    {
        if (enemy.wayPointUpdate == null) {
            enemy.wayPointUpdate = enemy.StartCoroutine(recalcPath());
        } else if (hasReachedDest()) {
            if (!((ShieldedEnemy)enemy).getShieldActivated() || (enemy.getHealth() < 300 && Random.value < 0.4f)) {
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
                } else if (!((ShieldedEnemy)enemy).getShieldActivated()) {
                    enemy.destination = getRandomHidePoint(8);
                } else if (i == 0 && getNumEnemies(9) < 3) {
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
                return new G5_Idle(enemy);
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
                if (enemy.getHealth() < 300) {
                    enemy.numBullets += 2;
                } else {
                    if (Random.value < 0.3f) {
                        enemy.numBullets += 2;
                    } else {
                        enemy.numBullets++;
                    }
                }
            } else {
                yield return new WaitForSeconds(0.1f);
            }
        }
    }
    private IEnumerator shootCor() {
        yield return new WaitForSeconds(0.16f);
        while (true) {   
            if (enemy.numBullets > 0 && lineOfSightCheck() && isAimed() && getDist() < enemy.gunRange && checkFriendlyFire(5)) {
                yield return new WaitForSeconds(0.06f);
                fire(10);
                enemy.numBullets--;
                leadPlayer = Random.value < enemy.leadChance;
                yield return new WaitForSeconds(enemy.cooldownTime - 0.06f);
            } else {
                yield return new WaitForSeconds(0.16f);
            }
        }
    }
}
