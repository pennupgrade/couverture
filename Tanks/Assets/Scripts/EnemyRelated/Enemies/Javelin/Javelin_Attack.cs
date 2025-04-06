using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Javelin_Attack : EnemyAlertState
{
    private bool playerGone, leadPlayer;
    public Javelin_Attack(Enemy enemy) : base(enemy) {
        enemy.numBullets = 3;
        leadPlayer = Random.value < enemy.leadChance;
    }

    public override Enemy_State Move(Vector3 _)
    {
        if (enemy.wayPointUpdate == null) {
            enemy.wayPointUpdate = enemy.StartCoroutine(recalcPath());
        } else if (hasReachedDest()) {
            enemy.destination = getLOSPoint(enemy.playerRB.position, 7, 3f);
            enemy.agent.SetDestination(enemy.destination);
        }
        turnTowardsVectorOmni(enemy.agent.desiredVelocity, 300);
        return this;
    }
    private IEnumerator recalcPath() {
        while (true) {
            for (int i = 0; i < 3; i++) {
                if (getDist() < 2.5f && lineOfSightCheck()) {
                    Vector3 dir = (enemy.rb.position - enemy.playerRB.position).normalized;
                    dir.y = 0;
                    enemy.destination = getRandomNavPointAwayFromPlayer(enemy.rb.position + 4 * dir, 5, 3);
                } else if (i == 0 && lineOfSightCheck() && getDist() < 6) {
                    enemy.destination = getRandomPoint(5);
                } else if (i == 0){
                    enemy.destination = getLOSPoint(enemy.playerRB.position, 6, 3f);
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
                deleteCor();
                return new Javelin_Idle(enemy);
            } else if (enemy.numBullets <= 0) {
                deleteCor();
                return new Javelin_Hide(enemy);
            }
        }
        return this;
    }
    private void deleteCor() {
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
    }
    private IEnumerator alertPatroller() {
        while (true) {
            yield return new WaitForSeconds(8);
            playerGone = !checkIfPlayerDetected(false);
        }
    }

    public override Enemy_State RotateTurret(Vector3 _) {
        turnTurretTowardPlayerTimeDelay(leadPlayer, 1.2f);
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
            if (enemy.numBullets > 0 && lineOfSightCheck() && isAimed() && getDist() < enemy.gunRange && Random.value < 0.75f) {
                ((Javelin)enemy).stationary = true;
                enemy.audioManager.Stop("Engine");
                enemy.StartCoroutine(((Javelin)enemy).muzzleFlash());
                yield return new WaitForSeconds(1.25f);
                ((Javelin)enemy).stationary = false;
                enemy.audioManager.Play("Engine");
                fireRailgun();
            } else {
                yield return new WaitForSeconds(0.16f);
            }
        }
    }
    private void fireRailgun() {
        enemy.numBullets = 0;
        
        Vector3 rayPos = enemy.gunShotPos.position;
        Vector3 rayDir = enemy.gun.transform.forward;
        RaycastHit hit;
        if (Physics.Raycast(rayPos, rayDir, out hit, 20, ((Javelin)enemy).lm)) {
            //Debug.DrawRay(rayPos, Vector3.Distance(rayPos, hit.point) * rayDir.normalized, Color.blue, 1);
            if (Physics.Raycast(rayPos, rayDir, Vector3.Distance(rayPos, hit.point), 1 << 2)) {
                if (enemy.player.TryGetComponent<IDestroyable>(out IDestroyable d)) {
                    d.takeDamage(100);
                }
            }
            ((Javelin)enemy).fireBeam(Vector3.Distance(rayPos, hit.point));
        } else {
            if (Physics.Raycast(rayPos, rayDir, 20, 1 << 2)) {
                if (enemy.player.TryGetComponent<IDestroyable>(out IDestroyable d)) {
                    d.takeDamage(100);
                }
            }
            ((Javelin)enemy).fireBeam(20);
        }
    }
}
