using UnityEngine;

public class Hornet_Start : EnemyStartState
{
    public Hornet_Start(Enemy enemy) : base(enemy) {}
    protected override Enemy_State stateToTransitionTo(bool alert) {
        ((EnemyOmniMove)enemy).accel = true;
        return (alert) ? new Hornet_Alert(enemy) : new Hornet_Idle(enemy);
    }
}
