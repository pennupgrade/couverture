using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Defend_Active : EnemyAlertState
{
    private bool playerGone;
    private int frameTimer;
    private float rayDist;
    private Vector3 rayPos, rayDir;
    public Defend_Active(Enemy enemy) : base(enemy) {
        frameTimer = 1;
    }

    public override Enemy_State Patrol(Vector3 playerPos)
    {
        if (enemy.alertPatrol == null) {
            playerGone = false;
            enemy.alertPatrol = enemy.StartCoroutine(alertPatroller());
        }
        return this;
    }
    private IEnumerator alertPatroller() {
        while (true) {
            playerGone = getDist() > enemy.sightRange;
            yield return new WaitForSeconds(10);
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
            yield return new WaitForSeconds(1.2f);
            if (Vector3.Dot(enemy.gun.transform.forward, enemy.transform.forward) < 0) {
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
        frameTimer = 6;

        if (enemy.reloadCor != null) return this;

        rayDist = 33;
        rayPos = enemy.gun.transform.position;
        rayDir = enemy.gun.transform.forward;
        for (int i = 0; i < 3; i++) {
            RaycastHit hit;
            if (Physics.Raycast(rayPos, rayDir, out hit, rayDist, 1 << 3)) {
                //Debug.DrawRay(rayPos, Vector3.Distance(rayPos, hit.point) * rayDir.normalized, Color.red, 1);
                if (Physics.Raycast(rayPos, rayDir, Vector3.Distance(rayPos, hit.point), 1 << 8)) {
                    return this;
                }
                if (Physics.Raycast(rayPos, rayDir, Vector3.Distance(rayPos, hit.point), 1 << 2)) {
                    fireRockets(6);
                    return this;
                }
                rayDist -= Vector3.Distance(rayPos, hit.point);
                if (rayDist <= 0) return this;
                rayDir = Vector3.Reflect(rayDir, hit.normal);
                rayPos = hit.point + 0.06f * rayDir;

            } else {
                if (Physics.Raycast(rayPos, rayDir, rayDist, 1 << 2)) {
                    if (Physics.Raycast(rayPos, rayDir, rayDist, 1 << 8)) {
                        return this;
                    }
                    fireRockets(6);
                }
                return this;
            }
        }    
        return this;
    }
    private void fireRockets(float dispersion) {
        enemy.reloadCor = enemy.StartCoroutine(reloader());
        if (Random.value < 0.6f) {
            fire(dispersion);
            return;
        }
        enemy.StartCoroutine(shootCor(dispersion * 2));
    }
    private IEnumerator shootCor(float dispersion) {
        ((Defender)enemy).pauseRot = true;
        fire(dispersion);
        yield return new WaitForSeconds(0.5f);
        fire(dispersion);
        ((Defender)enemy).pauseRot = false;
    }
    private IEnumerator reloader() {
        yield return new WaitForSeconds(enemy.reload);
        enemy.reloadCor = null;
    }
}
