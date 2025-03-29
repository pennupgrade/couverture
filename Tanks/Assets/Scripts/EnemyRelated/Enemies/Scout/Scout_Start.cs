using UnityEngine;

public class Scout_Start : EnemyStartState
{
    public Scout_Start(Enemy enemy) : base(enemy) {}
    protected override Enemy_State stateToTransitionTo(bool alert) {
        enemy.audioManager.Play("Engine");
        return (alert) ? new Scout_Alert(enemy) : new Scout_Idle(enemy);
    }
}
