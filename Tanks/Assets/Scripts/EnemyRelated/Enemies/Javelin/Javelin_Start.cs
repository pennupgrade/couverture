using UnityEngine;

public class Javelin_Start : EnemyStartState
{
    public Javelin_Start(Enemy enemy) : base(enemy) {}
    protected override Enemy_State stateToTransitionTo(bool alert) {
        ((EnemyOmniMove)enemy).accel = true;
        return (alert) ? new Javelin_Attack(enemy) : new Javelin_Idle(enemy);
    }
}
