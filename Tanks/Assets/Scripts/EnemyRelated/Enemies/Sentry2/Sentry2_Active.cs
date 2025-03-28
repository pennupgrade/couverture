using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sentry2_Active : EnemyAlertState
{
    private bool playerGone, playerLOS;
    private int frameTimer;
    private float rayDist;
    private Vector3 rayPos, rayDir;
    public Sentry2_Active(Enemy enemy) : base(enemy) {
        ((Sentry2)enemy).pauseRot = false;
        frameTimer = 1;
    }

    public override Enemy_State Patrol(Vector3 playerPos)
    {
        if (enemy.alertPatrol == null) {
            playerGone = false;
            playerLOS = false;
            enemy.alertPatrol = enemy.StartCoroutine(alertPatroller());
        } else if (playerLOS) {
            if (enemy.alertPatrol != null) {
                enemy.StopCoroutine(enemy.alertPatrol);
                enemy.alertPatrol = null;
            } if (enemy.idleTurretCor != null) {
                enemy.StopCoroutine(enemy.idleTurretCor);
                enemy.idleTurretCor = null;
            }
            return new Sentry2_Attack(enemy);
        }
        return this;
    }
    private IEnumerator alertPatroller() {
        while (true) {
            playerGone = getDist() > enemy.sightRange;
            playerLOS = checkIfPlayerDetected(true);

            yield return new WaitForSeconds(2);
        }
    }

    public override Enemy_State RotateTurret(Vector3 playerPos) {
        if (enemy.idleTurretCor == null) {
            enemy.idleTurretCor = enemy.StartCoroutine(turretScan());
        }
        return this;
    }
    protected IEnumerator turretScan() {
        enemy.cTurretTurn = -enemy.rotSpeed;
        while (true) {
            yield return new WaitForSeconds(1f);
            if (Vector3.Dot(enemy.gun.transform.forward, enemy.transform.forward) < -0.16f) {
                if (Vector3.Dot(-enemy.gun.transform.right, enemy.transform.forward) > 0) {
                    enemy.cTurretTurn = -enemy.rotSpeed;
                } else {
                    enemy.cTurretTurn = enemy.rotSpeed;
                }
            }
        }
    }

    public override Enemy_State Shoot(Vector3 playerPos) {
        if (playerGone) return this;
        frameTimer--;
        if (frameTimer > 0) {
            return this;
        }
        frameTimer = 4;

        if (enemy.reloadCor != null) return this;

        rayDist = 12;
        rayPos = enemy.gun.transform.position;
        rayDir = enemy.gun.transform.forward;
        for (int i = 0; i < 2; i++) {
            RaycastHit hit;
            if (Physics.Raycast(rayPos, rayDir, out hit, rayDist, 1 << 3)) {
                //Debug.DrawRay(rayPos, Vector3.Distance(rayPos, hit.point) * rayDir.normalized, Color.red, 1);
                if (Physics.Raycast(rayPos, rayDir, Vector3.Distance(rayPos, hit.point), 1 << 8)) {
                    return this;
                }
                if (Physics.Raycast(rayPos, rayDir, Vector3.Distance(rayPos, hit.point), 1 << 2)) {
                    enemy.activeShootPeriodically = enemy.StartCoroutine(shootCor());
                    return this;
                }
                rayDist -= Vector3.Distance(rayPos, hit.point);
                if (rayDist <= 0) return this;
                rayDir = Vector3.Reflect(rayDir, hit.normal);
                rayPos = hit.point + 0.01f * rayDir;

            } else {
                if (Physics.Raycast(rayPos, rayDir, rayDist, 1 << 2)) {
                    if (Physics.Raycast(rayPos, rayDir, rayDist, 1 << 8)) {
                        return this;
                    }
                    enemy.activeShootPeriodically = enemy.StartCoroutine(shootCor());
                }
                return this;
            }
        }    
        return this;
    }
    private IEnumerator shootCor() {
        enemy.reloadCor = enemy.StartCoroutine(reloader());
        ((Sentry2)enemy).pauseRot = true;
        fire(10);
        yield return new WaitForSeconds(1.5f * enemy.cooldownTime);
        fire(18);
        yield return new WaitForSeconds(1.5f * enemy.cooldownTime);
        fire(25);
        ((Sentry2)enemy).pauseRot = false;
        enemy.activeShootPeriodically = null;
    }
    private IEnumerator reloader() {
        yield return new WaitForSeconds(enemy.reload + 2 * enemy.cooldownTime);
        enemy.reloadCor = null;
    }
}
