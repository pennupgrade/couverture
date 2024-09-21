using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class G2_Alert : EnemyAlertState
{
    protected bool playerGone, leadPlayer;
    public G2_Alert(Enemy enemy) : base(enemy) {
        enemy.numBullets = 5;
        enemy.speed = 1.8f;
        leadPlayer = false;
    }

    public override Enemy_State Move(Vector3 _)
    {
        if (enemy.wayPointUpdate == null) {
            enemy.wayPointUpdate = enemy.StartCoroutine(recalcPath());
        } else if (hasReachedDest()) {
            if (enemy.getHealth() < 300 && Random.value < 0.5f) {
                enemy.destination = getRandomHidePoint(8);
            } else {
                enemy.destination = getLOSPoint(enemy.playerRB.position, 7, 3.5f);
            }
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
                } else if (enemy.getHealth() < 300 && Random.value < 0.5f) {
                    enemy.destination = getRandomHidePoint(8);
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
                return new G2_Idle(enemy);
            }
        }
        return this;
    }
    protected IEnumerator alertPatroller() {
        while (true) {
            playerGone = !checkIfPlayerDetected(false);
            yield return new WaitForSeconds(16);
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
                if (enemy.numBullets == 1 || Random.value < 0.85f) {
                    enemy.numBullets++;
                } else {
                    enemy.numBullets = enemy.magSize;
                }
            } else {
                yield return null;
            }
        }
    }
    private IEnumerator shootCor() {
        yield return new WaitForSeconds(0.16f);
        while (true) {   
            if (enemy.numBullets > 0 && lineOfSightCheck() && isAimed() && getDist() < enemy.gunRange && checkFriendlyFire(4)) {
                if (enemy.numBullets == enemy.magSize && getDist() < 5 && Random.value < 0.75f) {
                    int left = (Random.value) < 0.5f ? 1 : -1;
                    for (int i = 0; i < enemy.magSize; i++) {
                        fire(left * (-28 + 14 * i), false);
                        yield return new WaitForSeconds(0.42f);
                    }
                    yield return new WaitForSeconds(enemy.cooldownTime);
                    enemy.numBullets = 1;
                } else if (enemy.numBullets >= 3 || Random.value < 0.8f){
                    fire(24);
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
