using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Artemis_Alert : EnemyAlertState
{
    protected bool playerGone;
    public Artemis_Alert(Enemy enemy) : base(enemy) {
        enemy.audioManager.Stop("Engine");
        enemy.cSpeed = 0;
        ((Artemis)enemy).toggleLaser(1);
        enemy.playSound("Laser");
    }

    public override Enemy_State Move(Vector3 _)
    {
        if (enemy.wayPointUpdate == null) {
            enemy.wayPointUpdate = enemy.StartCoroutine(recalcPath());
        } else if (hasReachedDest()) {
            if (Vector3.Distance(((Artemis)enemy).homePoint, enemy.rb.position) > 5) {
                enemy.destination = getRandomNavPoint(((Artemis)enemy).homePoint, 4);
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
            for (int i = 0; i < 3; ++i) {
                if (i == 0) {
                    if (Vector3.Distance(((Artemis)enemy).homePoint, enemy.rb.position) > 5) {
                        enemy.destination = getRandomNavPoint(((Artemis)enemy).homePoint, 4);
                    } else {
                        enemy.destination = getRandomPoint(5);
                    }
                }
                enemy.agent.SetDestination(enemy.destination);
                yield return new WaitForSeconds(3f);
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
                ((Artemis)enemy).toggleLaser(0);
                enemy.audioManager.Play("Engine");
                enemy.cSpeed = enemy.speed;
                return new Artemis_Idle(enemy);
            }
        }
        return this;
    }
    protected IEnumerator alertPatroller() {
        while (true) {
            yield return new WaitForSeconds(6);
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
        return this;
    }
    private IEnumerator shootCor() {
        ((Artemis)enemy).stopTurnA = true;
        yield return new WaitForSeconds(1.4f);
        ((Artemis)enemy).stopTurnA = false;
        ((Artemis)enemy).toggleLaser(2);
        while (true) {
            float playerDist = getDist();
            if (lineOfSightCheck() && isAimed() && playerDist < enemy.gunRange && checkFriendlyFire(playerDist)) {
                if (((Artemis)enemy).hitPlayer()) {
                    ((Artemis)enemy).toggleLaser(1);
                    yield return new WaitForSeconds(enemy.reload);
                    ((Artemis)enemy).toggleLaser(2);
                } else {
                    yield return new WaitForSeconds(0.1f);
                }
            } else {
                yield return new WaitForSeconds(0.1f);
            }
        }
    }
}
