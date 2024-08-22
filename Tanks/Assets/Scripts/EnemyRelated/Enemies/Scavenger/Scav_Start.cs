using UnityEngine;

public class Scav_Start : EnemyStartState
{
    public Scav_Start(Enemy enemy) : base(enemy) {}
    protected override Enemy_State stateToTransitionTo(bool alert) {
        return (alert) ? new Scav_Alert(enemy) : new Scav_Idle(enemy);
    }
}
