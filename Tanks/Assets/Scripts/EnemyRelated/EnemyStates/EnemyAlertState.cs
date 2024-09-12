using UnityEngine;

public abstract class EnemyAlertState : Enemy_State
{
    public EnemyAlertState(Enemy enemy) : base(enemy) {}

    public override Enemy_State Patrol(Vector3 playerPos)
    {
        Debug.Log("Patrolling from EnemyAlertState");
        return this;
    }

    public abstract override Enemy_State RotateTurret(Vector3 playerPos);
    public abstract override Enemy_State Shoot(Vector3 playerPos);

    protected int getNumEnemies(float r) {
        Collider[] hitColliders = new Collider[8];
        return Physics.OverlapSphereNonAlloc(enemy.transform.position, r, hitColliders, 1 << 8);
    }
}
