using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Bomb_Attack : EnemyAlertState
{
    public Bomb_Attack(Enemy enemy) : base(enemy) {
        enemy.speed += 1;
        enemy.cSpeed = enemy.speed;
    }

    public override Enemy_State Move(Vector3 _)
    {
        if (enemy.wayPointUpdate == null) {
            enemy.wayPointUpdate = enemy.StartCoroutine(recalcPath());
        } else if (hasReachedDest()) {
            enemy.destination = getPlayerPoint(0);
            enemy.agent.SetDestination(enemy.destination);
        }
        turnTowardsVector(enemy.agent.desiredVelocity, 300);
        return this;
    }
    private IEnumerator recalcPath() {
        while (true) {
            enemy.destination = getPlayerPoint(0);
            enemy.agent.SetDestination(enemy.destination);
            yield return new WaitForSeconds(1.6f);
        }
    }

    public override Enemy_State Patrol(Vector3 playerPos)
    {
        return this;
    }

    public override Enemy_State RotateTurret(Vector3 _) {
        return this;
    }
    
    public override Enemy_State Shoot(Vector3 _) {
        //explode
        if (getDist() < 1.4f) {
            enemy.takeDamage(200);
        }
        return this;
    }
}
