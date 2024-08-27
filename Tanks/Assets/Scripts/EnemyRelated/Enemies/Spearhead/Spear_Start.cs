using UnityEngine;

public class Spear_Start : EnemyStartState
{
    public Spear_Start(Enemy enemy) : base(enemy) {}
    protected override Enemy_State stateToTransitionTo(bool alert) {
        return (alert) ? new Spear_Alert(enemy) : new Spear_Idle(enemy);
    }
}
