using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnemyStartState : Enemy_State
{
    private bool changeState;
    private int frameTimer;
    public EnemyStartState(Enemy enemy) : base(enemy) {
        frameTimer = 1;
    }

    public override Enemy_State Patrol(Vector3 playerPos)
    {
        frameTimer--;
        if (frameTimer > 0) {
            return this;
        }
        frameTimer = 10;

        if (changeState || checkIfPlayerDetected(true) || 
            (enemy.moveStartRange != 0 && Vector2.Distance(new Vector2(enemy.rb.position.x, enemy.rb.position.z),
                new Vector2(enemy.playerRB.position.x, enemy.playerRB.position.z)) <= enemy.moveStartRange &&
                Mathf.Abs(enemy.rb.position.y - enemy.playerRB.position.y) < 1.4f))
        {
            enemy.cSpeed = enemy.speed;
            if (enemy.straightLineAtStart) {
                enemy.moveStraightTimer = enemy.StartCoroutine(straightLineTimer());
            }
            return stateToTransitionTo();
        }
        return this;
    }
    protected abstract Enemy_State stateToTransitionTo();
    private IEnumerator straightLineTimer() {
        yield return new WaitForSeconds(1);
        enemy.moveStraightTimer = null;
    }

    public override Enemy_State RotateTurret(Vector3 playerPos)
    {
        if (enemy.idleTurretCor == null) {
            enemy.idleTurretCor = enemy.StartCoroutine(idleTurretTurn());
        }
        return this;
    }

    public override Enemy_State Shoot(Vector3 playerPos) {
        return this;
    }
    public override Enemy_State Move(Vector3 playerPos) {
        return this;
    }

    public void ChangeToIdle() {
        changeState = true;
    }
}
