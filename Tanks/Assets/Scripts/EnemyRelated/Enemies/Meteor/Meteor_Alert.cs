using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Meteor_Alert : EnemyAlertState
{
    private bool playerGone;
    public Meteor_Alert(Enemy enemy) : base(enemy) {
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
                return new Meteor_Idle(enemy);
            }
        }
        return this;
    }
    private IEnumerator alertPatroller() {
        while (true) {
            playerGone = enemy.player != null && getDist() > enemy.gunRange;
            yield return new WaitForSeconds(30);
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
        return this;
    }
    private IEnumerator shootCor() {
        yield return new WaitForSeconds(0.16f);
        while (true) {            
            if (getDist() < enemy.gunRange && getDist() > 4 && isAimed()) {
                Vector3 point = enemy.player.transform.position + enemy.pTank.Velocity + Random.insideUnitSphere * 3.6f;
                point.y = enemy.player.transform.position.y;
                if (Vector3.Distance(point, enemy.rb.position) < 4) {
                    yield return new WaitForSeconds(1);
                } else if (Physics.Raycast(point + 5 * Vector3.up, -Vector3.up, out RaycastHit hit, 7, 1 << 3)){ 
                    if (((Meteor)enemy).fireMissile(hit.point)) {
                        yield return new WaitForSeconds(enemy.reload + Random.value * 2);
                    } else {
                       yield return new WaitForSeconds(2); 
                    }
                } else {
                    yield return new WaitForSeconds(1);
                }
            } else {
                yield return new WaitForSeconds(0.5f);
            }
        }
    }
}
