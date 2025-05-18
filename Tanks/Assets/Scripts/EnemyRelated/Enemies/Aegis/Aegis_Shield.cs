using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Aegis_Shield : EnemyAlertState
{
    private bool turretMode, leadPlayer, playerGone;
    public Aegis_Shield(Enemy enemy) : base(enemy) {
        ((EnemyOmniMove)enemy).accel = false;
        enemy.cSpeed = 0.05f;
        enemy.numBullets = 3;
        leadPlayer = false;
        turretMode = true;
    }

    public override Enemy_State Move(Vector3 _)
    {
        if (enemy.wayPointUpdate == null) {
            enemy.wayPointUpdate = enemy.StartCoroutine(recalcPath());
        } else if (hasReachedDest()) {
            enemy.destination = getPlayerPoint(0);
            enemy.agent.SetDestination(enemy.destination);
        }
        turnTowardsVector(enemy.agent.desiredVelocity, 300);
        return this;
    }
    private IEnumerator recalcPath() {
        while (true) {
            for (int i = 0; i < 3; i++) {
                enemy.destination = getPlayerPoint(0);
                if (!lineOfSightCheck() || getDist() > 6f) {
                    turretMode = false;
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
                changeStateHelper();
                return new Aegis_Idle(enemy);
            } else if (!turretMode) {
                changeStateHelper();
                return new Aegis_Alert(enemy);
            }
        }
        return this;
    }

    private void changeStateHelper() {
        ((EnemyOmniMove)enemy).accel = true;
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
    }

    protected IEnumerator alertPatroller() {
        while (true) {
            playerGone = !checkIfPlayerDetected(false);
            yield return new WaitForSeconds(12);
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
                enemy.numBullets++;
            } else {
                yield return null;
            }
        }
    }
    private IEnumerator shootCor() {
        yield return new WaitForSeconds(0.16f);
        while (true) {   
            if (enemy.numBullets > 0 && lineOfSightCheck() && isAimed() && getDist() < enemy.gunRange && checkFriendlyFire(4)) {
                fire(24);
                enemy.numBullets--;
                yield return new WaitForSeconds(enemy.cooldownTime);
            } else {
                yield return new WaitForSeconds(0.16f);
            }
        }
    }
}
