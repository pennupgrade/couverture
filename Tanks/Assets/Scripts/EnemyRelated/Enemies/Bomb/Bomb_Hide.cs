using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class Bomb_Hide : EnemyIdleState
{
    private bool attackMode;
    public Bomb_Hide(Enemy enemy) : base(enemy) {
        attackMode = false;
        enemy.speed += 0.5f;
        enemy.cSpeed = enemy.speed;
        enemy.StartCoroutine(attackDelay());
    }
    private IEnumerator attackDelay() {
        yield return new WaitForSeconds(6 + 8 * Random.value);
        attackMode = true;
    }

    public override Enemy_State Move(Vector3 _)
    {
        if (enemy.wayPointUpdate == null) {
            enemy.wayPointUpdate = enemy.StartCoroutine(recalcPath());
        } else if (hasReachedDest()) {
            enemy.destination = getRandomHidePoint(8);
            enemy.agent.SetDestination(enemy.destination);
        }
        turnTowardsVector(enemy.agent.desiredVelocity, 300);
        return this;
    }
    private IEnumerator recalcPath() {
        while (true) {
            for (int i = 0; i < 4; i++) {
                if (i == 0) {
                    enemy.destination = getRandomHidePoint(8);
                }
                enemy.agent.SetDestination(enemy.destination);
                yield return new WaitForSeconds(3);
            }
        }
    }

    public override Enemy_State Patrol(Vector3 playerPos)
    {
        if (attackMode) {
            return new Bomb_Attack(enemy);
        }
        return this;
    }

    public override Enemy_State RotateTurret(Vector3 _) {
        return this;
    }
}
