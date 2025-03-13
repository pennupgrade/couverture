using UnityEngine;

public class Viper_Start : EnemyStartState
{
    public Viper_Start(Enemy enemy) : base(enemy) {}
    protected override Enemy_State stateToTransitionTo(bool alert) {
        return (alert) ? new Viper_Alert(enemy) : new Viper_Idle(enemy);
    }
}
