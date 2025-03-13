using UnityEngine;

public class G5_Start : EnemyStartState
{
    public G5_Start(Enemy enemy) : base(enemy) {}
    protected override Enemy_State stateToTransitionTo(bool alert) {
        ((EnemyOmniMove)enemy).accel = true;
        return (alert) ? new G5_Alert(enemy) : new G5_Idle(enemy);
    }
}
