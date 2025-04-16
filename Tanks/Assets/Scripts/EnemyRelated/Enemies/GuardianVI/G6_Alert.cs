using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class G6_Alert : EnemyAlertState
{
    protected bool leadPlayer;
    private float rayDist;
    private Vector3 rayPos, rayDir;
    public G6_Alert(Enemy enemy) : base(enemy) {
        enemy.numBullets = enemy.magSize;
        leadPlayer = true;
    }

    public override Enemy_State Move(Vector3 _)
    {
        if (enemy.wayPointUpdate == null) {
            enemy.wayPointUpdate = enemy.StartCoroutine(recalcPath());
        } else if (hasReachedDest()) {
            if (enemy.getHealth() < 200 && Random.value < 0.4f) {
                enemy.destination = getRandomHidePoint(8);
            } else if (getNumEnemies(9) < 2) {
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
                } else if (i == 0 && getNumEnemies(9) < 2) {
                    enemy.destination = getLOSPoint(enemy.playerRB.position, 9, 4.5f);
                } else if (i == 0){
                    enemy.destination = getRandomPoint(6);
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
                enemy.numBullets = enemy.magSize;
            } else {
                yield return null;
            }
        }
    }
    private IEnumerator shootCor() {
        yield return new WaitForSeconds(0.16f);
        while (true) {   
            if (enemy.numBullets > 0 && lineOfSightCheck() && isAimed() && getDist() < enemy.gunRange && checkFriendlyFire(5)) {
                fire(13);
                enemy.numBullets--;
                leadPlayer = Random.value < enemy.leadChance;
                yield return new WaitForSeconds(enemy.cooldownTime);
            } else {
                yield return new WaitForSeconds(0.16f);
            }
        }
    }
}
