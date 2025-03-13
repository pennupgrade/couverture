using UnityEngine;

public class G3_Start : EnemyStartState
{
    public G3_Start(Enemy enemy) : base(enemy) {}
    protected override Enemy_State stateToTransitionTo(bool alert) {
        ((EnemyOmniMove)enemy).accel = true;
        return (alert) ? new G3_Alert(enemy) : new G3_Idle(enemy);
    }
}
