// SPDX-FileCopyrightText: 2024 The Catanks Contributors
//
// SPDX-License-Identifier: MPL-2.0

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class G6_Alert : EnemyAlertState
{
    protected bool leadPlayer;
    private bool tryingBounce;
    private Vector3 bouncePos;
    private Coroutine bounceCor;
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
        if (!tryingBounce) {
            turnTurretTowardPlayer(leadPlayer);
        } else {
            turnTurretTowardPos(bouncePos);
        }
        return this;
    }
    public override Enemy_State Shoot(Vector3 _) {
        if (enemy.activeShootPeriodically == null) {
            enemy.activeShootPeriodically = enemy.StartCoroutine(shootCor());
        }
        if (enemy.reloadCor == null) {
            enemy.reloadCor = enemy.StartCoroutine(reloader());
        }
        if (bounceCor == null) {
            tryingBounce = false;
            bounceCor = enemy.StartCoroutine(bounceCoroutine());
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
                yield return new WaitForSeconds(0.2f);
            }
        }
    }
    private IEnumerator shootCor() {
        yield return new WaitForSeconds(0.16f);
        while (true) {
            if (tryingBounce && enemy.numBullets > 0 && Vector3.Dot((bouncePos - enemy.gun.transform.position).normalized, enemy.gun.transform.forward) > 0.98f && checkFriendlyFire(5))  {
                ((Guardian6)enemy).fire();
                tryingBounce = false;
                yield return new WaitForSeconds(enemy.cooldownTime);
            } else if (!tryingBounce && enemy.numBullets > 0 && lineOfSightCheck() && isAimed() && getDist() < enemy.gunRange && checkFriendlyFire(5)) {
                yield return new WaitForSeconds(0.06f);
                
                ((Guardian6)enemy).fire();
                enemy.numBullets--;
                leadPlayer = Random.value < enemy.leadChance;
                
                yield return new WaitForSeconds(enemy.cooldownTime - 0.06f);
            } else {
                yield return new WaitForSeconds(0.2f);
            }
        }
    }

    private IEnumerator bounceCoroutine() {
        while (true) {
            if (!tryingBounce) {
                yield return new WaitForSeconds(enemy.cooldownTime);
                if (calcAllBounces(out Vector3 shootPos)) {
                    if (Vector3.Distance(shootPos, enemy.rb.position) > 1.8f) {
                        tryingBounce = true;
                        bouncePos = shootPos;
                    }
                }
            } else {
                yield return new WaitForSeconds(2 * enemy.cooldownTime);
            }
        }
    }

    private bool calcAllBounces(out Vector3 hitpos) {
        //left
        float dist = Vector3.Distance(enemy.gun.transform.position, enemy.gunShotPos.position);
        Vector3 shootDir;
        //right
        for (int i = 20; i <= 120; i += 20) {
            //right
            shootDir = Quaternion.AngleAxis(i, Vector3.up) * enemy.gun.transform.forward;
            if (calcBounce(shootDir, enemy.gun.transform.position + dist * shootDir, out Vector3 hitPos)) {
                hitpos = hitPos;
                return true;
            }
            //left
            shootDir = Quaternion.AngleAxis(-i, Vector3.up) * enemy.gun.transform.forward;
            if (calcBounce(shootDir, enemy.gun.transform.position + dist * shootDir, out Vector3 hitPos2)) {
                hitpos = hitPos2;
                return true;
            }
        }
        hitpos = Vector3.zero;
        return false;
    }

    private bool calcBounce(Vector3 rayDir, Vector3 rayPos, out Vector3 hitPos) {
        float rayDist = 10;
        hitPos = Vector3.zero;
        for (int i = 0; i < 2; i++) {
            RaycastHit hit;
            if (Physics.Raycast(rayPos, rayDir, out hit, rayDist, 1 << 3)) {
                if (i == 0) {
                    hitPos = hit.point;
                } 
                Debug.DrawRay(rayPos, Vector3.Distance(rayPos, hit.point) * rayDir.normalized, Color.red, 1);
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
