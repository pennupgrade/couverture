using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Javelin_Hide : G1_Idle
{
    private bool reloadDone;
    public Javelin_Hide(Enemy enemy) : base(enemy) {
        frameTimer = 1;
        enemy.StartCoroutine(beamCooldown());
    }

    private IEnumerator beamCooldown() {
        yield return new WaitForSeconds(enemy.cooldownTime);
        reloadDone = true;
    }

    public override Enemy_State Move(Vector3 _)
    {
        if (enemy.wayPointUpdate == null) {
            enemy.wayPointUpdate = enemy.StartCoroutine(recalcPath());
        } else if (hasReachedDest()) {
            enemy.destination = getRandomHidePoint(8);
            enemy.agent.SetDestination(enemy.destination);
        }
        turnTowardsVectorOmni(enemy.agent.desiredVelocity, 300);
        return this;
    }
    protected override IEnumerator recalcPath() {
        while (true) {
            for (int i = 0; i < 4; i++) {
                if (getDist() < 3.5f && lineOfSightCheck()) {
                    Vector3 dir = (enemy.rb.position - enemy.playerRB.position).normalized;
                    dir.y = 0;
                    enemy.destination = getRandomNavPointAwayFromPlayer(enemy.rb.position + 4 * dir, 5, 3);
                } else if (i == 0) {
                    enemy.destination = getRandomHidePoint(8);
                }
                enemy.agent.SetDestination(enemy.destination);
                yield return new WaitForSeconds(3);
            }
        }
    }

    public override Enemy_State Patrol(Vector3 playerPos)
    {
        frameTimer--;
        if (frameTimer > 0) {
            return this;
        }
        frameTimer = 6;

        if (reloadDone) {
            if (enemy.idleTurretCor != null) {
                enemy.StopCoroutine(enemy.idleTurretCor);
                enemy.idleTurretCor = null;
            } if (enemy.wayPointUpdate != null) {
                enemy.StopCoroutine(enemy.wayPointUpdate);
                enemy.wayPointUpdate = null;
            }

            return new Javelin_Idle(enemy);
        }
        return this;
    }

    public override Enemy_State RotateTurret(Vector3 _) {
        turnTurretTowardPlayerTimeDelay(false, 0f);
        return this;
    }
}
