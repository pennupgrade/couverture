using UnityEngine;

public class Bomb_Start : EnemyStartState
{
    public Bomb_Start(Enemy enemy) : base(enemy) {}
    protected override Enemy_State stateToTransitionTo(bool alert) {
        return (alert) ? new Bomb_Attack(enemy) : new Bomb_Idle(enemy);
    }
}
