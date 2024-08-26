using UnityEngine;

public class Scout_Start : EnemyStartState
{
    public Scout_Start(Enemy enemy) : base(enemy) {}
    protected override Enemy_State stateToTransitionTo(bool alert) {
        return (alert) ? new Scout_Alert(enemy) : new Scout_Idle(enemy);
    }
}
