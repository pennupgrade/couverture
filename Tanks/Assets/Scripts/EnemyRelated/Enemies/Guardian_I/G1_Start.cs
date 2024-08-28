using UnityEngine;

public class G1_Start : EnemyStartState
{
    public G1_Start(Enemy enemy) : base(enemy) {}
    protected override Enemy_State stateToTransitionTo(bool alert) {
        return (alert) ? new G1_Alert(enemy) : new G1_Idle(enemy);
    }
}
