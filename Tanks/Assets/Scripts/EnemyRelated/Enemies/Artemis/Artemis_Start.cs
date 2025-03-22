using UnityEngine;

public class Artemis_Start : EnemyStartState
{
    public Artemis_Start(Enemy enemy) : base(enemy) {}
    protected override Enemy_State stateToTransitionTo(bool alert) {
        return (alert) ? new Artemis_Alert(enemy) : new Artemis_Idle(enemy);
    }
}
