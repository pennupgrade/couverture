using UnityEngine;

public class Lance_Start : EnemyStartState
{
    public Lance_Start(Enemy enemy) : base(enemy) {}
    protected override Enemy_State stateToTransitionTo(bool alert) {
        enemy.audioManager.Play("Engine");
        return (alert) ? new Lance_Alert(enemy) : new Lance_Idle(enemy);
    }
}
