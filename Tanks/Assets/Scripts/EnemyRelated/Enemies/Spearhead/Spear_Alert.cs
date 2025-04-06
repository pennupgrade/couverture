using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Spear_Alert : EnemyAlertState
{
    private bool playerGone, leadPlayer;
    public Spear_Alert(Enemy enemy) : base(enemy) {
        leadPlayer = false;
        enemy.speed = 1.7f;
        enemy.cSpeed = enemy.speed;
    }

    public override Enemy_State Move(Vector3 _)
    {
        if (enemy.wayPointUpdate == null) {
            enemy.wayPointUpdate = enemy.StartCoroutine(recalcPath());
        } else if (hasReachedDest()) {
            enemy.destination = getLOSPoint(enemy.playerRB.position, 4.5f, 3f);
            enemy.agent.SetDestination(enemy.destination);
        }
        turnTowardsVector(enemy.agent.desiredVelocity, 300);
        return this;
    }
    private IEnumerator recalcPath() {
        while (true) {
            for (int i = 0; i < 3; i++) {
                if (getDist() < 3 && lineOfSightCheck()) {
                    Vector3 dir = (enemy.rb.position - enemy.playerRB.position).normalized;
                    dir.y = 0;
                    enemy.destination = getRandomNavPointAwayFromPlayer(enemy.rb.position + 4 * dir, 5, 3);
                } else if (i == 0 && lineOfSightCheck() && getDist() < 5) {
                    enemy.destination = getRandomPoint(3);
                } else if (i == 0){
                    enemy.destination = getLOSPoint(enemy.playerRB.position, 4.5f, 3f);
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
                }   
                return new Spear_Idle(enemy);
            }
        }
        return this;
    }
    private IEnumerator alertPatroller() {
        while (true) {
            yield return new WaitForSeconds(9);
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
                    fire(50);
                    enemy.numBullets--;
                    yield return new WaitForSeconds(enemy.cooldownTime);
                }
            }

            if (enemy.numBullets < enemy.magSize) {
                leadPlayer = Random.value < enemy.leadChance;
                yield return new WaitForSeconds(enemy.reload - enemy.cooldownTime);
                enemy.numBullets = enemy.magSize;
            } else {
                yield return new WaitForSeconds(0.2f);
            }
        }
    }
}
