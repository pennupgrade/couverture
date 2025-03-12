using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lance_Alert : EnemyAlertState
{
    private bool playerGone, leadPlayer;
    public Lance_Alert(Enemy enemy) : base(enemy) {
        leadPlayer = false;
    }

    public override Enemy_State Move(Vector3 _)
    {
        if (enemy.wayPointUpdate == null) {
            enemy.wayPointUpdate = enemy.StartCoroutine(recalcPath());
        } else if (hasReachedDest()) {
            if (Vector3.Distance(((Lance)enemy).homePoint, enemy.rb.position) > 6) {
                enemy.destination = getRandomNavPoint(((Lance)enemy).homePoint, 5);
            } else {
                enemy.destination = getRandomPoint(5);
            }
            enemy.agent.SetDestination(enemy.destination);
        }
        turnTowardsVector(enemy.agent.desiredVelocity, 300);
        return this;
    }
    private IEnumerator recalcPath() {
        while (true) {
            for (int i = 0; i < 4; i++) {
                if (i == 0) {
                    if (Vector3.Distance(((Lance)enemy).homePoint, enemy.rb.position) > 10) {
                        enemy.destination = getRandomNavPoint(((Lance)enemy).homePoint, 6);
                    } else {
                        enemy.destination = getRandomPoint(6);
                    }
                }
                enemy.agent.SetDestination(enemy.destination);
                yield return new WaitForSeconds(3);
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

                return new Lance_Idle(enemy);
            }
        }
        return this;
    }
    private IEnumerator alertPatroller() {
        while (true) {
            yield return new WaitForSeconds(7);
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
            if (lineOfSightCheck() && isAimed() && getDist() < enemy.gunRange && checkFriendlyFire(6)) {
                fire(10);
                leadPlayer = Random.value < enemy.leadChance;
                yield return new WaitForSeconds(enemy.reload);
            } else {
                yield return new WaitForSeconds(0.2f);
            }
        }
    }
}
