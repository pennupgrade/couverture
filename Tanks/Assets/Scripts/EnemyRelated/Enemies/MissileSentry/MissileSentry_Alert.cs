using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissileSentry_Alert : EnemyAlertState
{
    private bool playerGone;
    public MissileSentry_Alert(Enemy enemy) : base(enemy) {
        enemy.numBullets = enemy.magSize;
        ((MissileSentry)enemy).toggleLaser();
        enemy.playSound("Laser");
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
                }   if (enemy.reloadCor != null) {
                    enemy.StopCoroutine(enemy.reloadCor);
                    enemy.reloadCor = null;
                }
                ((MissileSentry)enemy).toggleLaser();
                return new MissileSentry_Idle(enemy);
            }
        }
        return this;
    }
    private IEnumerator alertPatroller() {
        while (true) {
            playerGone = !checkIfPlayerDetected(false);
            yield return new WaitForSeconds(6);
        }
    }

    public override Enemy_State RotateTurret(Vector3 playerPos) {
        turnTurretTowardPlayer(false);
        return this;
    }
    public override Enemy_State Shoot(Vector3 playerPos) {
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
                ((MissileSentry)enemy).fireRocket();
                yield return new WaitForSeconds(0.55f);
                ((MissileSentry)enemy).fireRocket();
                yield return new WaitForSeconds(0.55f);
                ((MissileSentry)enemy).fireRocket();
                enemy.numBullets = enemy.magSize - 4;
            } else {
                yield return new WaitForSeconds(0.2f);
            }
        }
    }
}
