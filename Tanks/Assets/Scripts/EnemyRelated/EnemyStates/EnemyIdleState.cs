using UnityEngine;

public abstract class EnemyIdleState : Enemy_State
{
    public EnemyIdleState(Enemy enemy) : base(enemy) {}

    public override Enemy_State Patrol(Vector3 playerPos)
    {
        Debug.Log("Patrolling from EnemyIdleState");
        return this;
    }

    public abstract override Enemy_State RotateTurret(Vector3 playerPos);

    public override Enemy_State Shoot(Vector3 playerPos) {
        return this;
    }
}
