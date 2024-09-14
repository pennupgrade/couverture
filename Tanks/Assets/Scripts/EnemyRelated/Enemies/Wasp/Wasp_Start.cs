using UnityEngine;

public class Wasp_Start : EnemyStartState
{
    public Wasp_Start(Enemy enemy) : base(enemy) {}
    protected override Enemy_State stateToTransitionTo(bool alert) {
        ((EnemyOmniMove)enemy).accel = true;
        return (alert) ? new Wasp_Alert(enemy) : new Wasp_Idle(enemy);
    }
}