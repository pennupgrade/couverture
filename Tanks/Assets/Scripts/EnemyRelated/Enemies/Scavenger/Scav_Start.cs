using UnityEngine;

public class Scav_Start : EnemyStartState
{
    public Scav_Start(Enemy enemy) : base(enemy) {}
    protected override Enemy_State stateToTransitionTo() {
        return new Scav_Idle(enemy);
    }
}
