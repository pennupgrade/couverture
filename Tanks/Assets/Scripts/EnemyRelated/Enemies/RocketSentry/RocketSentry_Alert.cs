using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocketSentry_Alert : EnemyAlertState
{
    private bool playerGone, leadPlayer;
    public RocketSentry_Alert(Enemy enemy) : base(enemy) {
        enemy.reload = 4f;
        leadPlayer = false;
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
                return new RocketSentry_Idle(enemy);
            }
        }
        return this;
    }
    private IEnumerator alertPatroller() {
        while (true) {
            playerGone = !checkIfPlayerDetected(false);
            yield return new WaitForSeconds(8);
        }
    }

    public override Enemy_State RotateTurret(Vector3 playerPos) {
        turnTurretTowardPlayer(leadPlayer);
        return this;
    }
    public override Enemy_State Shoot(Vector3 playerPos) {
        if (enemy.activeShootPeriodically == null) {
            enemy.activeShootPeriodically = enemy.StartCoroutine(shootCor());
        }
        return this;
    }
    private IEnumerator shootCor() {
        yield return new WaitForSeconds(0.16f);
        while (true) {            
            if (lineOfSightCheck() && isAimed() && getDist() < enemy.gunRange) {
                fire(8);
                leadPlayer = Random.value < enemy.leadChance;
                yield return new WaitForSeconds(enemy.reload);
                enemy.reload = Mathf.Max(enemy.reload - 1.2f, 1.5f);
            } else {
                yield return new WaitForSeconds(0.2f);
            }
        }
    }
}
