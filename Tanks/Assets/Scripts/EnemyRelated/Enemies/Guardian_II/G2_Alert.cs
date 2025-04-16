using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class G2_Alert : EnemyAlertState
{
    protected bool playerGone, leadPlayer;

    private float rayDist;
    private Vector3 rayPos, rayDir;
    public G2_Alert(Enemy enemy) : base(enemy) {
        enemy.numBullets = 5;
        enemy.speed = 1.6f;
        enemy.cSpeed = enemy.speed;
        leadPlayer = false;
    }

    public override Enemy_State Move(Vector3 _)
    {
        if (enemy.wayPointUpdate == null) {
            enemy.wayPointUpdate = enemy.StartCoroutine(recalcPath());
        } else if (hasReachedDest()) {
            if (enemy.getHealth() < 300 && Random.value < 0.5f) {
                enemy.destination = getRandomHidePoint(8);
            } else {
                enemy.destination = getLOSPoint(enemy.playerRB.position, 7, 3.5f);
            }
            enemy.agent.SetDestination(enemy.destination);
        }
        turnTowardsVector(enemy.agent.desiredVelocity, 300);
        return this;
    }
    private IEnumerator recalcPath() {
        while (true) {
            for (int i = 0; i < 3; i++) {
                if (getDist() < 3.2f && lineOfSightCheck()) {
                    Vector3 dir = (enemy.rb.position - enemy.playerRB.position).normalized;
                    dir.y = 0;
                    enemy.destination = getRandomNavPointAwayFromPlayer(enemy.rb.position + 4 * dir, 5, 3);
                } else if (enemy.getHealth() < 300 && Random.value < 0.5f) {
                    enemy.destination = getRandomHidePoint(8);
                } else if (i == 0 && lineOfSightCheck() && getDist() < 5) {
                    enemy.destination = getRandomPoint(4);
                } else if (i == 0){
                    enemy.destination = getLOSPoint(enemy.playerRB.position, 7, 3.5f);
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
                return new G2_Idle(enemy);
            }
        }
        return this;
    }
    protected IEnumerator alertPatroller() {
        while (true) {
            yield return new WaitForSeconds(16);
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
                if (enemy.numBullets == 1 || Random.value < 0.85f) {
                    enemy.numBullets++;
                } else {
                    enemy.numBullets = enemy.magSize;
                }
            } else {
                yield return new WaitForSeconds(0.1f);
            }
        }
    }
    private IEnumerator shootCor() {
        yield return new WaitForSeconds(0.16f);
        bool LOS = false;
        while (true) {
            LOS = lineOfSightCheck();
            if (enemy.numBullets > 0 && LOS && isAimed() && getDist() < enemy.gunRange && checkFriendlyFire(4)) {
                if (enemy.numBullets == enemy.magSize && getDist() < 5 && Random.value < 0.75f) {
                    int left = (Random.value) < 0.5f ? 1 : -1;
                    for (int i = 0; i < enemy.magSize; i++) {
                        fire(left * (-28 + 14 * i), false);
                        yield return new WaitForSeconds(0.42f);
                    }
                    yield return new WaitForSeconds(enemy.cooldownTime);
                    enemy.numBullets = 1;
                } else if (enemy.numBullets >= 3 || Random.value < 0.8f){
                    fire(24);
                    enemy.numBullets--;
                    leadPlayer = Random.value < enemy.leadChance;
                    yield return new WaitForSeconds(enemy.cooldownTime);
                } else {
                    yield return new WaitForSeconds(enemy.reload);
                }
            } else if (enemy.numBullets > 0 && !LOS && checkFriendlyFire(4)) {
                //bouncing
                for (int i = -32; i <= 32; i += 12) {
                    if (calcBounce(Quaternion.AngleAxis(i, Vector3.up) * enemy.gun.transform.forward)) {
                        fire(i, false);
                        enemy.numBullets--;
                        break;
                    }
                }
                yield return new WaitForSeconds(enemy.cooldownTime);
            } else {
                yield return new WaitForSeconds(0.16f);
            }
        }
    }

    protected bool calcBounce(Vector3 rayDirection) {
        rayDist = 12;
        rayPos = enemy.gun.transform.position;
        rayDir = rayDirection;
        for (int i = 0; i < 2; i++) {
            RaycastHit hit;
            if (Physics.Raycast(rayPos, rayDir, out hit, rayDist, 1 << 3)) {
                //Debug.DrawRay(rayPos, Vector3.Distance(rayPos, hit.point) * rayDir.normalized, Color.red, 1);
                if (Physics.Raycast(rayPos, rayDir, Vector3.Distance(rayPos, hit.point), 1 << 8)) {
                    return false;
                }
                if (Physics.Raycast(rayPos, rayDir, Vector3.Distance(rayPos, hit.point), 1 << 2)) {
                    return true;
                }
                rayDist -= Vector3.Distance(rayPos, hit.point);
                if (rayDist <= 0) return false;
                rayDir = Vector3.Reflect(rayDir, hit.normal);
                rayPos = hit.point + 0.01f * rayDir;

            } else {
                if (Physics.Raycast(rayPos, rayDir, rayDist, 1 << 2)) {
                    if (Physics.Raycast(rayPos, rayDir, rayDist, 1 << 8)) {
                        return false;
                    }
                    return true;
                }
                return false;
            }
        }    
        return false;
    }
}
