using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Viper_Alert : EnemyAlertState
{
    protected bool playerGone;
    public Viper_Alert(Enemy enemy) : base(enemy) {
        enemy.numBullets = enemy.magSize;
        ((Viper)enemy).toggleLaser();
    }

    public override Enemy_State Move(Vector3 _)
    {
        if (enemy.wayPointUpdate == null) {
            enemy.wayPointUpdate = enemy.StartCoroutine(recalcPath());
        } else if (hasReachedDest()) {
            if (Random.value < 0.5f) {
                enemy.destination = getRandomNavPointAwayFromPlayer(enemy.transform.position, 7, 2.5f);
            } else {
                enemy.destination = getLOSPoint(enemy.playerRB.position, 7, 2.5f);
            }
            enemy.agent.SetDestination(enemy.destination);
        }
        turnTowardsVector(enemy.agent.desiredVelocity, 300);
        return this;
    }
    private IEnumerator recalcPath() {
        while (true) {
            for (int i = 0; i < 3; ++i) {
                if (i == 0 && Random.value < 0.5f) {
                    enemy.destination = getRandomNavPointAwayFromPlayer(enemy.transform.position, 7, 2.5f);
                } else if (i == 0) {
                    enemy.destination = getLOSPoint(enemy.playerRB.position, 7, 2.5f);
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
                ((Viper)enemy).toggleLaser();
                return new Viper_Idle(enemy);
            }
        }
        return this;
    }
    protected IEnumerator alertPatroller() {
        while (true) {
            yield return new WaitForSeconds(12);
            playerGone = !checkIfPlayerDetected(false);
        }
    }

    public override Enemy_State RotateTurret(Vector3 _) {
        turnTurretTowardPlayer(false);
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
            if (lineOfSightCheck() && isAimed()) {
                enemy.numBullets--;
            } else {
                enemy.numBullets = enemy.magSize;
            }
            yield return new WaitForSeconds(enemy.cooldownTime);
        }
    }
    private IEnumerator shootCor() {
        yield return new WaitForSeconds(0.16f);
        while (true) {   
            if (enemy.numBullets <= 0 && lineOfSightCheck() && getDist() < enemy.gunRange && checkFriendlyFire(4)) {
                ((Viper)enemy).fireRocket();
                enemy.numBullets = enemy.magSize - 2;
            } else {
                yield return new WaitForSeconds(0.16f);
            }
        }
    }
}
